import numpy as np,wave,json
from pathlib import Path
root=Path.cwd();out=root/'Logs/MachineryAudio/ReferenceAnalysis';sr=24000
clips={}
for p in (root/'Assets/Snowglow/Audio/Machinery/Recorded').glob('*.wav'):
 with wave.open(str(p),'rb') as f:clips[p.stem]=np.frombuffer(f.readframes(f.getnframes()),dtype='<i2').astype(float)/32768
with wave.open(str(root/'Assets/Snowglow/Audio/reverse-alarm.wav'),'rb') as f:
 a=np.frombuffer(f.readframes(f.getnframes()),dtype='<i2').astype(float)/32768;rate=f.getframerate();alarm=np.interp(np.arange(round(len(a)*sr/rate))*rate/sr,np.arange(len(a)),a)
results=[]
for machine in ['tractor','loader']:
 t=np.arange(20*sr)/sr;s=np.zeros_like(t);th=np.zeros_like(t);load=np.zeros_like(t)
 ramp=(t>=3)&(t<7);s[ramp]=(t[ramp]-3)/4*.65;th[ramp]=.8
 cruise=(t>=7)&(t<11);s[cruise]=.65;th[cruise]=.6;load[cruise]=.1
 work=(t>=11)&(t<15);s[work]=.12;th[work]=1;load[work]=1
 rev=t>=16;s[rev]=.28;th[rev]=.6;load[rev]=.25
 # Offline listening demo; live game curves use actual physics telemetry.
 for arr in [s,th,load]:
  for i in range(1,len(arr)):arr[i]=arr[i-1]+(arr[i]-arr[i-1])*(1-np.exp(-1/(sr*.18)))
 r=np.clip(s*.52+th*.34+s*.16-load*.1,0,1);pitch=(.9+r*.4)*(.96 if machine=='loader' else 1);gain=(.115+th*.055+s*.035+load*.055)*(1.08 if machine=='loader' else 1);mix=np.zeros_like(t)
 for key,p,g in [('idle',pitch,gain),('work',pitch*.98,load*(.07+th*.1)),('drive',(.85+s*.45+load*.02)*(.98 if machine=='loader' else 1),np.clip(s*3,0,1)*(.012+s*.038+load*.025))]:
  x=clips[machine+'-'+key];phase=np.cumsum(p)%len(x);mix+=np.interp(phase,np.arange(len(x)),x)*g
 a=clips['brake-air'];start=int(15*sr);mix[start:start+len(a)]+=a*.2
 idx=np.where(rev)[0];mix[idx]+=alarm[np.arange(len(idx))%len(alarm)]*.35
 results.append(mix)
preview=np.concatenate([results[0],np.zeros(sr),results[1]])
with wave.open(str(out/'Machinery-recorded-preview.wav'),'wb') as f:f.setparams((1,2,sr,len(preview),'NONE','not compressed'));f.writeframes((np.clip(preview,-1,1)*32767).astype('<i2').tobytes())
(out/'PreviewMetrics.json').write_text(json.dumps(dict(seconds=len(preview)/sr,peak=float(np.max(np.abs(preview))),rms=float(np.sqrt(np.mean(preview**2))),sequence='tractor 20s, 1s gap, loader 20s; each: idle, acceleration, cruise, struggle, brake, reverse'),indent=2))
print('PREVIEW',np.max(np.abs(preview)))
