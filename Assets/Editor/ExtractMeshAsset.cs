using UnityEngine;
using UnityEditor;
using System.IO;

public class ExtractMeshAsset
{
    [MenuItem("Assets/Extract Selected Mesh To Asset", true)]
    static bool ValidateExtractMesh()
    {
        return Selection.activeObject is Mesh;
    }

    [MenuItem("Assets/Extract Selected Mesh To Asset")]
    static void ExtractSelectedMesh()
    {
        Mesh sourceMesh = Selection.activeObject as Mesh;
        if (sourceMesh == null)
        {
            Debug.LogError("Select a Mesh asset first.");
            return;
        }

        string originalPath = AssetDatabase.GetAssetPath(sourceMesh);
        string folder = Path.GetDirectoryName(originalPath);
        string newPath = AssetDatabase.GenerateUniqueAssetPath(
            Path.Combine(folder, sourceMesh.name + "_copy.asset")
        );

        Mesh newMesh = Object.Instantiate(sourceMesh);
        AssetDatabase.CreateAsset(newMesh, newPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Created mesh asset at: " + newPath);
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = newMesh;
    }
}
