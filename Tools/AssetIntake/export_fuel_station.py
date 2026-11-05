"""Read-only source intake; excludes studio ground, converts FONT to mesh, never executes embedded text."""
import bpy,bmesh,json
from pathlib import Path
root=Path(__file__).resolve().parents[2]
bpy.ops.wm.open_mainfile(filepath=str(root/'SourceAssets/SnowMachineryFuelStation/Low_Poly_Snow_Machinery_Fuel_Station.blend'))
out=root/'Assets/Snowglow/Catalog/Sources/SnowMachineryFuelStation';out.mkdir(parents=True,exist_ok=True)
objects=[o for o in bpy.context.scene.objects if o.type in {'MESH','FONT'} and o.name!='Plane']
mats=sorted({m for o in objects for m in o.data.materials},key=lambda m:m.name)
def srgb(v):return 12.92*v if v<=.0031308 else 1.055*v**(1/2.4)-.055
im=bpy.data.images.new('Fuel station palette',16,1,alpha=False);im.colorspace_settings.name='Non-Color';im.pixels=[v for i in range(16) for v in ([srgb(c) for c in mats[i].diffuse_color[:3]]+[1] if i<len(mats) else [1,1,1,1])];im.filepath_raw=str(out/'Palette.png');im.file_format='PNG';im.save()
mat=bpy.data.materials.new('FuelPalette');snow=bpy.data.materials.new('FuelSnow');deps=bpy.context.evaluated_depsgraph_get();made=[];rows=[]
for original in objects:
 for is_snow in ([False,True] if original.type=='MESH' else [False]):
  mesh=bpy.data.meshes.new_from_object(original.evaluated_get(deps),depsgraph=deps);slots=list(mesh.materials)
  bm=bmesh.new();bm.from_mesh(mesh);remove=[f for f in bm.faces if (slots[f.material_index].name in {'Snow white'})!=is_snow];bmesh.ops.delete(bm,geom=remove,context='FACES');bmesh.ops.delete(bm,geom=[v for v in bm.verts if not v.link_faces],context='VERTS');bm.to_mesh(mesh);bm.free()
  if not mesh.polygons:continue
  while mesh.uv_layers:mesh.uv_layers.remove(mesh.uv_layers[0])
  uv=mesh.uv_layers.new(name='PaletteUV')
  for f in mesh.polygons:
   ix=mats.index(slots[f.material_index])
   for loop in f.loop_indices:uv.data[loop].uv=((ix+.5)/16,.5)
   f.material_index=0
  mesh.materials.clear();mesh.materials.append(snow if is_snow else mat)
  obj=bpy.data.objects.new(('SnowCap | ' if is_snow else '')+original.name,mesh);bpy.context.scene.collection.objects.link(obj);obj.matrix_world=original.matrix_world.copy();made.append(obj);mesh.calc_loop_triangles();rows.append(dict(name=obj.name,triangles=len(mesh.loop_triangles)))
# Combine static intake copies into three editable material/lighting groups.
groups={group:[o for o in made if ('SnowCap' if o.name.startswith('SnowCap') else 'Luminous' if '| lens' in o.name else 'Body')==group] for group in ('Body','Luminous','SnowCap')}
joined=[];rows=[]
for group in ('Body','Luminous','SnowCap'):
 parts=groups[group]
 if not parts:continue
 bpy.ops.object.select_all(action='DESELECT')
 for o in parts:o.select_set(True)
 bpy.context.view_layer.objects.active=parts[0];bpy.ops.object.join();o=parts[0];o.name=group;o.data.calc_loop_triangles();joined.append(o);rows.append(dict(name=group,triangles=len(o.data.loop_triangles)))
made=joined
removed=0
for o in made:
 bm=bmesh.new();bm.from_mesh(o.data);bmesh.ops.triangulate(bm,faces=list(bm.faces));bad=[f for f in bm.faces if f.calc_area()<1e-12];removed+=len(bad);bmesh.ops.delete(bm,geom=bad,context='FACES');bmesh.ops.delete(bm,geom=[v for v in bm.verts if not v.link_faces],context='VERTS');bm.to_mesh(o.data);bm.free();o.data.calc_loop_triangles()
rows=[dict(name=o.name,triangles=len(o.data.loop_triangles)) for o in made]
print('DEGENERATE_TRIANGLES_REMOVED',removed,flush=True)
for name,pos in [('BasisX',(1,0,0)),('BasisY',(0,1,0)),('BasisZ',(0,0,1))]:
 o=bpy.data.objects.new(name,None);bpy.context.scene.collection.objects.link(o);o.location=pos;made.append(o)
bpy.ops.object.select_all(action='DESELECT')
for o in made:o.select_set(True)
bpy.ops.export_scene.fbx(filepath=str(out/'FuelStation.fbx'),use_selection=True,object_types={'MESH','EMPTY'},axis_forward='-Z',axis_up='Y',apply_unit_scale=True,apply_scale_options='FBX_SCALE_UNITS',bake_space_transform=True,mesh_smooth_type='FACE',add_leaf_bones=False,bake_anim=False)
(out/'ExportMetrics.json').write_text(json.dumps(dict(source_triangles=3088,removed_zero_area_triangles=removed,parts=rows,triangles=sum(r['triangles'] for r in rows),excluded=['Plane (studio ground)','cameras','lights','embedded text (not executed)'],palette=[dict(name=m.name,color=list(m.diffuse_color)) for m in mats]),indent=2))
print('FUEL_EXPORT',sum(r['triangles'] for r in rows),flush=True)
