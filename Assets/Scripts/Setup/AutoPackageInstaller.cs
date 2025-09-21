using UnityEngine;
using UnityEditor;

namespace NeuralRink.Setup
{
    /// <summary>
    /// Automatically installs required Unity packages when the project opens.
    /// This helps resolve compilation errors for missing dependencies.
    /// </summary>
    [InitializeOnLoad]
    public static class AutoPackageInstaller
    {
        static AutoPackageInstaller()
        {
            EditorApplication.delayCall += CheckAndInstallPackages;
        }

        /// <summary>
        /// Check for and install required packages.
        /// </summary>
        private static void CheckAndInstallPackages()
        {
            Debug.Log("🔧 Neural Rink: Checking for required packages...");

            // Check if Input System is installed
            if (!IsPackageInstalled("com.unity.inputsystem"))
            {
                Debug.Log("📦 Installing Input System...");
                InstallPackage("com.unity.inputsystem");
            }
            else
            {
                Debug.Log("✅ Input System already installed");
            }

            // Check if TextMeshPro is installed
            if (!IsPackageInstalled("com.unity.textmeshpro"))
            {
                Debug.Log("📦 Installing TextMeshPro...");
                InstallPackage("com.unity.textmeshpro");
            }
            else
            {
                Debug.Log("✅ TextMeshPro already installed");
            }

            Debug.Log("🎯 Neural Rink: Package check complete!");
        }

        /// <summary>
        /// Check if a package is installed.
        /// </summary>
        private static bool IsPackageInstalled(string packageId)
        {
            var packages = UnityEditor.PackageManager.Client.List();
            foreach (var package in packages.Result)
            {
                if (package.name == packageId)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Install a Unity package.
        /// </summary>
        private static void InstallPackage(string packageId)
        {
            UnityEditor.PackageManager.Client.Add(packageId);
        }
    }
}
