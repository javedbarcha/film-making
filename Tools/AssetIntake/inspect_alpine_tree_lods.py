import bpy,json
from pathlib import Path
root=Path(__file__).resolve().parents[2]
bpy.ops.wm.open_mainfile(filepath=str(root/'SourceAssets/AlpineTreesLowestLOD/Alpine_Trees_26_Variants_4_LODs.blend'))
deps=bpy.context.evaluated_depsgraph_get()
rows=[]
for o in bpy.data.objects:
 r={'name':o.name,'type':o.type,'collections':[c.name for c in o.users_collection],'location':list(o.location)}
 if o.type=='MESH':
  e=o.evaluated_get(deps);m=e.to_mesh();m.calc_loop_triangles();r.update(vertices=len(m.vertices),triangles=len(m.loop_triangles),materials=[x.name if x else None for x in m.materials],modifiers=[x.type for x in o.modifiers]);e.to_mesh_clear()
 rows.append(r)
data={'collections':[{'name':c.name,'children':[x.name for x in c.children],'objects':[o.name for o in c.objects]} for c in bpy.data.collections], 'objects':rows,'materials':[{'name':m.name,'color':list(m.diffuse_color)} for m in bpy.data.materials],'texts':[t.name for t in bpy.data.texts]}
(root/'SourceAssets/AlpineTreesLowestLOD/Inspection.json').write_text(json.dumps(data,indent=2))
print('INSPECTION_COMPLETE',len(rows),'objects',flush=True)
