using UnityEngine;
using System.Collections.Generic;
using NeuralRink.Systems;

namespace NeuralRink.Effects
{
    /// <summary>
    /// Manages particle effects for Neural Rink including goals, saves, and ice effects.
    /// Provides centralized control over visual effects with performance optimization.
    /// </summary>
    public class ParticleEffectManager : MonoBehaviour
    {
        [Header("Effect Prefabs")]
        [SerializeField] private GameObject goalEffectPrefab;
        [SerializeField] private GameObject saveEffectPrefab;
        [SerializeField] private GameObject iceSprayEffectPrefab;
        [SerializeField] private GameObject stickHitEffectPrefab;
        [SerializeField] private GameObject puckTrailEffectPrefab;
        
        [Header("Effect Settings")]
        [SerializeField] private float effectLifetime = 3f;
        [SerializeField] private int maxActiveEffects = 20;
        [SerializeField] private bool disableInTraining = true;
        
        [Header("Performance")]
        [SerializeField] private TrainingSwitch trainingSwitch;
        [SerializeField] private bool useObjectPooling = true;
        
        // Effect pools
        private Queue<GameObject> goalEffectPool = new Queue<GameObject>();
        private Queue<GameObject> saveEffectPool = new Queue<GameObject>();
        private Queue<GameObject> iceSprayPool = new Queue<GameObject>();
        private Queue<GameObject> stickHitPool = new Queue<GameObject>();
        private Queue<GameObject> puckTrailPool = new Queue<GameObject>();
        
        // Active effects tracking
        private List<GameObject> activeEffects = new List<GameObject>();
        
        // Singleton instance
        public static ParticleEffectManager Instance { get; private set; }
        
        /// <summary>
        /// Initialize particle effect manager.
        /// </summary>
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeEffectManager();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// Initialize effect manager components.
        /// </summary>
        private void InitializeEffectManager()
        {
            // Find training switch
            if (trainingSwitch == null)
            {
                trainingSwitch = FindFirstObjectByType<TrainingSwitch>();
            }
            
            // Create default effects if not assigned
            CreateDefaultEffects();
            
            // Initialize object pools
            if (useObjectPooling)
            {
                InitializeEffectPools();
            }
        }
        
        /// <summary>
        /// Create default particle effects if not assigned.
        /// </summary>
        private void CreateDefaultEffects()
        {
            if (goalEffectPrefab == null)
            {
                goalEffectPrefab = CreateDefaultGoalEffect();
            }
            
            if (saveEffectPrefab == null)
            {
                saveEffectPrefab = CreateDefaultSaveEffect();
            }
            
            if (iceSprayEffectPrefab == null)
            {
                iceSprayEffectPrefab = CreateDefaultIceSprayEffect();
            }
            
            if (stickHitEffectPrefab == null)
            {
                stickHitEffectPrefab = CreateDefaultStickHitEffect();
            }
            
            if (puckTrailEffectPrefab == null)
            {
                puckTrailEffectPrefab = CreateDefaultPuckTrailEffect();
            }
        }
        
