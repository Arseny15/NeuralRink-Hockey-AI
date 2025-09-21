#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NeuralRink.Setup
{
    public static class UnityProjectSetup
    {
        // Assign in Inspector or via code if you want to open a default scene
        public static SceneAsset PlaySceneAsset;
        public static SceneAsset TrainingSceneAsset;

        [MenuItem("NeuralRink/Setup/Open Play Scene")]
        public static void OpenPlay()
        {
            OpenSceneAsset(PlaySceneAsset);
        }

        [MenuItem("NeuralRink/Setup/Open Training Scene")]
        public static void OpenTraining()
        {
            OpenSceneAsset(TrainingSceneAsset);
        }

        static void OpenSceneAsset(SceneAsset asset)
        {
            if (asset == null) { EditorUtility.DisplayDialog("NeuralRink", "SceneAsset not assigned.", "OK"); return; }
            string path = AssetDatabase.GetAssetPath(asset);
            if (string.IsNullOrEmpty(path)) { EditorUtility.DisplayDialog("NeuralRink", "Invalid scene asset path.", "OK"); return; }
            EditorSceneManager.OpenScene(path);
        }
    }
}
#else
namespace NeuralRink.Setup
{
    // Runtime stub so player builds compile
    public static class UnityProjectSetup { }
}
#endif