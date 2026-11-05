"""Export source snowmen without executing embedded text or saving the blend."""
import bpy,bmesh,json
from pathlib import Path
from mathutils import Matrix,Vector
root=Path(__file__).resolve().parents[2]
bpy.ops.wm.open_mainfile(filepath=str(root/'SourceAssets/ThreeSnowmen/Three_Low_Poly_Snowmen_1000_Triangles.blend'))
out=root/'Assets/Snowglow/Catalog/Sources/ThreeSnowmen';out.mkdir(parents=True,exist_ok=True)
objects=sorted([o for o in bpy.context.scene.objects if o.type=='MESH' and o.name[:2] in {'01','02','03'}],key=lambda o:o.name)
assert len(objects)==3
mats=sorted({m for o in objects for m in o.data.materials},key=lambda m:m.name);width=32
def srgb(v):return 12.92*v if v<=.0031308 else 1.055*v**(1/2.4)-.055
im=bpy.data.images.new('Snowmen palette',width,1,alpha=False);im.colorspace_settings.name='Non-Color';im.pixels=[v for i in range(width) for v in ([srgb(c) for c in mats[i].diffuse_color[:3]]+[1] if i<len(mats) else [1,1,1,1])];im.filepath_raw=str(out/'Palette.png');im.file_format='PNG';im.save()
material=bpy.data.materials.new('SnowmanPalette');snow=bpy.data.materials.new('SnowmanSnow');deps=bpy.context.evaluated_depsgraph_get();rows=[]
for original in objects:
 prefix=original.name[:2];anchor=next(o for o in bpy.context.scene.objects if o.type=='EMPTY' and o.name.startswith(prefix));origin=anchor.matrix_world.translation.copy();made=[];parts=[];removed=0
 for is_snow in (False,True):
  mesh=bpy.data.meshes.new_from_object(original.evaluated_get(deps),depsgraph=deps);mesh.transform(Matrix.Translation(-origin)@original.matrix_world);slots=list(mesh.materials)
  bm=bmesh.new();bm.from_mesh(mesh);bmesh.ops.delete(bm,geom=[f for f in bm.faces if (slots[f.material_index].name=='Snow ivory white')!=is_snow],context='FACES');bmesh.ops.triangulate(bm,faces=list(bm.faces));bad=[f for f in bm.faces if f.calc_area()<1e-12];removed+=len(bad);bmesh.ops.delete(bm,geom=bad,context='FACES');bmesh.ops.delete(bm,geom=[v for v in bm.verts if not v.link_faces],context='VERTS');bm.to_mesh(mesh);bm.free()
  while mesh.uv_layers:mesh.uv_layers.remove(mesh.uv_layers[0])
  uv=mesh.uv_layers.new(name='PaletteUV')
  for f in mesh.polygons:
   ix=mats.index(slots[f.material_index])
   for loop in f.loop_indices:uv.data[loop].uv=((ix+.5)/width,.5)
   f.material_index=0
  mesh.materials.clear();mesh.materials.append(snow if is_snow else material);o=bpy.data.objects.new('SnowBody' if is_snow else 'ClothingAndDetails',mesh);bpy.context.scene.collection.objects.link(o);made.append(o);mesh.calc_loop_triangles();parts.append(dict(name=o.name,triangles=len(mesh.loop_triangles)))
 for name,pos in [('BasisX',(1,0,0)),('BasisY',(0,1,0)),('BasisZ',(0,0,1))]:
  o=bpy.data.objects.new(name,None);bpy.context.scene.collection.objects.link(o);o.location=pos;made.append(o)
 bpy.ops.object.select_all(action='DESELECT')
 for o in made:o.select_set(True)
 bpy.ops.export_scene.fbx(filepath=str(out/('Snowman'+prefix+'.fbx')),use_selection=True,object_types={'MESH','EMPTY'},axis_forward='-Z',axis_up='Y',apply_unit_scale=True,apply_scale_options='FBX_SCALE_UNITS',bake_space_transform=True,mesh_smooth_type='FACE',add_leaf_bones=False,bake_anim=False)
 rows.append(dict(index=prefix,name=original.name,parts=parts,triangles=sum(p['triangles'] for p in parts),removed_zero_area=removed,origin=list(origin)))
 for o in made:bpy.data.objects.remove(o,do_unlink=True)
(out/'ExportMetrics.json').write_text(json.dumps(dict(snowmen=rows,palette=[dict(name=m.name,color=list(m.diffuse_color)) for m in mats],excluded=['Plane','cameras','lights','embedded text (never executed)']),indent=2));print('SNOWMEN_EXPORT',rows,flush=True)
