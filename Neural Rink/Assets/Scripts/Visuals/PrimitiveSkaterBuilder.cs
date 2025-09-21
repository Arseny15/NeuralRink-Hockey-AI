using UnityEngine;

namespace NeuralRink.Visuals
{
    /// <summary>
    /// Builder for creating detailed primitive-based hockey skater visuals.
    /// Generates realistic proportions and equipment using Unity primitives.
    /// </summary>
    public class PrimitiveSkaterBuilder : MonoBehaviour
    {
        [Header("Materials (optional)")]
        public Material jerseyMat;     // body & limbs
        public Material blackMat;      // helmet, stick, skates
        public Material skinMat;       // head (optional)

        [Header("Anchors (auto-created if missing)")]
        public Transform visualRoot;   // child "Visual"
        public Transform stickRoot;    // child "StickRoot" on player root
        public Transform stickTip;     // child "StickTip" under stick

        [Header("Proportions (meters)")]
        public float height = 1.78f;   // overall
        public float shoulderWidth = 0.46f;

        [Header("Performance")]
        [SerializeField] private bool disableInTraining = true;
        [SerializeField] private NeuralRink.Systems.TrainingSwitch trainingSwitch;

        /// <summary>
        /// Initialize visual builder.
        /// </summary>
        private void Start()
        {
            // Find training switch if not assigned
            if (trainingSwitch == null)
            {
                trainingSwitch = FindObjectOfType<NeuralRink.Systems.TrainingSwitch>();
            }

            // Check if we should disable visuals in training
            if (disableInTraining && trainingSwitch != null && trainingSwitch.IsTrainingMode())
            {
                if (visualRoot != null)
                {
                    visualRoot.gameObject.SetActive(false);
                }
                return;
            }

            // Auto-build if no visual exists
            if (visualRoot == null || visualRoot.childCount == 0)
            {
                Build();
            }
        }

