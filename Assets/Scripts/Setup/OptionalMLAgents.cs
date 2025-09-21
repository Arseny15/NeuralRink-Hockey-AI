using UnityEngine;
using UnityEditor;

namespace NeuralRink.Setup
{
    /// <summary>
    /// Optional ML-Agents installer that can add ML-Agents packages if desired.
    /// This allows the game to work without ML-Agents but provides the option to add it.
    /// </summary>
    public class OptionalMLAgents : EditorWindow
    {
        [MenuItem("Neural Rink/Setup/Install ML-Agents (Optional)")]
        public static void ShowWindow()
        {
            GetWindow<OptionalMLAgents>("ML-Agents Installer");
        }
        
        private void OnGUI()
        {
            GUILayout.Label("🤖 ML-Agents Installation (Optional)", EditorStyles.boldLabel);
            GUILayout.Space(10);
            
            GUILayout.Label("The Neural Rink game works perfectly without ML-Agents using simple AI.");
            GUILayout.Label("However, you can install ML-Agents for advanced AI training capabilities.");
            GUILayout.Space(10);
            
            GUILayout.Label("Benefits of ML-Agents:");
            GUILayout.Label("• Advanced neural network AI");
            GUILayout.Label("• Reinforcement learning training");
            GUILayout.Label("• Your trained model (270.6 reward)");
            GUILayout.Label("• Professional AI development tools");
            GUILayout.Space(10);
            
            GUILayout.Label("Without ML-Agents:");
            GUILayout.Label("• Simple but effective AI goalie");
            GUILayout.Label("• No compilation errors");
            GUILayout.Label("• Faster Unity startup");
            GUILayout.Label("• Smaller project size");
            GUILayout.Space(20);
            
            if (GUILayout.Button("Install ML-Agents 4.0.0", GUILayout.Height(30)))
            {
                InstallMLAgents();
            }
            
            GUILayout.Space(10);
            
            if (GUILayout.Button("Keep Simple AI (Recommended)", GUILayout.Height(30)))
            {
                EditorUtility.DisplayDialog("Simple AI Selected", 
                    "Great choice! The game will use simple but effective AI.\n\n" +
                    "You can always install ML-Agents later if you want advanced AI features.", 
                    "OK");
                Close();
            }
        }
        
        /// <summary>
        /// Install ML-Agents packages.
        /// </summary>
        private void InstallMLAgents()
        {
            try
            {
                Debug.Log("📦 Installing ML-Agents 4.0.0...");
                
                // Add ML-Agents to package manifest
                AddMLAgentsToManifest();
                
                EditorUtility.DisplayDialog("ML-Agents Installation", 
                    "ML-Agents 4.0.0 has been added to the project!\n\n" +
                    "Unity will download and install the packages automatically.\n\n" +
                    "After installation, you can use your trained model (270.6 reward) for advanced AI!", 
                    "OK");
                
                Close();
            }
            catch (System.Exception e)
            {
                EditorUtility.DisplayDialog("Installation Error", 
                    $"Failed to install ML-Agents: {e.Message}\n\n" +
                    "The game will continue to work with simple AI.", 
                    "OK");
            }
        }
        
        /// <summary>
        /// Add ML-Agents to package manifest.
        /// </summary>
        private void AddMLAgentsToManifest()
        {
            string manifestPath = "Packages/manifest.json";
            string manifestContent = System.IO.File.ReadAllText(manifestPath);
            
            // Check if ML-Agents is already installed
            if (manifestContent.Contains("com.unity.ml-agents"))
            {
                Debug.Log("✅ ML-Agents already installed");
                return;
            }
            
            // Add ML-Agents packages
            string mlAgentsPackages = "    \"com.unity.ml-agents\": \"4.0.0\",\n    \"com.unity.ml-agents.extensions\": \"4.0.0\",\n";
            
            // Insert before the closing brace
            int insertIndex = manifestContent.LastIndexOf("}");
            manifestContent = manifestContent.Insert(insertIndex, mlAgentsPackages);
            
            System.IO.File.WriteAllText(manifestPath, manifestContent);
            
            Debug.Log("✅ ML-Agents packages added to manifest.json");
        }
    }
}
