"""Prepare two reusable rear sign faces from preserved source; never save the blend.

Only upper board solids and their source lettering are copied. Full-height posts,
footings and decorative snow are excluded. A rear board sits beyond the post;
letters therefore stay on a real surface rather than intersecting the support.
Run Blender --background --factory-startup --disable-autoexec --python this_file.
"""
import bpy, bmesh, json
from pathlib import Path
from mathutils import Matrix

root=Path(__file__).resolve().parents[2]
bpy.ops.wm.open_mainfile(filepath=str(root/'SourceAssets/AlpineSignBoards/Alpine_Mobile_Sign_Boards_12_Variants.blend'))
scene=bpy.context.scene;deps=bpy.context.evaluated_depsgraph_get()
out=root/'Assets/Snowglow/Catalog/Sources/AlpineSignBoards'
palette=json.loads((out/'ExportMetrics.json').read_text())['palette']
names=[m['name'] for m in palette]
material=bpy.data.materials.new('SignPalette')
rows=[]
for index in ['04','05']:
 coll=next(c for c in bpy.data.collections if c.name.startswith(index+' '))
 origin=next(o for o in coll.all_objects if o.type=='EMPTY').matrix_world.translation
 meshes=[];post_high=-1e9;panel_high=-1e9;solids=0
 for original in list(coll.all_objects):
  if original.type not in {'MESH','FONT'}:continue
  mesh=bpy.data.meshes.new_from_object(original.evaluated_get(deps),depsgraph=deps)
  mesh.transform(Matrix.Translation(-origin)@original.matrix_world)
  bm=bmesh.new();bm.from_mesh(mesh);bm.normal_update()
  if original.type=='MESH':
   remaining=set(bm.faces);remove=[]
   while remaining:
    first=remaining.pop();component={first};pending=[first]
    while pending:
     for edge in pending.pop().edges:
      for face in edge.link_faces:
       if face in remaining:remaining.remove(face);component.add(face);pending.append(face)
    vs={v for f in component for v in f.verts}
    snow=any(mesh.materials[f.material_index].name in ['Fresh snow','Icy snow edges'] for f in component)
    if snow or min(v.co.z for v in vs)<.95:
     if not snow:post_high=max(post_high,max(v.co.y for v in vs))
     remove.extend(component)
    else:
     solids+=1;panel_high=max(panel_high,max(v.co.y for v in vs))
     if all(len(e.link_faces)==2 for f in component for e in f.edges):
      bmesh.ops.recalc_face_normals(bm,faces=list(component));bm.normal_update()
      volume=sum(f.verts[0].co.dot(f.verts[i].co.cross(f.verts[i+1].co))/6 for f in component for i in range(1,len(f.verts)-1))
      if volume<0:bmesh.ops.reverse_faces(bm,faces=list(component))
   bmesh.ops.delete(bm,geom=remove,context='FACES')
   bmesh.ops.delete(bm,geom=[v for v in bm.verts if not v.link_faces],context='VERTS')
  bm.to_mesh(mesh);bm.free()
  if mesh.polygons:meshes.append(mesh)
  else:bpy.data.meshes.remove(mesh)
 assert meshes and solids>0 and post_high>-1e8
 # Rotate around the upright source axis. Keep the inner rear board beyond the
 # complete support depth, then preserve original face-to-lettering offsets.
 shift=post_high+panel_high+.025
 made=[]
 for mesh in meshes:
  slots=list(mesh.materials)
  while mesh.uv_layers:mesh.uv_layers.remove(mesh.uv_layers[0])
  uv=mesh.uv_layers.new(name='PaletteUV')
  for f in mesh.polygons:
   color=names.index(slots[f.material_index].name)
   for loop in f.loop_indices:uv.data[loop].uv=((color+.5)/16,.5)
   f.material_index=0
  mesh.materials.clear();mesh.materials.append(material)
  mesh.transform(Matrix.Translation((0,shift,0))@Matrix.Rotation(3.141592653589793,4,'Z'))
  obj=bpy.data.objects.new('Rear panel and artwork',mesh);scene.collection.objects.link(obj);made.append(obj)
 bpy.ops.object.select_all(action='DESELECT')
 for o in made:o.select_set(True)
 bpy.context.view_layer.objects.active=made[0];bpy.ops.object.join();obj=bpy.context.view_layer.objects.active;obj.name='Rear panel and artwork';obj.data.calc_loop_triangles()
 rows.append(dict(index=index,triangles=len(obj.data.loop_triangles),upperSolids=solids,sourcePostRear=post_high,sourcePanelRear=panel_high,sourceRearShift=shift,clearance=.025))
 bpy.ops.export_scene.fbx(filepath=str(out/('RearPanel'+index+'.fbx')),use_selection=True,object_types={'MESH'},axis_forward='-Z',axis_up='Y',apply_unit_scale=True,apply_scale_options='FBX_SCALE_UNITS',bake_space_transform=True,mesh_smooth_type='FACE',add_leaf_bones=False,bake_anim=False)
 bpy.data.objects.remove(obj,do_unlink=True)
(out/'RearPanelMetrics.json').write_text(json.dumps(dict(source='SourceAssets/AlpineSignBoards/Alpine_Mobile_Sign_Boards_12_Variants.blend',notes='Read-only source preparation. Upper board solids plus lettering; no posts, snow, lights or collisions. Existing shared palette. Rear inner board is 25 mm beyond original support.',variants=rows),indent=2))
print('REAR_PANEL_EXPORT',rows,flush=True)