        [ContextMenu("Build Visual (Forward)")]
        public void Build()
        {
            if (!visualRoot) visualRoot = PV.Ensure(transform, "Visual");
            PV.Clear(visualRoot);

            float torsoH = height * 0.60f;    // torso capsule height
            float hipY   = 0.95f;             // hip height
            float kneeY  = 0.55f;             // knee height
            float ankleY = 0.10f;

            // Torso (capsule) + chest (cube to square shoulders)
            PV.Make(visualRoot, "Torso", PrimitiveType.Capsule,
                new Vector3(0, torsoH*0.5f, 0),
                new Vector3(0.42f, torsoH, 0.42f),
                jerseyMat);

            PV.Make(visualRoot, "ChestPad", PrimitiveType.Cube,
                new Vector3(0, torsoH*0.80f, 0.03f),
                new Vector3(shoulderWidth, 0.22f, 0.18f),
                jerseyMat);

            // Head + helmet
            var head = PV.Make(visualRoot, "Head", PrimitiveType.Sphere,
                new Vector3(0, torsoH + 0.18f, 0),
                new Vector3(0.22f, 0.22f, 0.22f),
                skinMat ? skinMat : jerseyMat);
            PV.Make(head.transform, "Helmet", PrimitiveType.Sphere,
                Vector3.zero, new Vector3(1.15f, 1.0f, 1.15f), blackMat);

            // Arms: upper/lower = cylinders + sphere joints
            float upperLen = 0.30f, lowerLen = 0.28f, armRad = 0.09f;
            Vector3 shoulderL = new Vector3(-shoulderWidth*0.5f, torsoH*0.85f, 0.03f);
            Vector3 shoulderR = new Vector3( shoulderWidth*0.5f, torsoH*0.85f, 0.03f);

            PV.Make(visualRoot, "Shoulder_L", PrimitiveType.Sphere, shoulderL, Vector3.one*0.14f, jerseyMat);
            PV.Make(visualRoot, "Shoulder_R", PrimitiveType.Sphere, shoulderR, Vector3.one*0.14f, jerseyMat);

            PV.Make(visualRoot, "UpperArm_L", PrimitiveType.Cylinder, shoulderL + new Vector3(-upperLen*0.5f, -0.05f, 0),
                new Vector3(armRad, upperLen*0.5f, armRad), jerseyMat, Quaternion.Euler(0,0,90));
            PV.Make(visualRoot, "UpperArm_R", PrimitiveType.Cylinder, shoulderR + new Vector3( upperLen*0.5f, -0.05f, 0),
                new Vector3(armRad, upperLen*0.5f, armRad), jerseyMat, Quaternion.Euler(0,0,90));

            Vector3 elbowL = shoulderL + new Vector3(-upperLen, -0.05f, 0);
            Vector3 elbowR = shoulderR + new Vector3( upperLen, -0.05f, 0);
            PV.Make(visualRoot, "Elbow_L", PrimitiveType.Sphere, elbowL, Vector3.one*0.12f, jerseyMat);
            PV.Make(visualRoot, "Elbow_R", PrimitiveType.Sphere, elbowR, Vector3.one*0.12f, jerseyMat);

            PV.Make(visualRoot, "Forearm_L", PrimitiveType.Cylinder, elbowL + new Vector3(-lowerLen*0.5f, -0.05f, 0.02f),
                new Vector3(armRad*0.9f, lowerLen*0.5f, armRad*0.9f), jerseyMat, Quaternion.Euler(0,0,90));
            PV.Make(visualRoot, "Forearm_R", PrimitiveType.Cylinder, elbowR + new Vector3( lowerLen*0.5f, -0.05f, 0.02f),
                new Vector3(armRad*0.9f, lowerLen*0.5f, armRad*0.9f), jerseyMat, Quaternion.Euler(0,0,90));

            // Hands (small spheres)
            PV.Make(visualRoot, "Hand_L", PrimitiveType.Sphere, elbowL + new Vector3(-lowerLen, -0.05f, 0.02f), Vector3.one*0.11f, jerseyMat);
            PV.Make(visualRoot, "Hand_R", PrimitiveType.Sphere, elbowR + new Vector3( lowerLen, -0.05f, 0.02f), Vector3.one*0.11f, jerseyMat);

            // Legs: thighs + calves (cylinders) + knee joints
            float thighLen = hipY - kneeY;
            float calfLen  = kneeY - ankleY;
            float legRad   = 0.17f;

            Vector3 hipL = new Vector3(-0.18f, hipY, 0.03f);
            Vector3 hipR = new Vector3( 0.18f, hipY, 0.03f);

            PV.Make(visualRoot, "Thigh_L", PrimitiveType.Cylinder, hipL - new Vector3(0, thighLen*0.5f, 0),
                new Vector3(legRad, thighLen*0.5f, legRad), jerseyMat);
            PV.Make(visualRoot, "Thigh_R", PrimitiveType.Cylinder, hipR - new Vector3(0, thighLen*0.5f, 0),
                new Vector3(legRad, thighLen*0.5f, legRad), jerseyMat);

            Vector3 kneeL = new Vector3(-0.18f, kneeY, 0.03f);
            Vector3 kneeR = new Vector3( 0.18f, kneeY, 0.03f);
            PV.Make(visualRoot, "Knee_L", PrimitiveType.Sphere, kneeL, Vector3.one*0.14f, jerseyMat);
            PV.Make(visualRoot, "Knee_R", PrimitiveType.Sphere, kneeR, Vector3.one*0.14f, jerseyMat);

            PV.Make(visualRoot, "Calf_L", PrimitiveType.Cylinder, kneeL - new Vector3(0, calfLen*0.5f, 0),
                new Vector3(legRad*0.9f, calfLen*0.5f, legRad*0.9f), jerseyMat);
            PV.Make(visualRoot, "Calf_R", PrimitiveType.Cylinder, kneeR - new Vector3(0, calfLen*0.5f, 0),
                new Vector3(legRad*0.9f, calfLen*0.5f, legRad*0.9f), jerseyMat);

            // Skates (boot + blade)
            var bootL = PV.Make(visualRoot, "Skate_L", PrimitiveType.Cube, new Vector3(-0.18f, ankleY, 0), new Vector3(0.28f,0.12f,0.18f), blackMat);
            var bootR = PV.Make(visualRoot, "Skate_R", PrimitiveType.Cube, new Vector3( 0.18f, ankleY, 0), new Vector3(0.28f,0.12f,0.18f), blackMat);
            PV.Make(bootL.transform, "Blade_L", PrimitiveType.Cube, new Vector3(0, -0.07f, 0), new Vector3(0.28f,0.02f,0.02f), blackMat);
            PV.Make(bootR.transform, "Blade_R", PrimitiveType.Cube, new Vector3(0, -0.07f, 0), new Vector3(0.28f,0.02f,0.02f), blackMat);

            // Stick (root + blade + stickTip)
            BuildStick(torsoH);
        }

