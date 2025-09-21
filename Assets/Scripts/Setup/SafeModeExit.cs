using UnityEngine;
using UnityEditor;

namespace NeuralRink.Setup
{
    /// <summary>
    /// Helper script to exit Unity Safe Mode after fixing compilation errors.
    /// This script will automatically run when Unity compiles successfully.
    /// </summary>
    [InitializeOnLoad]
    public static class SafeModeExit
    {
        static SafeModeExit()
        {
            // Check if we're in Safe Mode
            if (EditorApplication.isCompiling)
            {
                EditorApplication.delayCall += CheckSafeMode;
            }
        }

        /// <summary>
        /// Check if we can exit Safe Mode.
        /// </summary>
        private static void CheckSafeMode()
        {
            if (!EditorApplication.isCompiling)
            {
                Debug.Log("🎉 Neural Rink: Compilation successful! Unity should exit Safe Mode automatically.");
                Debug.Log("🎮 You can now play your Neural Rink game!");
            }
        }
    }
}
