"""Inspect supplied Blender data; run with --disable-autoexec, never execute embedded text."""
import bpy,json
from pathlib import Path
from mathutils import Vector
root=Path(__file__).resolve().parents[2]
bpy.ops.wm.open_mainfile(filepath=str(root/'SourceAssets/AlpineSignBoards/Alpine_Mobile_Sign_Boards_12_Variants.blend'))
deps=bpy.context.evaluated_depsgraph_get()
rows=[]
for o in bpy.data.objects:
    r={'name':o.name,'type':o.type,'parent':o.parent.name if o.parent else None,'collections':[c.name for c in o.users_collection],'position':list(o.matrix_world.translation)}
    if o.type in {'MESH','CURVE','FONT'}:
        e=o.evaluated_get(deps);m=e.to_mesh();m.calc_loop_triangles();pts=[o.matrix_world@v.co for v in m.vertices]
        r.update(vertices=len(m.vertices),triangles=len(m.loop_triangles),materials=[s.material.name if s.material else None for s in o.material_slots],bounds=[[min(p[i] for p in pts) for i in range(3)],[max(p[i] for p in pts) for i in range(3)]])
        e.to_mesh_clear()
    if o.type=='LIGHT':r.update(lightType=o.data.type,energy=o.data.energy,color=list(o.data.color),radius=o.data.shadow_soft_size)
    rows.append(r)
data={'units':bpy.context.scene.unit_settings.scale_length,'collections':[{'name':c.name,'children':[x.name for x in c.children],'objects':[o.name for o in c.objects]} for c in bpy.data.collections],'objects':rows,'texts':[t.name for t in bpy.data.texts],'materials':[{'name':m.name,'color':list(m.diffuse_color)} for m in bpy.data.materials]}
(root/'SourceAssets/AlpineSignBoards/Inspection.json').write_text(json.dumps(data,indent=2))
print('INSPECTED',len(rows),'objects',len(data['collections']),'collections',flush=True)
