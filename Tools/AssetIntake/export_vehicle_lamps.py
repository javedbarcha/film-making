import bpy,bmesh,json,math
from pathlib import Path
from mathutils import Matrix,Vector
root=Path.cwd();out=root/'Assets/Snowglow/Catalog/Sources/VehicleLights';rows=[]
for model,source in [('Tractor','SourceAssets/LowPolySnowPlower/low_poly_snow_plower.blend'),('Loader','SourceAssets/LowPolySnowLoader/snow_loader_lp.blend')]:
 bpy.ops.wm.open_mainfile(filepath=str(root/source));groups={'Front':[],'Rear':[],'Beacon':[]};positions=[]
 material=bpy.data.materials.new('OperatingLamp')
 for o in list(bpy.context.scene.objects):
  if o.type!='MESH':continue
  name=o.name.lower();key=None
  if 'headlight lens' in name or 'work light lens' in name or 'front lower lamp' in name:key='Front'
  elif 'rear orange lamp' in name or 'rear red-orange lamp' in name:key='Rear'
  elif 'amber roof beacon' in name or name=='amber beacon':key='Beacon'
  if key:groups[key].append(o)
 for key,objects in groups.items():
  assert objects,(model,key);made=[]
  for o in objects:
   mesh=o.data.copy();mesh.transform(o.matrix_world)
   bm=bmesh.new();bm.from_mesh(mesh);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.normal_update()
   assert all(e.is_manifold for e in bm.edges),(model,o.name,'open lamp')
   assert bm.calc_volume(signed=True)>0,(model,o.name,'inward lamp')
   bm.to_mesh(mesh);bm.free();mesh.update()
   for v in mesh.vertices:v.co+=v.normal*.004
   # Source rear lenses intersect the ballast/counterweight skin. Move the
   # prepared luminous shells out by 6 cm so both remain visible from behind.
   if key=='Rear':mesh.transform(Matrix.Translation((-.06,0,0)))
   mesh.transform(Matrix.Rotation(-math.pi/2,4,'Z'));mesh.materials.clear();mesh.materials.append(material)
   for f in mesh.polygons:f.material_index=0
   obj=bpy.data.objects.new(key,mesh);bpy.context.scene.collection.objects.link(obj);made.append(obj)
   center=sum((o.matrix_world@v.co for v in o.data.vertices),Vector())/len(o.data.vertices)
   if key=='Front':positions.append({'name':o.name,'position':[center.y,center.z,center.x]})
  bpy.ops.object.select_all(action='DESELECT')
  for o in made:o.select_set(True)
  bpy.context.view_layer.objects.active=made[0];bpy.ops.object.join();bpy.context.object.name=key
  assert len(bpy.context.object.data.materials)==1,(model,key,'uncontrolled material slots')
  bpy.ops.export_scene.fbx(filepath=str(out/(model+key+'.fbx')),use_selection=True,object_types={'MESH'},axis_forward='-Z',axis_up='Y',apply_unit_scale=True,apply_scale_options='FBX_SCALE_UNITS',bake_space_transform=True,bake_anim=False,add_leaf_bones=False)
  bpy.data.objects.remove(bpy.context.object,do_unlink=True)
 rows.append({'model':model,'lights':positions,'parts':{k:[o.name for o in v] for k,v in groups.items()}})
(out/'LampBindings.json').write_text(json.dumps({'vehicles':rows},indent=2))