        /// <summary>
        /// Build hockey stick with proper proportions.
        /// </summary>
        private void BuildStick(float torsoH)
        {
            if (!stickRoot) stickRoot = PV.Ensure(transform, "StickRoot");
            stickRoot.localPosition = new Vector3(0.30f, torsoH*0.50f, 0.25f);
            stickRoot.localRotation = Quaternion.Euler(0, 10f, 0);

            var shaft = PV.Make(stickRoot, "Stick", PrimitiveType.Cube, Vector3.zero, new Vector3(1.25f,0.03f,0.05f), blackMat, Quaternion.identity);
            var blade = PV.Make(shaft.transform, "Blade", PrimitiveType.Cube, new Vector3(0.62f,-0.05f,0.20f), new Vector3(0.36f,0.02f,0.20f), blackMat);
            
            if (!stickTip) stickTip = PV.Ensure(shaft.transform, "StickTip");
            stickTip.localPosition = new Vector3(0.82f, -0.02f, 0.28f); // toe

            // Assign stick tip to player controller
            var playerController = GetComponent<NeuralRink.Gameplay.PlayerController>();
            if (playerController != null && stickTip != null)
            {
                // Use reflection to set private field if needed
                var field = typeof(NeuralRink.Gameplay.PlayerController).GetField("shotPoint", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(playerController, stickTip);
                }
            }
        }

        /// <summary>
        /// Clear all visual components.
        /// </summary>
        [ContextMenu("Clear Visual")]
        public void ClearVisual()
        {
            if (visualRoot)
            {
                PV.Clear(visualRoot);
            }
        }

        /// <summary>
        /// Set materials for the skater.
        /// </summary>
        public void SetMaterials(Material jersey, Material black, Material skin = null)
        {
            jerseyMat = jersey;
            blackMat = black;
            skinMat = skin;

            // Rebuild with new materials
            if (visualRoot != null && visualRoot.childCount > 0)
            {
                Build();
            }
        }

        /// <summary>
        /// Set skater height and proportions.
        /// </summary>
        public void SetProportions(float newHeight, float newShoulderWidth)
        {
            height = newHeight;
            shoulderWidth = newShoulderWidth;

            // Rebuild with new proportions
            if (visualRoot != null && visualRoot.childCount > 0)
            {
                Build();
            }
        }

        /// <summary>
        /// Get stick tip transform for shooting.
        /// </summary>
        public Transform GetStickTip()
        {
            return stickTip;
        }

        /// <summary>
        /// Get visual root transform.
        /// </summary>
        public Transform GetVisualRoot()
        {
            return visualRoot;
        }

        /// <summary>
        /// Enable or disable visual components.
        /// </summary>
        public void SetVisualActive(bool active)
        {
            if (visualRoot != null)
            {
                visualRoot.gameObject.SetActive(active);
            }
        }

        /// <summary>
        /// Check if visual is currently active.
        /// </summary>
        public bool IsVisualActive()
        {
            return visualRoot != null && visualRoot.gameObject.activeInHierarchy;
        }
    }
}
