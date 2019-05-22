

using UnityEngine;
using UnityEditor;  // Most of the utilities we are going to use are contained in the UnityEditor namespace

// We inherit from the AssetPostProcessor class which contains all the exposed variables and event triggers for asset importing pipeline
internal sealed class CustomAssetImporter : AssetPostprocessor {
  // Couple of constants used below to be able to change from a single point, you may use direct literals instead of these consts to if you please
  private const int webTextureSize = 2048;
  private const int standaloneTextureSize = 4096;
  private const int iosTextureSize = 1024;
  private const int androidTextureSize = 1024;


  //-------------Pre Processors

  // This event is raised when a texture asset is imported
  private void OnPreprocessTexture() {
    // Get the reference to the assetImporter (From the AssetPostProcessor class) and unbox it to a TextureImporter (Which is inherited and extends the AssetImporter with texture specific utilities)
    var importer = assetImporter as TextureImporter;
    var path = assetPath.Split('/');
    var fileName = path[path.Length - 1];
    fileName = fileName.Split('.')[0];
    if (fileName.Substring(fileName.Length - 2, 2) == "UI") return;

    // Set the texture import type drop-down to advanced so our changes reflect in the import settings inspector
    importer.textureType = TextureImporterType.Sprite;
    importer.filterMode = FilterMode.Point;
    importer.spritePixelsPerUnit = 8;
    importer.isReadable = true;

    importer.textureCompression = TextureImporterCompression.Uncompressed;
    importer.compressionQuality = 100;
    TextureImporterSettings settings = new TextureImporterSettings();
    importer.ReadTextureSettings(settings);
    settings.spriteAlignment = (int)SpriteAlignment.TopLeft;
  }
}
