"""Original synthesized heavy-machine backup alarm: 0.36s beep, 0.49s gap."""
import math, struct, wave
from pathlib import Path
root=Path(__file__).resolve().parents[2]
rate=22050
samples=[]
for i in range(round(rate*.85)):
    t=i/rate
    envelope=max(0,min(1,t/.008,(.36-t)/.012)) if t<.36 else 0
    tone=math.sin(2*math.pi*780*t)+.18*math.sin(2*math.pi*1560*t)
    samples.append(round(32767*.48*envelope*tone))
path=root/'Assets/Snowglow/Audio/reverse-alarm.wav'
with wave.open(str(path),'wb') as out:
    out.setnchannels(1);out.setsampwidth(2);out.setframerate(rate)
    out.writeframes(b''.join(struct.pack('<h',s) for s in samples))
print(path, len(samples), 'samples; peak',max(abs(s) for s in samples)/32767)
