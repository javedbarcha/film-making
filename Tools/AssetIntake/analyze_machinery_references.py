import bpy,aud,numpy as np,json,wave,hashlib
from pathlib import Path
root=Path.cwd();out=root/'Logs/MachineryAudio/ReferenceAnalysis';rows=[]
for path in sorted((root/'SourceAssets/MachineryAudio/References').glob('*.mp3')):
 sound=aud.Sound(str(path));print('SPECS',path.name,sound.specs,flush=True);sound=sound.rechannel(1).resample(24000);data=np.asarray(sound.data(),dtype=np.float32).reshape(-1);np.save(out/(path.stem+'.npy'),data)
 with wave.open(str(out/(path.stem+'.wav')),'wb') as f:f.setparams((1,2,24000,len(data),'NONE','not compressed'));f.writeframes((np.clip(data,-1,1)*32767).astype('<i2').tobytes())
 n=2400;frames=data[:len(data)//n*n].reshape(-1,n);spec=np.abs(np.fft.rfft(frames*np.hanning(n),axis=1));freq=np.fft.rfftfreq(n,1/24000);rms=np.sqrt(np.mean(frames**2,axis=1));tones=spec[:,(freq>400)&(freq<2500)];dom=np.argmax(tones,axis=1);peak=freq[(freq>400)&(freq<2500)][dom];ratio=np.max(tones,axis=1)/(np.sum(tones,axis=1)+1e-8)
 details=[]
 for s in range(int(len(data)/24000)):
  a=s*10;b=min(len(rms),a+10);details.append(dict(t=s,rms=round(float(np.mean(rms[a:b])),4),tone_hz=round(float(np.median(peak[a:b])),1),tone_ratio=round(float(np.max(ratio[a:b])),3)))
 row=dict(file=path.name,seconds=len(data)/24000,sha256=hashlib.sha256(path.read_bytes()).hexdigest(),peak=float(np.max(np.abs(data))),analysis=details);rows.append(row);print('DECODED',path.name,row['seconds'],flush=True)
(out/'Analysis.json').write_text(json.dumps(rows,indent=2));print('DONE',[(r['file'],r['seconds']) for r in rows],flush=True)
