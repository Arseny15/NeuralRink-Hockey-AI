using UnityEngine;

namespace NeuralRink.Visuals
{
    /// <summary>
    /// Builder for creating detailed primitive-based goalie visuals.
    /// Generates realistic goalie proportions and equipment using Unity primitives.
    /// </summary>
    public class PrimitiveGoalieBuilder : MonoBehaviour
    {
        [Header("Materials (optional)")]
        public Material jerseyMat;    // torso/arms
        public Material padsMat;      // leg pads, glove/blocker
        public Material blackMat;     // helmet rim etc.

        [Header("Anchors (auto)")]
        public Transform visualRoot;
        public Transform goalCenter;   // set to GoalieAgent.goalCenter
        public Transform saveZone;     // place trigger here

        [Header("Bounds")]
        public float torsoW = 0.95f, torsoH = 1.05f, torsoD = 0.40f;

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
                trainingSwitch = FindFirstObjectByType<NeuralRink.Systems.TrainingSwitch>();
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

        [ContextMenu("Build Visual (Goalie)")]
        public void Build()
        {
            if (!visualRoot) visualRoot = PV.Ensure(transform, "Visual");
            PV.Clear(visualRoot);

            // Torso block + shoulder pad shelf
            PV.Make(visualRoot, "Torso", PrimitiveType.Cube, new Vector3(0, torsoH*0.5f, 0),
                new Vector3(torsoW, torsoH, torsoD), jerseyMat);
            PV.Make(visualRoot, "ShoulderPad", PrimitiveType.Cube, new Vector3(0, torsoH*0.90f, 0.05f),
                new Vector3(torsoW*1.15f, 0.18f, torsoD*0.6f), jerseyMat);

            // Head + helmet
            PV.Make(visualRoot, "Helmet", PrimitiveType.Sphere, new Vector3(0, torsoH + 0.22f, 0.02f),
                new Vector3(0.28f,0.28f,0.28f), blackMat);

            // Arms (cylinders) + blocker + glove
            BuildArms();

            // Leg pads
            BuildLegPads();

            // Skates
            BuildSkates();

            // Goal reference & save zone anchors
            BuildAnchors();
        }

        /// <summary>
        /// Build goalie arms with glove and blocker.
        /// </summary>
        private void BuildArms()
        {
            float armLen = 0.32f, armRad = 0.10f;
            Vector3 shL = new Vector3(-torsoW*0.55f, torsoH*0.70f, 0.12f);
            Vector3 shR = new Vector3( torsoW*0.55f, torsoH*0.70f, 0.12f);
            
            PV.Make(visualRoot, "Arm_L", PrimitiveType.Cylinder, shL, new Vector3(armRad, armLen*0.5f, armRad), jerseyMat, Quaternion.Euler(0,0,90));
            PV.Make(visualRoot, "Arm_R", PrimitiveType.Cylinder, shR, new Vector3(armRad, armLen*0.5f, armRad), jerseyMat, Quaternion.Euler(0,0,90));

            // Glove and blocker
            PV.Make(visualRoot, "Glove_L", PrimitiveType.Cube, shL + new Vector3(-armLen,0,0.02f), new Vector3(0.24f,0.20f,0.08f), padsMat);
            PV.Make(visualRoot, "Blocker_R", PrimitiveType.Cube, shR + new Vector3( armLen,0,0.02f), new Vector3(0.26f,0.16f,0.08f), padsMat);
        }

        /// <summary>
        /// Build goalie leg pads.
        /// </summary>
        private void BuildLegPads()
        {
            float padH = 0.90f, padW = 0.30f, padD = 0.12f;
            PV.Make(visualRoot, "Pad_L", PrimitiveType.Cube, new Vector3(-padW*1.0f, padH*0.5f, torsoD*0.10f), new Vector3(padW,padH,padD), padsMat);
            PV.Make(visualRoot, "Pad_R", PrimitiveType.Cube, new Vector3( padW*1.0f, padH*0.5f, torsoD*0.10f), new Vector3(padW,padH,padD), padsMat);
        }

