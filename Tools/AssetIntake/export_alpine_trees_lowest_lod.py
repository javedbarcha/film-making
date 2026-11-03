"""Export only supplied LOD3 copies; preserve original mounting basis and source files."""
import bpy,bmesh,json
from pathlib import Path
from mathutils import Vector,Matrix
root=Path(__file__).resolve().parents[2]
bpy.ops.wm.open_mainfile(filepath=str(root/'SourceAssets/AlpineTreesLowestLOD/Alpine_Trees_26_Variants_4_LODs.blend'))
out=root/'Assets/Snowglow/Catalog/Sources/AlpineTreesLowestLOD';out.mkdir(parents=True,exist_ok=True)
collections=sorted([c for c in bpy.data.collections if c.name.endswith(' | LOD3') and c.name[:2].isdigit()],key=lambda c:c.name)
assert len(collections)==26
old=json.loads((root/'Assets/Snowglow/Catalog/Sources/AlpineTrees/ExportMetrics.json').read_text())
names=[p['name'] for p in old['palette']]
for p in old['palette']:
 assert max(abs(a-b) for a,b in zip(bpy.data.materials[p['name']].diffuse_color,p['color']))<1e-5
body=bpy.data.materials.new('AlpineTreePalette');snow=bpy.data.materials.new('AlpineTreeSnow')
deps=bpy.context.evaluated_depsgraph_get();rows=[]
for coll in collections:
 base=bpy.data.collections[coll.name.removesuffix(' | LOD3')]
 base_meshes=[o for o in base.all_objects if o.type=='MESH']
 floor=min((o.matrix_world@v.co).z for o in base_meshes for v in o.data.vertices)
 anchor=next(o for o in base.all_objects if o.type=='EMPTY').matrix_world.translation
 origin=Vector((anchor.x,anchor.y,floor));made=[];groups={};sources=[o for o in coll.all_objects if o.type=='MESH']
 assert len(sources) in (1,2)
 for source in sources:
  mesh=bpy.data.meshes.new_from_object(source.evaluated_get(deps),depsgraph=deps);mesh.transform(Matrix.Translation(-origin)@source.matrix_world)
  bm=bmesh.new();bm.from_mesh(mesh);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(mesh);bm.free()
  key='SnowCap' if ' | Snow | LOD3' in source.name else 'TreeWood';slots=[m.name for m in mesh.materials]
  while mesh.uv_layers:mesh.uv_layers.remove(mesh.uv_layers[0])
  uv=mesh.uv_layers.new(name='PaletteUV')
  for face in mesh.polygons:
   index=names.index(slots[face.material_index])
   for loop in face.loop_indices:uv.data[loop].uv=((index+.5)/16,.5)
   face.material_index=0
  mesh.materials.clear();mesh.materials.append(snow if key=='SnowCap' else body);mesh.calc_loop_triangles()
  degenerate=sum((mesh.vertices[t.vertices[1]].co-mesh.vertices[t.vertices[0]].co).cross(mesh.vertices[t.vertices[2]].co-mesh.vertices[t.vertices[0]].co).length<1e-10 for t in mesh.loop_triangles)
  groups[key]={'vertices':len(mesh.vertices),'triangles':len(mesh.loop_triangles),'degenerateTriangles':degenerate}
  obj=bpy.data.objects.new(key,mesh);bpy.context.scene.collection.objects.link(obj);made.append(obj)
 for frame_name,position in [('BasisX',(1,0,0)),('BasisY',(0,1,0)),('BasisZ',(0,0,1))]:
  frame=bpy.data.objects.new(frame_name,None);bpy.context.scene.collection.objects.link(frame);frame.location=position;made.append(frame)
 bpy.ops.object.select_all(action='DESELECT')
 for obj in made:obj.select_set(True)
 slug=coll.name[:2]+'_LOD3'
 bpy.ops.export_scene.fbx(filepath=str(out/(slug+'.fbx')),use_selection=True,object_types={'MESH','EMPTY'},axis_forward='-Z',axis_up='Y',apply_unit_scale=True,apply_scale_options='FBX_SCALE_UNITS',bake_space_transform=True,mesh_smooth_type='FACE',add_leaf_bones=False,bake_anim=False)
 points=[v.co for o in made if o.type=='MESH' for v in o.data.vertices];low=[min(p[i] for p in points) for i in range(3)];high=[max(p[i] for p in points) for i in range(3)]
 rows.append({'index':coll.name[:2],'name':base.name[3:],'model':slug,'lod':3,'triangles':sum(g['triangles']-g['degenerateTriangles'] for g in groups.values()),'groups':groups,'size':[high[0]-low[0],high[2]-low[2],high[1]-low[1]],'minimum':[low[0],low[2],-high[1]],'sourceCollection':coll.name,'included':[o.name for o in sources],'origin':list(origin)})
 for obj in made:bpy.data.objects.remove(obj,do_unlink=True)
(out/'ExportMetrics.json').write_text(json.dumps({'trees':rows,'lod':3,'paletteReference':'Assets/Snowglow/Catalog/Sources/AlpineTrees/Palette.png','excludedLODLevels':[0,1,2],'excludedPresentation':[o.name for o in bpy.data.collections['00 Presentation | Hide for Export'].all_objects]},indent=2))
print('LOWEST_LOD_EXPORT_COMPLETE',[(r['index'],r['triangles'],round(r['minimum'][1],3)) for r in rows],flush=True)
