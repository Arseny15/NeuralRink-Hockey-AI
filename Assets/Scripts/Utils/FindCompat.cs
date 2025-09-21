using UnityEngine;

namespace NeuralRink.Utils
{
    /// <summary>
    /// Compatibility wrapper for Unity's Find APIs.
    /// Provides modern FindFirstObjectByType and FindObjectsByType methods.
    /// </summary>
    public static class FindCompat
    {
        /// <summary>
        /// Find the first object of type T in the scene.
        /// </summary>
        public static T First<T>() where T : Object
            => Object.FindFirstObjectByType<T>();

        /// <summary>
        /// Find all objects of type T in the scene.
        /// </summary>
        public static T[] Many<T>() where T : Object
            => Object.FindObjectsByType<T>(FindObjectsSortMode.None);
    }
}
