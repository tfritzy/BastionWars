import bpy
import os

# Define the directory containing the .blend files
source_directory = "C:/development/BastionWars/godot/Rendering/Models/Tiles/blend"
# Define the directory where the .obj files will be saved
destination_directory = "C:/development/BastionWars/godot/Rendering/Models/Tiles/glb"

# Make sure the destination directory exists
if not os.path.exists(destination_directory):
    os.makedirs(destination_directory)

# Iterate through all files in the source directory
for filename in os.listdir(source_directory):
    if filename.endswith(".blend"):
        # Construct the full file path
        blend_file_path = os.path.join(source_directory, filename)

        # Load the .blend file
        bpy.ops.wm.open_mainfile(filepath=blend_file_path)

        # Define the output path for the .obj file
        obj_file_path = os.path.join(
            destination_directory, os.path.splitext(filename)[0] + ".obj"
        )

        # Export the .blend file as .obj
        bpy.ops.export_scene.gltf(
            filepath=destination_directory,
            export_format="GLB",  # Export format
            export_materials="NONE",  # Do not export materials
            export_apply=True,  # Apply transformations
        )


print("Export completed.")
