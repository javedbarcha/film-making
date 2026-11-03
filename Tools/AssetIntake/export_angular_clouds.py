"""Export flat-shaded cloud copies with one shared opaque color palette."""
import bpy, bmesh, json
from pathlib import Path
from mathutils import Vector, Matrix
root=Path(__file__).resolve().parents[2]
bpy.ops.wm.open_mainfile(filepath=str(root/'SourceAssets/AngularClouds/Eight_Angular_Low_Poly_Clouds.blend'))
out=root/'Assets/Snowglow/Catalog/Sources/AngularClouds';out.mkdir(parents=True,exist_ok=True)
sources=sorted([o for o in bpy.context.scene.objects if o.type=='MESH'],key=lambda o:o.name)
assert len(sources)==8
names=['Cloud White','Cloud Pale Ice','Cloud Soft Underside']
colors=[list(bpy.data.materials[n].diffuse_color) for n in names]
def srgb(v):return 12.92*v if v<=.0031308 else 1.055*v**(1/2.4)-.055
palette=bpy.data.images.new('CloudPalette',16,1,alpha=False);palette.colorspace_settings.name='Non-Color'
palette.pixels=[v for i in range(16) for v in ([srgb(c) for c in colors[min(i,2)][:3]]+[1])]
palette.filepath_raw=str(out/'Palette.png');palette.file_format='PNG';palette.save()
material=bpy.data.materials.new('AngularCloudPalette');rows=[]
for source in sources:
    mesh=source.data.copy();points=[source.matrix_world@v.co for v in mesh.vertices]
    low=Vector([min(p[i] for p in points) for i in range(3)]);high=Vector([max(p[i] for p in points) for i in range(3)])
    origin=Vector(((low.x+high.x)*.5,(low.y+high.y)*.5,low.z))
    mesh.transform(Matrix.Translation(-origin)@source.matrix_world)
    bm=bmesh.new();bm.from_mesh(mesh);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(mesh);bm.free()
    slots=[m.name for m in mesh.materials]
    while mesh.uv_layers:mesh.uv_layers.remove(mesh.uv_layers[0])
    uv=mesh.uv_layers.new(name='CloudPaletteUV')
    for face in mesh.polygons:
        index=names.index(slots[face.material_index])
        for loop in face.loop_indices:uv.data[loop].uv=((index+.5)/16,.5)
        face.material_index=0;face.use_smooth=False
    mesh.materials.clear();mesh.materials.append(material);mesh.calc_loop_triangles()
    assert len(mesh.loop_triangles)==332
    obj=bpy.data.objects.new('Cloud',mesh);bpy.context.scene.collection.objects.link(obj)
    bpy.ops.object.select_all(action='DESELECT');obj.select_set(True);bpy.context.view_layer.objects.active=obj
    slug=source.name
    bpy.ops.export_scene.fbx(filepath=str(out/(slug+'.fbx')),use_selection=True,object_types={'MESH'},axis_forward='-Z',axis_up='Y',apply_unit_scale=True,apply_scale_options='FBX_SCALE_UNITS',bake_space_transform=True,mesh_smooth_type='FACE',add_leaf_bones=False,bake_anim=False)
    rows.append({'index':source.name[:2],'name':source.name[3:].replace('_',' '),'model':slug,'triangles':len(mesh.loop_triangles),'size':[high.x-low.x,high.z-low.z,high.y-low.y],'included':[source.name]})
    bpy.data.objects.remove(obj,do_unlink=True)
(out/'ExportMetrics.json').write_text(json.dumps({'clouds':rows,'palette':[{'name':n,'linearColor':c} for n,c in zip(names,colors)],'excluded':'No presentation objects, cameras, lights or embedded texts in supplied file.'},indent=2))
print('CLOUD_EXPORT_COMPLETE',json.dumps(rows),flush=True)
