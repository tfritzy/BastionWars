import bpy
import os
import sys


def export_to_gltf(blend_file_path, output_folder):
    # Open the .blend file
    bpy.ops.wm.open_mainfile(filepath=blend_file_path)

    # Ensure the context is correctly set
    if not bpy.context.view_layer.objects.active:
        for obj in bpy.context.view_layer.objects:
            if obj.type == "MESH":
                bpy.context.view_layer.objects.active = obj
                break

    # Set the output file path
    output_file_path = os.path.join(
        output_folder, os.path.splitext(os.path.basename(blend_file_path))[0] + ".gltf"
    )

    # Export to glTF
    bpy.ops.export_scene.gltf(filepath=output_file_path, export_format="GLTF_SEPARATE")


def main(folder_path, output_folder):
    if not os.path.exists(output_folder):
        os.makedirs(output_folder)

    # List all .blend files in the folder
    blend_files = [f for f in os.listdir(folder_path) if f.endswith(".blend")]

    for blend_file in blend_files:
        blend_file_path = os.path.join(folder_path, blend_file)
        export_to_gltf(blend_file_path, output_folder)
        print(f"Exported {blend_file} to glTF")


if __name__ == "__main__":
    folder_path = "C:/development/BastionWars/godot/Rendering/Models/Tiles/blend"
    output_folder = "C:/development/BastionWars/godot/Rendering/Models/Tiles/glb"
    main(folder_path, output_folder)
