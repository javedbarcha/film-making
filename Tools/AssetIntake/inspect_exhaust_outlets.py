import bpy,json
from pathlib import Path
from mathutils import Vector
rows=[]
for model,path in [('tractor','SourceAssets/LowPolySnowPlower/low_poly_snow_plower.blend'),('loader','SourceAssets/LowPolySnowLoader/snow_loader_lp.blend')]:
 bpy.ops.wm.open_mainfile(filepath=str(Path(path).resolve()));deps=bpy.context.evaluated_depsgraph_get()
 for o in bpy.data.objects:
  if 'exhaust' not in o.name.lower() or o.type!='MESH':continue
  m=bpy.data.meshes.new_from_object(o.evaluated_get(deps),depsgraph=deps);pts=[o.matrix_world@v.co for v in m.vertices];low=[min(v[i] for v in pts) for i in range(3)];high=[max(v[i] for v in pts) for i in range(3)];top=[v for v in pts if v.z>high[2]-.002];tip=sum(top,Vector())/len(top);rows.append(dict(model=model,name=o.name,low=low,high=high,top=list(tip),unity_body_tip=[-tip.y,tip.z,tip.x]));print(model,o.name,low,high,'top',list(tip),flush=True)
Path('Logs/ExhaustSmoke/TipCorrection/SourceOutlets.json').write_text(json.dumps(dict(outlets=rows),indent=2))
