#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// Automatically sets any texture inside Assets/AssetFigma to Sprite (UI) type on import.
/// Also provides a manual menu to fix existing imports.
/// </summary>
public class FigmaTextureImporter : AssetPostprocessor
{
    const string FIGMA_FOLDER = "Assets/AssetFigma";

    void OnPreprocessTexture()
    {
        if (!assetPath.StartsWith(FIGMA_FOLDER)) return;

        TextureImporter ti = (TextureImporter)assetImporter;
        if (ti.textureType == TextureImporterType.Sprite) return; // already correct

        ti.textureType              = TextureImporterType.Sprite;
        ti.spriteImportMode         = SpriteImportMode.Single;
        ti.alphaIsTransparency      = true;
        ti.mipmapEnabled            = false;
        ti.filterMode               = FilterMode.Bilinear;
        ti.maxTextureSize           = 2048;
        ti.textureCompression       = TextureImporterCompression.CompressedHQ;

        // Good defaults for UI sprites
        ti.spritePivot              = new Vector2(0.5f, 0.5f);
        ti.spritePixelsPerUnit      = 100f;
    }

    [MenuItem("PasarAjaib/Fix Figma Texture Imports")]
    public static void FixAllFigmaTextures()
    {
        string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { FIGMA_FOLDER });
        int count = 0;
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            if (ti == null) continue;

            bool changed = false;
            if (ti.textureType != TextureImporterType.Sprite)
            { ti.textureType = TextureImporterType.Sprite; changed = true; }
            if (ti.spriteImportMode != SpriteImportMode.Single)
            { ti.spriteImportMode = SpriteImportMode.Single; changed = true; }
            if (!ti.alphaIsTransparency)
            { ti.alphaIsTransparency = true; changed = true; }
            if (ti.mipmapEnabled)
            { ti.mipmapEnabled = false; changed = true; }

            if (changed)
            {
                ti.SaveAndReimport();
                count++;
            }
        }
        Debug.Log($"[PasarAjaib] Fixed {count} texture(s) in {FIGMA_FOLDER}");
        AssetDatabase.Refresh();
    }
}
#endif
