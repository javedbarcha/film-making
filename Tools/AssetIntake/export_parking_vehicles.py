"""Export static parking scenery; source blend and embedded text remain untouched."""
import bpy,bmesh,json
from pathlib import Path
from mathutils import Matrix
root=Path(__file__).resolve().parents[2]
bpy.ops.wm.open_mainfile(filepath=str(root/'SourceAssets/SnowTownVehicles/Snow_Town_Eight_Low_Poly_Vehicles.blend'))
out=root/'Assets/Snowglow/Catalog/Sources/SnowTownVehicles';out.mkdir(parents=True,exist_ok=True)
anchors=sorted([o for o in bpy.context.scene.objects if o.type=='EMPTY' and 'MOVE VEHICLE' in o.name],key=lambda o:o.name);assert len(anchors)==8
mats=sorted({m for a in anchors for o in a.children_recursive if o.type=='MESH' for m in o.data.materials},key=lambda m:m.name);width=32;assert len(mats)<=width
def srgb(v):return 12.92*v if v<=.0031308 else 1.055*v**(1/2.4)-.055
im=bpy.data.images.new('Parking palette',width,1,alpha=False);im.colorspace_settings.name='Non-Color';im.pixels=[v for i in range(width) for v in ([srgb(c) for c in mats[i].diffuse_color[:3]]+[1] if i<len(mats) else [1,1,1,1])];im.filepath_raw=str(out/'Palette.png');im.file_format='PNG';im.save()
target={g:bpy.data.materials.new('Parking'+g) for g in ('Body','Snow','Glazing')};deps=bpy.context.evaluated_depsgraph_get();rows=[]
for anchor in anchors:
 origin=anchor.matrix_world.translation.copy();source=[o for o in anchor.children_recursive if o.type=='MESH'];prepared=[];source_tris=0;removed=0;front=[]
 for original in source:
  mesh=bpy.data.meshes.new_from_object(original.evaluated_get(deps),depsgraph=deps);mesh.transform(Matrix.Translation(-origin)@original.matrix_world);mesh.calc_loop_triangles();source_tris+=len(mesh.loop_triangles);slots=list(mesh.materials)
  if '| Body' in original.name:front += [p.center.y for p in mesh.polygons if slots[p.material_index].name=='Warm headlamp']
  bm=bmesh.new();bm.from_mesh(mesh);remaining=set(bm.faces)
  while remaining:
   first=remaining.pop();part={first};pending=[first]
   while pending:
    for edge in pending.pop().edges:
     for f in edge.link_faces:
      if f in remaining:remaining.remove(f);part.add(f);pending.append(f)
   if all(len(e.link_faces)==2 for f in part for e in f.edges):bmesh.ops.recalc_face_normals(bm,faces=list(part))
  bmesh.ops.triangulate(bm,faces=list(bm.faces));bad=[f for f in bm.faces if f.calc_area()<1e-12];removed+=len(bad);bmesh.ops.delete(bm,geom=bad,context='FACES');bm.to_mesh(mesh);bm.free();prepared.append((mesh,slots))
 made=[];parts=[]
 def group(name):return 'Snow' if name=='Fresh snow' else 'Glazing' if name=='Opaque blue glazing' else 'Body'
 for g in target:
  copies=[]
  for original,slots in prepared:
   mesh=original.copy();bm=bmesh.new();bm.from_mesh(mesh);bmesh.ops.delete(bm,geom=[f for f in bm.faces if group(slots[f.material_index].name)!=g],context='FACES');bmesh.ops.delete(bm,geom=[v for v in bm.verts if not v.link_faces],context='VERTS');bm.to_mesh(mesh);bm.free()
   if not mesh.polygons:continue
   while mesh.uv_layers:mesh.uv_layers.remove(mesh.uv_layers[0])
   uv=mesh.uv_layers.new(name='PaletteUV')
   for f in mesh.polygons:
    ix=mats.index(slots[f.material_index])
    for loop in f.loop_indices:uv.data[loop].uv=((ix+.5)/width,.5)
    f.material_index=0
   mesh.materials.clear();mesh.materials.append(target[g]);o=bpy.data.objects.new(g,mesh);bpy.context.scene.collection.objects.link(o);copies.append(o)
  if not copies:continue
  bpy.ops.object.select_all(action='DESELECT')
  for o in copies:o.select_set(True)
  bpy.context.view_layer.objects.active=copies[0]
  if len(copies)>1:bpy.ops.object.join()
  o=copies[0];o.name=g;o.data.calc_loop_triangles();made.append(o);parts.append(dict(name=g,triangles=len(o.data.loop_triangles)))
 for name,pos in [('BasisX',(1,0,0)),('BasisY',(0,1,0)),('BasisZ',(0,0,1))]:
  o=bpy.data.objects.new(name,None);bpy.context.scene.collection.objects.link(o);o.location=pos;made.append(o)
 bpy.ops.object.select_all(action='DESELECT')
 for o in made:o.select_set(True)
 index=anchor.name[:2];bpy.ops.export_scene.fbx(filepath=str(out/('Vehicle'+index+'.fbx')),use_selection=True,object_types={'MESH','EMPTY'},axis_forward='-Z',axis_up='Y',apply_unit_scale=True,apply_scale_options='FBX_SCALE_UNITS',bake_space_transform=True,mesh_smooth_type='FACE',add_leaf_bones=False,bake_anim=False)
 rows.append(dict(index=index,name=anchor.name.split(' |')[0][3:],source_triangles=source_tris,triangles=sum(x['triangles'] for x in parts),removed_zero_area=removed,frontSign=1 if sum(front)>0 else -1,parts=parts))
 for o in made:bpy.data.objects.remove(o,do_unlink=True)
(out/'ExportMetrics.json').write_text(json.dumps(dict(vehicles=rows,palette=[dict(name=m.name,color=list(m.diffuse_color)) for m in mats],excluded=['studio Plane','cameras','lights','embedded scripts (not executed)']),indent=2));print('PARKING_EXPORT',rows,flush=True)
