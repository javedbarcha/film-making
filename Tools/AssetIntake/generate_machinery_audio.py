"""Original deterministic procedural machinery loops; no external recordings/dependencies."""
import math,wave,struct,json
from pathlib import Path
root=Path(__file__).resolve().parents[2];out=root/'Assets/Snowglow/Audio/Machinery';out.mkdir(parents=True,exist_ok=True)
sr=24000;report=[]
def save(name,seconds,fn):
 n=int(sr*seconds);samples=[fn(i/sr) for i in range(n)];peak=max(abs(x) for x in samples);gain=.65/max(peak,1e-6);samples=[x*gain for x in samples]
 with wave.open(str(out/(name+'.wav')),'wb') as f:f.setparams((1,2,sr,n,'NONE','not compressed'));f.writeframes(b''.join(struct.pack('<h',round(max(-1,min(1,x))*32767)) for x in samples))
 report.append(dict(name=name,seconds=seconds,sample_rate=sr,peak=max(abs(x) for x in samples),rms=math.sqrt(sum(x*x for x in samples)/n),wrap_step=abs(samples[0]-samples[-1])))
def tone(f,t):return math.sin(2*math.pi*f*t)
def diesel(t):
 firing=.48*tone(36,t)+.23*tone(72,t)+.14*tone(108,t)+.08*tone(180,t)
 texture=sum(tone(f,t)*a for f,a in [(211,.025),(317,.019),(443,.012),(691,.009),(1049,.006)])
 return (firing+texture)*(.93+.045*tone(2,t)+.025*tone(3.25,t))
def loaded(t):
 pulse=(.5+.5*tone(36,t))**5-.24609375
 growl=.32*tone(54,t)+.18*tone(108,t)+.12*tone(162,t)
 chatter=sum(tone(f,t)*a for f,a in [(231,.045),(379,.035),(613,.024),(977,.014),(1619,.009)])
 return .6*pulse+growl+chatter*(.55+.45*tone(36,t))
def transmission(t):return (.5*tone(260,t)+.22*tone(520,t)+.09*tone(780,t)+.04*tone(1040,t))*(.9+.06*tone(8,t)+.04*tone(13,t))
save('diesel-base',4,diesel);save('diesel-load',4,loaded);save('transmission',2,transmission)
(root/'SourceAssets/MachineryAudio/ClipMetrics.json').write_text(json.dumps(report,indent=2));print(json.dumps(report))
