"""Prepare traction ramp read-only. Studio/embedded scripts are never imported or executed."""
import bpy,bmesh,json,hashlib
from pathlib import Path
from mathutils import Matrix
root=Path(__file__).resolve().parents[2];src=root/'SourceAssets/TractionTestRamp/Low_Poly_Vehicle_Traction_Test_Ramp.blend';out=root/'Assets/Snowglow/Catalog/Sources/TractionTestRamp'
bpy.ops.wm.open_mainfile(filepath=str(src));
for c in bpy.data.collections:c.hide_viewport=False
for o in bpy.data.objects:o.hide_viewport=False;o.hide_set(False)
bpy.context.view_layer.update();deps=bpy.context.evaluated_depsgraph_get()
visual=next(o for o in bpy.data.objects if o.name=='Traction ramp | visible mesh');mats=list(visual.data.materials)
def srgb(v):return 12.92*v if v<=.0031308 else 1.055*v**(1/2.4)-.055
im=bpy.data.images.new('Ramp palette',16,1,alpha=False);im.colorspace_settings.name='Non-Color';im.pixels=[v for i in range(16) for v in ([srgb(c) for c in mats[i].diffuse_color[:3]]+[1] if i<len(mats) else [1,1,1,1])];im.filepath_raw=str(out/'Palette.png');im.file_format='PNG';im.save()
mat=bpy.data.materials.new('RampPalette');snow=bpy.data.materials.new('RampSnow');made=[];rows=[]
for original,group in [(visual,'Body'),(visual,'SnowCap')]+[(o,'CollisionWedge' if 'Ramp |' in o.name else 'CollisionStop') for o in bpy.data.objects if o.type=='MESH' and o.name.startswith('COL_')]:
 ev=original.evaluated_get(deps);mesh=bpy.data.meshes.new_from_object(ev,depsgraph=deps);slots=list(mesh.materials);mesh.transform(Matrix.Translation((0,-8,0))@ev.matrix_world);bm=bmesh.new();bm.from_mesh(mesh)
 if group in ['Body','SnowCap']:
  rm=[f for f in bm.faces if (slots[f.material_index].name=='Snow on outer footings')!=(group=='SnowCap')];bmesh.ops.delete(bm,geom=rm,context='FACES')
 deg=[f for f in bm.faces if f.calc_area()<1e-10];removed=len(deg);bmesh.ops.delete(bm,geom=deg,context='FACES');bmesh.ops.delete(bm,geom=[v for v in bm.verts if not v.link_faces],context='VERTS')
 bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));left=set(bm.faces);flipped=0
 while left:
  a=left.pop();faces={a};queue=[a]
  while queue:
   for e in queue.pop().edges:
    for f in e.link_faces:
     if f in left:left.remove(f);faces.add(f);queue.append(f)
  if all(len(e.link_faces)==2 for f in faces for e in f.edges):
   vol=sum(f.verts[0].co.dot(f.verts[i].co.cross(f.verts[i+1].co))/6 for f in faces for i in range(1,len(f.verts)-1))
   if vol<0:bmesh.ops.reverse_faces(bm,faces=list(faces));flipped+=1
 bm.to_mesh(mesh);bm.free()
 while mesh.uv_layers:mesh.uv_layers.remove(mesh.uv_layers[0])
 uv=mesh.uv_layers.new(name='PaletteUV')
 for p in mesh.polygons:
  idx=mats.index(slots[p.material_index]) if slots[p.material_index] in mats else 0
  for loop in p.loop_indices:uv.data[loop].uv=((idx+.5)/16,.5)
  p.material_index=0
 mesh.materials.clear();mesh.materials.append(snow if group=='SnowCap' else mat);mesh.calc_loop_triangles();obj=bpy.data.objects.new(group,mesh);bpy.context.scene.collection.objects.link(obj);made.append(obj);rows.append(dict(group=group,triangles=len(mesh.loop_triangles),zeroAreaRemoved=removed,flippedComponents=flipped,bounds=[[min(v.co[i] for v in mesh.vertices) for i in range(3)],[max(v.co[i] for v in mesh.vertices) for i in range(3)]]))
for name,loc in [('BasisX',(1,0,0)),('BasisY',(0,1,0)),('BasisZ',(0,0,1))]:
 o=bpy.data.objects.new(name,None);bpy.context.scene.collection.objects.link(o);o.location=loc;made.append(o)
bpy.ops.object.select_all(action='DESELECT')
for o in made:o.select_set(True)
bpy.context.view_layer.objects.active=made[0]
bpy.ops.export_scene.fbx(filepath=str(out/'TractionRamp.fbx'),use_selection=True,object_types={'MESH','EMPTY'},axis_forward='-Z',axis_up='Y',apply_unit_scale=True,apply_scale_options='FBX_SCALE_UNITS',bake_space_transform=True,mesh_smooth_type='FACE',add_leaf_bones=False,bake_anim=False)
report=dict(source=str(src.relative_to(root)),sha256=hashlib.sha256(src.read_bytes()).hexdigest(),notes='Metres; source +Y uphill, +Z up. Geometry recentred at source Y=8. Preserved supplied smooth wedge/rear stop collision. Snow separately removable; no ledger volume. Source unchanged; studio excluded, embedded text not executed.',groups=rows,collisionProfile=dict(inclineRun=14,crestLength=2,rise=3.2),palette=[dict(name=m.name,color=list(m.diffuse_color)) for m in mats]);(out/'ExportMetrics.json').write_text(json.dumps(report,indent=2));print(json.dumps(report,indent=2))
