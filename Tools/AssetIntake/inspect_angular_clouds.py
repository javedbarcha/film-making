"""Inspect supplied cloud data; never execute embedded Blender text."""
import bpy, json
from pathlib import Path
root=Path(__file__).resolve().parents[2]
bpy.ops.wm.open_mainfile(filepath=str(root/'SourceAssets/AngularClouds/Eight_Angular_Low_Poly_Clouds.blend'))
rows=[]
for o in bpy.data.objects:
    row={'name':o.name,'type':o.type,'collections':[c.name for c in o.users_collection], 'location':list(o.location),'dimensions':list(o.dimensions)}
    if o.type=='MESH':
        o.data.calc_loop_triangles()
        row.update(vertices=len(o.data.vertices),triangles=len(o.data.loop_triangles),materials=[m.name if m else None for m in o.data.materials],modifiers=[m.type for m in o.modifiers])
    rows.append(row)
data={'objects':rows,'collections':[{'name':c.name,'objects':[o.name for o in c.all_objects]} for c in bpy.data.collections], 'materials':[{'name':m.name,'color':list(m.diffuse_color)} for m in bpy.data.materials], 'texts':[t.name for t in bpy.data.texts]}
(root/'Logs/AngularClouds/Inspection.json').write_text(json.dumps(data,indent=2))
print(json.dumps(data,indent=2),flush=True)