        /// <summary>
        /// Build goalie skates.
        /// </summary>
        private void BuildSkates()
        {
            float padW = 0.30f;
            PV.Make(visualRoot, "Skate_L", PrimitiveType.Cube, new Vector3(-padW*1.0f, 0.12f, 0), new Vector3(0.30f,0.12f,0.18f), blackMat);
            PV.Make(visualRoot, "Skate_R", PrimitiveType.Cube, new Vector3( padW*1.0f, 0.12f, 0), new Vector3(0.30f,0.12f,0.18f), blackMat);
        }

        /// <summary>
        /// Build anchor points for goalie systems.
        /// </summary>
        private void BuildAnchors()
        {
            // Goal reference & save zone anchors for existing scripts
            if (!goalCenter) goalCenter = PV.Ensure(transform, "GoalCenter");
            goalCenter.localPosition = new Vector3(0, 0.90f, -0.55f);
            
            if (!saveZone) saveZone = PV.Ensure(transform, "SaveZoneAnchor");
            saveZone.localPosition = new Vector3(0, 1.05f, 0.35f);

            // Assign goal center to goalie agent
            var goalieAgent = GetComponent<NeuralRink.Agents.GoalieAgent>();
            if (goalieAgent != null && goalCenter != null)
            {
                // Use reflection to set private field if needed
                var field = typeof(NeuralRink.Agents.GoalieAgent).GetField("goalTransform", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(goalieAgent, goalCenter);
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
        /// Set materials for the goalie.
        /// </summary>
        public void SetMaterials(Material jersey, Material pads, Material black)
        {
            jerseyMat = jersey;
            padsMat = pads;
            blackMat = black;

            // Rebuild with new materials
            if (visualRoot != null && visualRoot.childCount > 0)
            {
                Build();
            }
        }

        /// <summary>
        /// Set goalie torso dimensions.
        /// </summary>
        public void SetTorsoDimensions(float width, float height, float depth)
        {
            torsoW = width;
            torsoH = height;
            torsoD = depth;

            // Rebuild with new dimensions
            if (visualRoot != null && visualRoot.childCount > 0)
            {
                Build();
            }
        }

        /// <summary>
        /// Get goal center transform.
        /// </summary>
        public Transform GetGoalCenter()
        {
            return goalCenter;
        }

        /// <summary>
        /// Get save zone anchor transform.
        /// </summary>
        public Transform GetSaveZoneAnchor()
        {
            return saveZone;
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

        /// <summary>
        /// Create save zone trigger at the anchor position.
        /// </summary>
        public void CreateSaveZoneTrigger()
        {
            if (saveZone == null) return;

            // Add BoxCollider as trigger
            BoxCollider trigger = saveZone.GetComponent<BoxCollider>();
            if (trigger == null)
            {
                trigger = saveZone.gameObject.AddComponent<BoxCollider>();
            }

            trigger.isTrigger = true;
            trigger.size = new Vector3(1.0f, 1.0f, 0.2f);
            trigger.center = Vector3.zero;

            // Add TriggerRelay script
            var triggerRelay = saveZone.GetComponent<NeuralRink.Gameplay.TriggerRelay>();
            if (triggerRelay == null)
            {
                triggerRelay = saveZone.gameObject.AddComponent<NeuralRink.Gameplay.TriggerRelay>();
                triggerRelay.SetTriggerType(NeuralRink.Gameplay.TriggerRelay.TriggerType.Save);
            }

            // Set tag
            saveZone.gameObject.tag = "SaveZone";
        }

        /// <summary>
        /// Setup goalie agent references.
        /// </summary>
        public void SetupGoalieAgentReferences()
        {
            var goalieAgent = GetComponent<NeuralRink.Agents.GoalieAgent>();
            if (goalieAgent == null) return;

            // Assign goal center reference
            if (goalCenter != null)
            {
                // Use reflection to set private field if needed
                var field = typeof(NeuralRink.Agents.GoalieAgent).GetField("goalTransform", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field != null)
                {
                    field.SetValue(goalieAgent, goalCenter);
                }
            }
        }
    }
}
