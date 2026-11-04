"""Export preserved sign-board copies, including evaluated FONT artwork. No embedded code execution."""
import bpy,bmesh,json,sys
from pathlib import Path
from mathutils import Matrix
root=Path(__file__).resolve().parents[2]
bpy.ops.wm.open_mainfile(filepath=str(root/'SourceAssets/AlpineSignBoards/Alpine_Mobile_Sign_Boards_12_Variants.blend'))
out=root/'Assets/Snowglow/Catalog/Sources/AlpineSignBoards';out.mkdir(parents=True,exist_ok=True)
colls=sorted([c for c in bpy.data.collections if c.name[:2] in [f'{i:02}' for i in range(1,13)]],key=lambda c:c.name)
deps=bpy.context.evaluated_depsgraph_get();scene=bpy.context.scene
materials=sorted({s.material for c in colls for o in c.all_objects if o.type in {'MESH','FONT'} for s in o.material_slots if s.material},key=lambda m:m.name)
names=[m.name for m in materials]
def srgb(v):return 12.92*v if v<=.0031308 else 1.055*v**(1/2.4)-.055
image=bpy.data.images.new('Signs palette',16,1,alpha=False);image.colorspace_settings.name='Non-Color';image.pixels=[v for i in range(16) for v in ([srgb(c) for c in materials[i].diffuse_color[:3]]+[1] if i<len(materials) else [1,1,1,1])];image.filepath_raw=str(out/'Palette.png');image.file_format='PNG';image.save()
target={k:bpy.data.materials.new('SignSnow' if k=='SnowCap' else 'SignPalette') for k in ('Body','Lettering','SnowCap')}
rows=[];mapping={};normal_audits=[]
selected=set(sys.argv[sys.argv.index('--')+1:]) if '--' in sys.argv else {f'{i:02}' for i in range(1,13)}
def volume(faces):
 return sum(f.verts[0].co.dot(f.verts[i].co.cross(f.verts[i+1].co))/6 for f in faces for i in range(1,len(f.verts)-1))
def repair(mesh,name,kind):
 bm=bmesh.new();bm.from_mesh(mesh);bm.normal_update();remaining=set(bm.faces);closed=negative=flipped=0;after_negative=0;open_parts=0
 while remaining:
  first=remaining.pop();component={first};pending=[first]
  while pending:
   for edge in pending.pop().edges:
    for face in edge.link_faces:
     if face in remaining:remaining.remove(face);component.add(face);pending.append(face)
  if all(len(e.link_faces)==2 for f in component for e in f.edges):
   closed+=1;before={f:f.normal.copy() for f in component};negative+=volume(component)<-1e-9
   bmesh.ops.recalc_face_normals(bm,faces=list(component));bm.normal_update()
   if volume(component)<-1e-9:bmesh.ops.reverse_faces(bm,faces=list(component));bm.normal_update()
   flipped+=sum(before[f].dot(f.normal)<-.9 for f in component);after_negative+=volume(component)<-1e-9
  else:open_parts+=1
 assert after_negative==0,name
 bm.to_mesh(mesh);bm.free();normal_audits.append({'object':name,'kind':kind,'closedSolids':closed,'inwardBefore':negative,'flippedFaces':flipped,'inwardAfter':after_negative,'openPartsPreserved':open_parts})

