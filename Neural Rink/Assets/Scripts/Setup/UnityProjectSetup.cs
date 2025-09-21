using UnityEngine;
using UnityEditor;
using System.IO;

namespace NeuralRink.Setup
{
    /// <summary>
    /// Automated Unity project setup for Neural Rink
    /// Run this script to automatically configure the project
    /// </summary>
    public class UnityProjectSetup : EditorWindow
    {
        [MenuItem("Neural Rink/Setup Project")]
        public static void ShowWindow()
        {
            GetWindow<UnityProjectSetup>("Neural Rink Setup");
        }

        private void OnGUI()
        {
            GUILayout.Label("Neural Rink Project Setup", EditorStyles.boldLabel);
            GUILayout.Space(10);

            if (GUILayout.Button("1. Create Folders", GUILayout.Height(30)))
            {
                CreateFolders();
            }

            if (GUILayout.Button("2. Create Tags & Layers", GUILayout.Height(30)))
            {
                CreateTagsAndLayers();
            }

            if (GUILayout.Button("3. Create Physics Materials", GUILayout.Height(30)))
            {
                CreatePhysicsMaterials();
            }

            if (GUILayout.Button("4. Create ScriptableObjects", GUILayout.Height(30)))
            {
                CreateScriptableObjects();
            }

            if (GUILayout.Button("5. Create Prefabs", GUILayout.Height(30)))
            {
                CreatePrefabs();
            }

            if (GUILayout.Button("6. Create Scenes", GUILayout.Height(30)))
            {
                CreateScenes();
            }

            if (GUILayout.Button("7. Configure Project Settings", GUILayout.Height(30)))
            {
                ConfigureProjectSettings();
            }

            GUILayout.Space(20);
            if (GUILayout.Button("Run All Setup Steps", GUILayout.Height(40)))
            {
                RunAllSetup();
            }

            GUILayout.Space(10);
            GUILayout.Label("Status: Ready for setup", EditorStyles.helpBox);
        }