        /// <summary>
        /// Create default goal effect.
        /// </summary>
        private GameObject CreateDefaultGoalEffect()
        {
            GameObject effect = new GameObject("GoalEffect");
            
            // Add particle system
            ParticleSystem particles = effect.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.startLifetime = 2f;
            main.startSpeed = 5f;
            main.startSize = 0.5f;
            main.startColor = Color.red;
            main.maxParticles = 100;
            
            // Add emission
            var emission = particles.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 50),
                new ParticleSystem.Burst(0.1f, 30),
                new ParticleSystem.Burst(0.2f, 20)
            });
            
            // Add shape
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 1f;
            
            // Add velocity over lifetime
            var velocityOverLifetime = particles.velocityOverLifetime;
            velocityOverLifetime.enabled = true;
            velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;
            velocityOverLifetime.radial = new ParticleSystem.MinMaxCurve(2f);
            
            return effect;
        }
        
        /// <summary>
        /// Create default save effect.
        /// </summary>
        private GameObject CreateDefaultSaveEffect()
        {
            GameObject effect = new GameObject("SaveEffect");
            
            // Add particle system
            ParticleSystem particles = effect.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.startLifetime = 1.5f;
            main.startSpeed = 3f;
            main.startSize = 0.3f;
            main.startColor = Color.green;
            main.maxParticles = 75;
            
            // Add emission
            var emission = particles.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 40),
                new ParticleSystem.Burst(0.1f, 20),
                new ParticleSystem.Burst(0.2f, 15)
            });
            
            // Add shape
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Cone;
            shape.angle = 30f;
            shape.radius = 0.5f;
            
            return effect;
        }
        
        /// <summary>
        /// Create default ice spray effect.
        /// </summary>
        private GameObject CreateDefaultIceSprayEffect()
        {
            GameObject effect = new GameObject("IceSprayEffect");
            
            // Add particle system
            ParticleSystem particles = effect.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.startLifetime = 0.8f;
            main.startSpeed = 8f;
            main.startSize = 0.1f;
            main.startColor = Color.white;
            main.maxParticles = 50;
            
            // Add emission
            var emission = particles.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 25),
                new ParticleSystem.Burst(0.05f, 15),
                new ParticleSystem.Burst(0.1f, 10)
            });
            
            // Add shape
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.2f;
            
            // Add gravity
            var forceOverLifetime = particles.forceOverLifetime;
            forceOverLifetime.enabled = true;
            forceOverLifetime.y = new ParticleSystem.MinMaxCurve(-2f);
            
            return effect;
        }
        
        /// <summary>
        /// Create default stick hit effect.
        /// </summary>
        private GameObject CreateDefaultStickHitEffect()
        {
            GameObject effect = new GameObject("StickHitEffect");
            
            // Add particle system
            ParticleSystem particles = effect.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.startLifetime = 0.5f;
            main.startSpeed = 4f;
            main.startSize = 0.2f;
            main.startColor = Color.yellow;
            main.maxParticles = 30;
            
            // Add emission
            var emission = particles.emission;
            emission.rateOverTime = 0;
            emission.SetBursts(new ParticleSystem.Burst[]
            {
                new ParticleSystem.Burst(0f, 20),
                new ParticleSystem.Burst(0.05f, 10)
            });
            
            // Add shape
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.1f;
            
            return effect;
        }
        
        /// <summary>
        /// Create default puck trail effect.
        /// </summary>
        private GameObject CreateDefaultPuckTrailEffect()
        {
            GameObject effect = new GameObject("PuckTrailEffect");
            
            // Add particle system
            ParticleSystem particles = effect.AddComponent<ParticleSystem>();
            var main = particles.main;
            main.startLifetime = 1f;
            main.startSpeed = 0f;
            main.startSize = 0.05f;
            main.startColor = Color.white;
            main.maxParticles = 100;
            
            // Add emission
            var emission = particles.emission;
            emission.rateOverTime = 50f;
            
            // Add shape
            var shape = particles.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.05f;
            
            // Add velocity over lifetime
            var velocityOverLifetime = particles.velocityOverLifetime;
            velocityOverLifetime.enabled = true;
            velocityOverLifetime.space = ParticleSystemSimulationSpace.Local;
            velocityOverLifetime.radial = new ParticleSystem.MinMaxCurve(-1f);
            
            return effect;
        }
        
        /// <summary>
        /// Initialize effect object pools.
        /// </summary>
        private void InitializeEffectPools()
        {
            // Create goal effect pool
            for (int i = 0; i < 5; i++)
            {
                GameObject effect = Instantiate(goalEffectPrefab, transform);
                effect.SetActive(false);
                goalEffectPool.Enqueue(effect);
            }
            
            // Create save effect pool
            for (int i = 0; i < 5; i++)
            {
                GameObject effect = Instantiate(saveEffectPrefab, transform);
                effect.SetActive(false);
                saveEffectPool.Enqueue(effect);
            }
            
            // Create ice spray pool
            for (int i = 0; i < 10; i++)
            {
                GameObject effect = Instantiate(iceSprayEffectPrefab, transform);
                effect.SetActive(false);
                iceSprayPool.Enqueue(effect);
            }
            
            // Create stick hit pool
            for (int i = 0; i < 10; i++)
            {
                GameObject effect = Instantiate(stickHitEffectPrefab, transform);
                effect.SetActive(false);
                stickHitPool.Enqueue(effect);
            }
            
            // Create puck trail pool
            for (int i = 0; i < 3; i++)
            {
                GameObject effect = Instantiate(puckTrailEffectPrefab, transform);
                effect.SetActive(false);
                puckTrailPool.Enqueue(effect);
            }
        }
        
        /// <summary>
        /// Play goal effect at position.
        /// </summary>
        public void PlayGoalEffect(Vector3 position)
        {
            if (ShouldDisableEffects()) return;
            
            GameObject effect = GetEffectFromPool(goalEffectPool, goalEffectPrefab);
            if (effect != null)
            {
                PlayEffect(effect, position, effectLifetime);
            }
        }
        
        /// <summary>
        /// Play save effect at position.
        /// </summary>
        public void PlaySaveEffect(Vector3 position)
        {
            if (ShouldDisableEffects()) return;
            
            GameObject effect = GetEffectFromPool(saveEffectPool, saveEffectPrefab);
            if (effect != null)
            {
                PlayEffect(effect, position, effectLifetime);
            }
        }
        
        /// <summary>
        /// Play ice spray effect at position.
        /// </summary>
        public void PlayIceSprayEffect(Vector3 position, Vector3 direction)
        {
            if (ShouldDisableEffects()) return;
            
            GameObject effect = GetEffectFromPool(iceSprayPool, iceSprayEffectPrefab);
            if (effect != null)
            {
                PlayEffect(effect, position, effectLifetime);
                
                // Orient effect in direction
                if (direction != Vector3.zero)
                {
                    effect.transform.rotation = Quaternion.LookRotation(direction);
                }
            }
        }
        
        /// <summary>
        /// Play stick hit effect at position.
        /// </summary>
        public void PlayStickHitEffect(Vector3 position, Vector3 direction)
        {
            if (ShouldDisableEffects()) return;
            
            GameObject effect = GetEffectFromPool(stickHitPool, stickHitEffectPrefab);
            if (effect != null)
            {
                PlayEffect(effect, position, effectLifetime * 0.5f);
                
                // Orient effect in direction
                if (direction != Vector3.zero)
                {
                    effect.transform.rotation = Quaternion.LookRotation(direction);
                }
            }
        }
        
        /// <summary>
        /// Start puck trail effect.
        /// </summary>
        public GameObject StartPuckTrailEffect(Transform followTarget)
        {
            if (ShouldDisableEffects()) return null;
            
            GameObject effect = GetEffectFromPool(puckTrailPool, puckTrailEffectPrefab);
            if (effect != null)
            {
                effect.SetActive(true);
                effect.transform.position = followTarget.position;
                
                // Make effect follow target
                var follow = effect.GetComponent<EffectFollower>();
                if (follow == null)
                {
                    follow = effect.AddComponent<EffectFollower>();
                }
                follow.SetTarget(followTarget);
                
                activeEffects.Add(effect);
            }
            
            return effect;
        }
        
        /// <summary>
        /// Stop puck trail effect.
        /// </summary>
        public void StopPuckTrailEffect(GameObject effect)
        {
            if (effect == null) return;
            
            var follow = effect.GetComponent<EffectFollower>();
            if (follow != null)
            {
                follow.SetTarget(null);
            }
            
            ReturnEffectToPool(effect, puckTrailPool);
        }
        
        /// <summary>
        /// Get effect from pool or create new one.
        /// </summary>
        private GameObject GetEffectFromPool(Queue<GameObject> pool, GameObject prefab)
        {
            if (useObjectPooling && pool.Count > 0)
            {
                return pool.Dequeue();
            }
            else
            {
                return Instantiate(prefab, transform);
            }
        }
        
        /// <summary>
        /// Play effect at position.
        /// </summary>
        private void PlayEffect(GameObject effect, Vector3 position, float lifetime)
        {
            effect.SetActive(true);
            effect.transform.position = position;
            effect.transform.rotation = Quaternion.identity;
            
            // Play particle system
            ParticleSystem particles = effect.GetComponent<ParticleSystem>();
            if (particles != null)
            {
                particles.Play();
            }
            
            activeEffects.Add(effect);
            
            // Return to pool after lifetime
            StartCoroutine(ReturnEffectAfterLifetime(effect, lifetime));
        }
        
        /// <summary>
        /// Return effect to pool after lifetime.
        /// </summary>
        private System.Collections.IEnumerator ReturnEffectAfterLifetime(GameObject effect, float lifetime)
        {
            yield return new WaitForSeconds(lifetime);
            
            if (effect != null)
            {
                // Stop particle system
                ParticleSystem particles = effect.GetComponent<ParticleSystem>();
                if (particles != null)
                {
                    particles.Stop();
                }
                
                // Determine which pool to return to
                Queue<GameObject> targetPool = DetermineEffectPool(effect);
                ReturnEffectToPool(effect, targetPool);
            }
        }
        
        /// <summary>
        /// Determine which pool an effect belongs to.
        /// </summary>
        private Queue<GameObject> DetermineEffectPool(GameObject effect)
        {
            string effectName = effect.name;
            
            if (effectName.Contains("Goal"))
                return goalEffectPool;
            else if (effectName.Contains("Save"))
                return saveEffectPool;
            else if (effectName.Contains("IceSpray"))
                return iceSprayPool;
            else if (effectName.Contains("StickHit"))
                return stickHitPool;
            else if (effectName.Contains("PuckTrail"))
                return puckTrailPool;
            
            return null;
        }
        
        /// <summary>
        /// Return effect to pool.
        /// </summary>
        private void ReturnEffectToPool(GameObject effect, Queue<GameObject> pool)
        {
            if (effect == null || pool == null) return;
            
            effect.SetActive(false);
            activeEffects.Remove(effect);
            
            if (useObjectPooling)
            {
                pool.Enqueue(effect);
            }
            else
            {
                Destroy(effect);
            }
        }
        
        /// <summary>
        /// Check if effects should be disabled.
        /// </summary>
        private bool ShouldDisableEffects()
        {
            return disableInTraining && 
                   trainingSwitch != null && 
                   trainingSwitch.IsTrainingMode();
        }
        
        /// <summary>
        /// Clear all active effects.
        /// </summary>
        public void ClearAllEffects()
        {
            foreach (GameObject effect in activeEffects)
            {
                if (effect != null)
                {
                    ParticleSystem particles = effect.GetComponent<ParticleSystem>();
                    if (particles != null)
                    {
                        particles.Stop();
                    }
                    
                    effect.SetActive(false);
                }
            }
            
            activeEffects.Clear();
        }
        
        /// <summary>
        /// Set effect lifetime.
        /// </summary>
        public void SetEffectLifetime(float lifetime)
        {
            effectLifetime = Mathf.Max(0.1f, lifetime);
        }
        
        /// <summary>
        /// Set maximum active effects.
        /// </summary>
        public void SetMaxActiveEffects(int max)
        {
            maxActiveEffects = Mathf.Max(1, max);
        }
        
        /// <summary>
        /// Enable or disable effects in training mode.
        /// </summary>
        public void SetDisableInTraining(bool disable)
        {
            disableInTraining = disable;
        }
        
        /// <summary>
        /// Get current number of active effects.
        /// </summary>
        public int GetActiveEffectCount()
        {
            return activeEffects.Count;
        }
        
        /// <summary>
        /// Check if effects are disabled in training.
        /// </summary>
        public bool IsDisabledInTraining()
        {
            return disableInTraining;
        }
        
        /// <summary>
        /// Cleanup on destroy.
        /// </summary>
        private void OnDestroy()
        {
            ClearAllEffects();
        }
    }
    
    /// <summary>
    /// Component for making effects follow a target transform.
    /// </summary>
    public class EffectFollower : MonoBehaviour
    {
        private Transform target;
        private Vector3 offset;
        
        /// <summary>
        /// Set target to follow.
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            if (target != null)
            {
                offset = transform.position - target.position;
            }
        }
        
        /// <summary>
        /// Update effect position.
        /// </summary>
        private void Update()
        {
            if (target != null)
            {
                transform.position = target.position + offset;
            }
        }
    }
}