for coll in colls:
 origin=next(o for o in coll.all_objects if o.type=='EMPTY').matrix_world.translation.copy();groups={};made=[];source_names=[]
 normalized={}
 for original in list(coll.all_objects):
  if original.type not in {'MESH','FONT'}:continue
  copy=bpy.data.meshes.new_from_object(original.evaluated_get(deps),depsgraph=deps);copy.transform(Matrix.Translation(-origin)@original.matrix_world)
  repair(copy,original.name,original.type);normalized[original.name]=copy

 for group in ('Body','Lettering','SnowCap'):
  meshes=[]
  for obj in list(coll.all_objects):
   if obj.type not in {'MESH','FONT'} or (obj.type=='FONT')!=(group=='Lettering'):continue
   mesh=normalized[obj.name].copy();slots=[m.name for m in mesh.materials]
   bm=bmesh.new();bm.from_mesh(mesh)
   if obj.type=='MESH':
    remove=[f for f in bm.faces if (slots[f.material_index] in ('Fresh snow','Icy snow edges'))!=(group=='SnowCap')];bmesh.ops.delete(bm,geom=remove,context='FACES');bmesh.ops.delete(bm,geom=[v for v in bm.verts if not v.link_faces],context='VERTS')
   bm.to_mesh(mesh);bm.free()
   if not mesh.polygons:bpy.data.meshes.remove(mesh);continue
   while mesh.uv_layers:mesh.uv_layers.remove(mesh.uv_layers[0])
   uv=mesh.uv_layers.new(name='PaletteUV')
   for face in mesh.polygons:
    ix=names.index(slots[face.material_index])
    for loop in face.loop_indices:uv.data[loop].uv=((ix+.5)/16,.5)
    face.material_index=0
   mesh.materials.clear();mesh.materials.append(target[group]);objcopy=bpy.data.objects.new(group,mesh);scene.collection.objects.link(objcopy);meshes.append(objcopy);source_names.append(obj.name)
  if not meshes:continue
  bpy.ops.object.select_all(action='DESELECT')
  for obj in meshes:obj.select_set(True)
  bpy.context.view_layer.objects.active=meshes[0];bpy.ops.object.join();obj=bpy.context.view_layer.objects.active;obj.name=group;obj.data.calc_loop_triangles();made.append(obj)
  groups[group]={'triangles':len(obj.data.loop_triangles),'vertices':len(obj.data.vertices)}
 for name,pos in [('BasisX',(1,0,0)),('BasisY',(0,1,0)),('BasisZ',(0,0,1))]:
  obj=bpy.data.objects.new(name,None);scene.collection.objects.link(obj);obj.location=pos;made.append(obj)
 bpy.ops.object.select_all(action='DESELECT')
 for obj in made:obj.select_set(True)
 slug='Sign'+coll.name[:2]
 if coll.name[:2] in selected:bpy.ops.export_scene.fbx(filepath=str(out/(slug+'.fbx')),use_selection=True,object_types={'MESH','EMPTY'},axis_forward='-Z',axis_up='Y',apply_unit_scale=True,apply_scale_options='FBX_SCALE_UNITS',bake_space_transform=True,mesh_smooth_type='FACE',add_leaf_bones=False,bake_anim=False)
 pts=[v.co for obj in made if obj.type=='MESH' for v in obj.data.vertices];low=[min(v[i] for v in pts) for i in range(3)];high=[max(v[i] for v in pts) for i in range(3)]
 rows.append({'index':coll.name[:2],'name':coll.name[3:],'model':slug,'size':[high[0]-low[0],high[2]-low[2],high[1]-low[1]],'groups':groups,'triangles':sum(g['triangles'] for g in groups.values()),'low':low,'high':high})
 mapping[coll.name]={'included':sorted(set(source_names)),'origin':list(origin),'text':[o.data.body for o in coll.all_objects if o.type=='FONT']}
 for obj in made:bpy.data.objects.remove(obj,do_unlink=True)
 for mesh in normalized.values():bpy.data.meshes.remove(mesh)
(out/'ExportMetrics.json').write_text(json.dumps({'signs':rows,'palette':[{'name':m.name,'color':list(m.diffuse_color)} for m in materials]},indent=2))
(out/'NormalsAudit.json').write_text(json.dumps(normal_audits,indent=2))
(out/'PartMap.json').write_text(json.dumps({'collections':mapping,'excluded':['00 Studio | exclude for export','READ ME | Alpine Sign Kit (not executed)']},indent=2))
print('SIGN_EXPORT_COMPLETE',[(r['index'],r['triangles']) for r in rows],flush=True)
