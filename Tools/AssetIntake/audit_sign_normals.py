"""Audit winding of connected source solids without executing embedded blend text."""
import bpy,bmesh,json
from pathlib import Path
from mathutils import Matrix
root=Path(__file__).resolve().parents[2]
bpy.ops.wm.open_mainfile(filepath=str(root/'SourceAssets/AlpineSignBoards/Alpine_Mobile_Sign_Boards_12_Variants.blend'))
rows=[];deps=bpy.context.evaluated_depsgraph_get()
for c in sorted(bpy.data.collections,key=lambda c:c.name):
 if c.name[:2] not in [f'{i:02}' for i in range(1,13)]:continue
 for o in list(c.all_objects):
  if not o or o.type not in {'MESH','FONT'}:continue
  mesh=bpy.data.meshes.new_from_object(o.evaluated_get(deps),depsgraph=deps);mesh.transform(o.matrix_world);bm=bmesh.new();bm.from_mesh(mesh);bm.normal_update();remaining=set(bm.faces);components=[]
  while remaining:
   first=remaining.pop();component={first};todo=[first]
   while todo:
    for edge in todo.pop().edges:
     for face in edge.link_faces:
      if face in remaining:remaining.remove(face);component.add(face);todo.append(face)
   closed=all(len(e.link_faces)==2 for f in component for e in f.edges)
   volume=sum(f.verts[0].co.dot(f.verts[i].co.cross(f.verts[i+1].co))/6 for f in component for i in range(1,len(f.verts)-1))
   before={f:f.normal.copy() for f in component}
   if closed:bmesh.ops.recalc_face_normals(bm,faces=list(component));bm.normal_update()
   changed=sum(before[f].dot(f.normal)<-.9 for f in component)
   components.append({'faces':len(component),'closed':closed,'signedVolumeBefore':volume,'flippedByRecalc':changed})
  rows.append({'collection':c.name,'object':o.name,'type':o.type,'components':components});bm.free();bpy.data.meshes.remove(mesh)
out=root/'SourceAssets/AlpineSignBoards/NormalsAudit-before.json';out.write_text(json.dumps(rows,indent=2))
for r in rows:
 if r['type']=='MESH':print(r['collection'],'closed',sum(c['closed'] for c in r['components']),'negative',sum(c['closed'] and c['signedVolumeBefore']< -1e-9 for c in r['components']),'flipped',sum(c['flippedByRecalc'] for c in r['components']),'open',sum(not c['closed'] for c in r['components']),flush=True)
