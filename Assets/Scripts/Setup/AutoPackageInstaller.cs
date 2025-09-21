using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace NeuralRink.Setup
{
    /// <summary>
    /// Automatically installs required Unity packages when the project opens.
    /// This helps resolve compilation errors for missing dependencies.
    /// </summary>
    [InitializeOnLoad]
    public static class AutoPackageInstaller
    {
        private static ListRequest listRequest;
        private static AddRequest addRequest;

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
            
            // Start async package list request
            listRequest = Client.List();
            EditorApplication.update += CheckPackageListProgress;
        }

        /// <summary>
        /// Check the progress of the package list request.
        /// </summary>
        private static void CheckPackageListProgress()
        {
            if (listRequest == null) return;

            if (listRequest.IsCompleted)
            {
                EditorApplication.update -= CheckPackageListProgress;
                
                if (listRequest.Status == StatusCode.Success)
                {
                    ProcessPackageList();
                }
                else
                {
                    Debug.LogError($"Failed to retrieve package list: {listRequest.Error.message}");
                }
                
                listRequest = null;
            }
        }

        /// <summary>
        /// Process the retrieved package list and install missing packages.
        /// </summary>
        private static void ProcessPackageList()
        {
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

            // Check Unity version for TextMeshPro compatibility
            CheckTextMeshProAvailability();

            Debug.Log("🎯 Neural Rink: Package check complete!");
        }

        /// <summary>
        /// Check TextMeshPro availability based on Unity version.
        /// </summary>
        private static void CheckTextMeshProAvailability()
        {
            var unityVersion = UnityEngine.Application.unityVersion;
            Debug.Log($"🔍 Unity Version: {unityVersion}");

            // Unity 6 and later have TextMeshPro built-in
            if (IsUnity6OrLater())
            {
                Debug.Log("✅ TextMeshPro is built-in with Unity 6 - no separate package needed");
                return;
            }

            // For older Unity versions, try to install TextMeshPro package
            if (!IsPackageInstalled("com.unity.textmeshpro"))
            {
                Debug.Log("📦 Installing TextMeshPro package for older Unity version...");
                InstallPackage("com.unity.textmeshpro");
            }
            else
            {
                Debug.Log("✅ TextMeshPro package already installed");
            }
        }

        /// <summary>
        /// Check if the current Unity version is 6 or later.
        /// </summary>
        private static bool IsUnity6OrLater()
        {
            var version = UnityEngine.Application.unityVersion;
            
            // Extract major version number (e.g., "6000.2.5f1" -> "6000")
            var versionParts = version.Split('.');
            if (versionParts.Length > 0 && int.TryParse(versionParts[0], out int majorVersion))
            {
                return majorVersion >= 6000; // Unity 6 uses version 6000.x.x
            }
            
            return false;
        }

        /// <summary>
        /// Check if a package is installed.
        /// </summary>
        private static bool IsPackageInstalled(string packageId)
        {
            if (listRequest?.Result == null) return false;
            
            foreach (var package in listRequest.Result)
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
            addRequest = Client.Add(packageId);
            EditorApplication.update += CheckAddPackageProgress;
        }

        /// <summary>
        /// Check the progress of the package add request.
        /// </summary>
        private static void CheckAddPackageProgress()
        {
            if (addRequest == null) return;

            if (addRequest.IsCompleted)
            {
                EditorApplication.update -= CheckAddPackageProgress;
                
                if (addRequest.Status == StatusCode.Success)
                {
                    Debug.Log($"✅ Successfully installed package: {addRequest.Result.name}");
                }
                else
                {
                    Debug.LogError($"❌ Failed to install package: {addRequest.Error.message}");
                }
                
                addRequest = null;
            }
        }
    }
}
