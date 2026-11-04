"""Read-only source intake; excludes studio ground, converts FONT to mesh, never executes embedded text."""
import bpy,bmesh,json
from pathlib import Path
root=Path(__file__).resolve().parents[2]
bpy.ops.wm.open_mainfile(filepath=str(root/'SourceAssets/FourBaySnowGarage/Low_Poly_Four_Bay_Snow_Garage.blend'))
out=root/'Assets/Snowglow/Catalog/Sources/FourBaySnowGarage';out.mkdir(parents=True,exist_ok=True)
objects=[o for o in bpy.context.scene.objects if o.type in {'MESH','FONT'} and o.name!='Preview ground']
mats=sorted({m for o in objects for m in o.data.materials},key=lambda m:m.name)
def srgb(v):return 12.92*v if v<=.0031308 else 1.055*v**(1/2.4)-.055
im=bpy.data.images.new('Garage palette',16,1,alpha=False);im.colorspace_settings.name='Non-Color';im.pixels=[v for i in range(16) for v in ([srgb(c) for c in mats[i].diffuse_color[:3]]+[1] if i<len(mats) else [1,1,1,1])];im.filepath_raw=str(out/'Palette.png');im.file_format='PNG';im.save()
mat=bpy.data.materials.new('GaragePalette');snow=bpy.data.materials.new('GarageSnow');deps=bpy.context.evaluated_depsgraph_get();made=[];rows=[]
for original in objects:
 for is_snow in ([False,True] if original.type=='MESH' else [False]):
  mesh=bpy.data.meshes.new_from_object(original.evaluated_get(deps),depsgraph=deps);slots=list(mesh.materials)
  bm=bmesh.new();bm.from_mesh(mesh);remove=[f for f in bm.faces if slots[f.material_index].name.startswith('Snow ')!=is_snow];bmesh.ops.delete(bm,geom=remove,context='FACES');bmesh.ops.delete(bm,geom=[v for v in bm.verts if not v.link_faces],context='VERTS');bm.to_mesh(mesh);bm.free()
  if not mesh.polygons:continue
  while mesh.uv_layers:mesh.uv_layers.remove(mesh.uv_layers[0])
  uv=mesh.uv_layers.new(name='PaletteUV')
  for f in mesh.polygons:
   ix=mats.index(slots[f.material_index])
   for loop in f.loop_indices:uv.data[loop].uv=((ix+.5)/16,.5)
   f.material_index=0
  mesh.materials.clear();mesh.materials.append(snow if is_snow else mat)
  obj=bpy.data.objects.new(('SnowCap | ' if is_snow else '')+original.name,mesh);bpy.context.scene.collection.objects.link(obj);obj.matrix_world=original.matrix_world.copy();made.append(obj);mesh.calc_loop_triangles();rows.append(dict(name=obj.name,triangles=len(mesh.loop_triangles)))
for name,pos in [('BasisX',(1,0,0)),('BasisY',(0,1,0)),('BasisZ',(0,0,1))]:
 o=bpy.data.objects.new(name,None);bpy.context.scene.collection.objects.link(o);o.location=pos;made.append(o)
bpy.ops.object.select_all(action='DESELECT')
for o in made:o.select_set(True)
bpy.ops.export_scene.fbx(filepath=str(out/'FourBayGarage.fbx'),use_selection=True,object_types={'MESH','EMPTY'},axis_forward='-Z',axis_up='Y',apply_unit_scale=True,apply_scale_options='FBX_SCALE_UNITS',bake_space_transform=True,mesh_smooth_type='FACE',add_leaf_bones=False,bake_anim=False)
(out/'ExportMetrics.json').write_text(json.dumps(dict(parts=rows,triangles=sum(r['triangles'] for r in rows),excluded=['Preview ground','cameras','lights','embedded text (not executed)'],palette=[dict(name=m.name,color=list(m.diffuse_color)) for m in mats]),indent=2))
print('GARAGE_EXPORT',sum(r['triangles'] for r in rows),flush=True)