        private void CreateFolders()
        {
            string[] folders = {
                "Assets/Materials",
                "Assets/Prefabs",
                "Assets/Scenes",
                "Assets/Models",
                "Assets/Scripts/Agents",
                "Assets/Scripts/Gameplay",
                "Assets/Scripts/Systems",
                "Assets/Scripts/Utils",
                "Assets/Scripts/Setup",
                "Assets/Scripts/UI",
                "Assets/Scripts/Visuals",
                "Assets/Scripts/Audio",
                "Assets/Scripts/Effects",
                "Assets/Input",
                "Assets/ML-Agents"
            };

            foreach (string folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    string parent = Path.GetDirectoryName(folder);
                    string child = Path.GetFileName(folder);
                    AssetDatabase.CreateFolder(parent, child);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("✅ Folders created successfully!");
        }

        private void CreateTagsAndLayers()
        {
            // Create tags
            string[] tags = { "Player", "Puck", "Goal", "SaveZone", "OutOfPlay" };
            
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            foreach (string tag in tags)
            {
                bool found = false;
                for (int i = 0; i < tagsProp.arraySize; i++)
                {
                    SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                    if (t.stringValue.Equals(tag))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    tagsProp.InsertArrayElementAtIndex(0);
                    SerializedProperty newTagProp = tagsProp.GetArrayElementAtIndex(0);
                    newTagProp.stringValue = tag;
                }
            }

            tagManager.ApplyModifiedProperties();
            Debug.Log("✅ Tags created successfully!");
        }

        private void CreatePhysicsMaterials()
        {
            string materialsPath = "Assets/Materials";

            // Ice Physics Material
            PhysicsMaterial iceMat = new PhysicsMaterial("Ice");
            iceMat.dynamicFriction = 0.02f;
            iceMat.staticFriction = 0.02f;
            iceMat.bounciness = 0f;
            iceMat.frictionCombine = PhysicsMaterialCombine.Minimum;
            AssetDatabase.CreateAsset(iceMat, $"{materialsPath}/Ice.physicMaterial");

            // Puck Physics Material
            PhysicsMaterial puckMat = new PhysicsMaterial("Puck");
            puckMat.dynamicFriction = 0.01f;
            puckMat.staticFriction = 0.01f;
            puckMat.bounciness = 0.05f;
            puckMat.frictionCombine = PhysicsMaterialCombine.Minimum;
            AssetDatabase.CreateAsset(puckMat, $"{materialsPath}/Puck.physicMaterial");

            // Wall Physics Material
            PhysicsMaterial wallMat = new PhysicsMaterial("Wall");
            wallMat.dynamicFriction = 0.8f;
            wallMat.staticFriction = 0.8f;
            wallMat.bounciness = 0f;
            wallMat.frictionCombine = PhysicsMaterialCombine.Average;
            AssetDatabase.CreateAsset(wallMat, $"{materialsPath}/Wall.physicMaterial");

            AssetDatabase.Refresh();
            Debug.Log("✅ Physics materials created successfully!");
        }

        private void CreateScriptableObjects()
        {
            string scriptsPath = "Assets/Scripts/Systems";

            // TrainingSwitch
            TrainingSwitch trainingSwitch = ScriptableObject.CreateInstance<TrainingSwitch>();
            trainingSwitch.TrainingMode = true;
            AssetDatabase.CreateAsset(trainingSwitch, $"{scriptsPath}/TrainingSwitch.asset");

            // SalarySystem
            SalarySystem salarySystem = ScriptableObject.CreateInstance<SalarySystem>();
            AssetDatabase.CreateAsset(salarySystem, $"{scriptsPath}/SalarySystem.asset");

            AssetDatabase.Refresh();
            Debug.Log("✅ ScriptableObjects created successfully!");
        }

        private void CreatePrefabs()
        {
            string prefabsPath = "Assets/Prefabs";

            // Create Puck Prefab
            GameObject puck = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            puck.name = "Puck";
            puck.tag = "Puck";
            puck.layer = LayerMask.NameToLayer("Puck");
            
            // Configure Rigidbody
            Rigidbody puckRb = puck.AddComponent<Rigidbody>();
            puckRb.mass = 0.17f;
            puckRb.linearDamping = 0.05f;
            puckRb.angularDamping = 0.05f;
            puckRb.useGravity = true;
            puckRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            // Configure Collider
            SphereCollider puckCol = puck.GetComponent<SphereCollider>();
            puckCol.radius = 0.15f;
            puckCol.material = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>("Assets/Materials/Puck.physicMaterial");

            // Add Controller
            puck.AddComponent<PuckController>();

            // Save as Prefab
            PrefabUtility.SaveAsPrefabAsset(puck, $"{prefabsPath}/Puck.prefab");
            DestroyImmediate(puck);

            Debug.Log("✅ Prefabs created successfully!");
        }

        private void CreateScenes()
        {
            // Create Training Scene
            SceneAsset trainingScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(trainingScene, "Assets/Scenes/Training.unity");

            // Create Play Scene
            SceneAsset playScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(playScene, "Assets/Scenes/Play.unity");

            Debug.Log("✅ Scenes created successfully!");
        }

        private void ConfigureProjectSettings()
        {
            // Configure Time settings
            Time.fixedDeltaTime = 0.02f;
            Time.maximumDeltaTime = 0.33f;

            // Configure Physics settings
            Physics.defaultSolverIterations = 12;
            Physics.defaultSolverVelocityIterations = 2;

            Debug.Log("✅ Project settings configured successfully!");
        }

        private void RunAllSetup()
        {
            CreateFolders();
            CreateTagsAndLayers();
            CreatePhysicsMaterials();
            CreateScriptableObjects();
            CreatePrefabs();
            CreateScenes();
            ConfigureProjectSettings();

            Debug.Log("🎉 Complete project setup finished!");
            Debug.Log("Next steps:");
            Debug.Log("1. Open Training.unity and Play.unity scenes");
            Debug.Log("2. Follow the Unity Setup Guide for detailed scene configuration");
            Debug.Log("3. Run training with: python alternative_train.py --run-id neural_rink_full --max-steps 100000");
        }
    }
}
