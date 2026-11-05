"""Local sample-derived machinery loops; run in Blender for aud MP3 decoding/numpy."""
import aud,numpy as np,wave,json,hashlib
from pathlib import Path
root=Path.cwd();refs=root/'SourceAssets/MachineryAudio/References';out=root/'Assets/Snowglow/Audio/Machinery/Recorded';out.mkdir(parents=True,exist_ok=True);sr=24000
names={'excavator':'freesound_community-excavator-19118','engine':'freesound_community-tractor-engine-27641','diesel':'freesound_community-diesel-tractor-2-55064','tractor':'spinopel-tractor-290811'};data={k:np.asarray(aud.Sound(str(refs/(v+'.mp3'))).rechannel(1).resample(sr).data(),dtype=np.float64).reshape(-1) for k,v in names.items()};rows=[]
def stable(key,start,end,seconds):
 x=data[key];best=None
 for t in np.arange(start,end-seconds,.25):
  a=x[int(t*sr):int((t+seconds)*sr)];r=np.sqrt(np.mean(a[:len(a)//2400*2400].reshape(-1,2400)**2,axis=1)+1e-10);score=np.std(r)/np.mean(r)+.01*np.max(np.abs(a))/(np.mean(r)+1e-6)
  if best is None or score<best[0]:best=(score,float(t))
 return best[1]
def filtered(x,low,high):
 f=np.fft.rfftfreq(len(x),1/sr);gain=(1/(1+(low/np.maximum(f,.001))**6))*(1/(1+(f/high)**8));gain[0]=0;return np.fft.irfft(np.fft.rfft(x)*gain,n=len(x))
def make(name,key,start,duration,band,loop=True):
 x=data[key][int(start*sr):int((start+duration)*sr)].copy();x=filtered(x,*band)
 if loop:
  n=int(.25*sr);theta=np.linspace(0,np.pi/2,n);y=x[n:].copy();y[-n:]=x[-n:]*np.cos(theta)+x[:n]*np.sin(theta);x=y
 else:
  x*=np.sin(np.linspace(0,np.pi,len(x)))**1.3
 rms=np.sqrt(np.mean(x*x));x=np.tanh(x/max(.001,rms*2.7));x*=.65/max(np.max(np.abs(x)),1e-6)
 with wave.open(str(out/(name+'.wav')),'wb') as f:f.setparams((1,2,sr,len(x),'NONE','not compressed'));f.writeframes((x*32767).astype('<i2').tobytes())
 rows.append(dict(name=name,reference=names[key]+'.mp3',start=start,source_seconds=duration,loop_seconds=len(x)/sr,band_hz=band,peak=float(np.max(np.abs(x))),rms=float(np.sqrt(np.mean(x*x))),loop=loop,wrap_step=float(abs(x[-1]-x[0]))));return x
make('tractor-idle','engine',stable('engine',5,38,5.25),5.25,(45,2200))
make('tractor-work','diesel',stable('diesel',20,65,4.25),4.25,(50,2600))
make('tractor-drive','tractor',stable('tractor',3,18,3.25),3.25,(220,2800))
make('loader-idle','excavator',stable('excavator',5,75,5.25),5.25,(45,1800))
make('loader-work','excavator',stable('excavator',280,308,4.25),4.25,(45,2600))
make('loader-drive','excavator',stable('excavator',190,255,3.25),3.25,(220,2800))
# A short reference-textured pneumatic cue: high-frequency machinery air only,
# amplitude shaped to behave as a brake event, never an embedded engine alarm.
x=data['excavator'];candidates=[]
for t in np.arange(5,80,.25):
 a=x[int(t*sr):int((t+.65)*sr)];h=filtered(a,1800,6500);candidates.append((float(np.sqrt(np.mean(h*h))),float(t)))
start=max(candidates)[1];make('brake-air','excavator',start,.65,(1800,6500),False)
manifest=dict(clips=rows,references=[dict(file=v+'.mp3',sha256=hashlib.sha256((refs/(v+'.mp3')).read_bytes()).hexdigest()) for v in names.values()],note='Reference-derived processing; event labels describe game use, not verified original event identity. No embedded instructions executed.')
(root/'SourceAssets/MachineryAudio/RecordedManifest.json').write_text(json.dumps(manifest,indent=2));print(json.dumps(rows),flush=True)
