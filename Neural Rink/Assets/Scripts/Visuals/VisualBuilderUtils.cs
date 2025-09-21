using UnityEngine;

namespace NeuralRink.Visuals
{
    /// <summary>
    /// Shared utility class for visual builders.
    /// Provides common functionality for creating and managing primitive-based visuals.
    /// </summary>
    internal static class PV
    {
        /// <summary>
        /// Create a primitive game object with specified properties.
        /// </summary>
        public static GameObject Make(Transform parent, string name, PrimitiveType type, Vector3 pos, Vector3 scale, Material mat = null, Quaternion? rot = null)
        {
            var go = GameObject.CreatePrimitive(type);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localScale = scale;
            go.transform.localRotation = rot ?? Quaternion.identity;
            
            // Apply material if provided
            if (mat != null)
            {
                var renderer = go.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    renderer.sharedMaterial = mat;
                }
            }
            
            // Disable collider for visuals only
            var col = go.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }
            
            return go;
        }

        /// <summary>
        /// Ensure a child transform exists, create if missing.
        /// </summary>
        public static Transform Ensure(Transform parent, string name)
        {
            var t = parent.Find(name);
            if (t == null)
            {
                var g = new GameObject(name);
                t = g.transform;
                t.SetParent(parent, false);
            }
            return t;
        }

        /// <summary>
        /// Clear all children of a transform.
        /// </summary>
        public static void Clear(Transform t)
        {
            if (t == null) return;

            for (int i = t.childCount - 1; i >= 0; i--)
            {
                var c = t.GetChild(i);
                if (c != null)
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        Object.DestroyImmediate(c.gameObject);
                    }
                    else
                    {
                        Object.Destroy(c.gameObject);
                    }
#else
                    Object.Destroy(c.gameObject);
#endif
                }
            }
        }

        /// <summary>
        /// Create a primitive with collider enabled (for physics objects).
        /// </summary>
        public static GameObject MakeWithCollider(Transform parent, string name, PrimitiveType type, Vector3 pos, Vector3 scale, Material mat = null, Quaternion? rot = null)
        {
            var go = Make(parent, name, type, pos, scale, mat, rot);
            
            // Re-enable collider
            var col = go.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = true;
            }
            
            return go;
        }

        /// <summary>
        /// Create a visual-only primitive (no collider).
        /// </summary>
        public static GameObject MakeVisualOnly(Transform parent, string name, PrimitiveType type, Vector3 pos, Vector3 scale, Material mat = null, Quaternion? rot = null)
        {
            var go = Make(parent, name, type, pos, scale, mat, rot);
            
            // Ensure collider is disabled
            var col = go.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }
            
            return go;
        }

        /// <summary>
        /// Create a primitive with specific physics material.
        /// </summary>
        public static GameObject MakeWithPhysicsMaterial(Transform parent, string name, PrimitiveType type, Vector3 pos, Vector3 scale, PhysicsMaterial physMat, Material mat = null, Quaternion? rot = null)
        {
            var go = MakeWithCollider(parent, name, type, pos, scale, mat, rot);
            
            // Apply physics material
            var col = go.GetComponent<Collider>();
            if (col != null && physMat != null)
            {
                col.material = physMat;
            }
            
            return go;
        }

        /// <summary>
        /// Create a primitive with rigidbody.
        /// </summary>
        public static GameObject MakeWithRigidbody(Transform parent, string name, PrimitiveType type, Vector3 pos, Vector3 scale, Material mat = null, Quaternion? rot = null, float mass = 1f)
        {
            var go = MakeWithCollider(parent, name, type, pos, scale, mat, rot);
            
            // Add rigidbody
            var rb = go.GetComponent<Rigidbody>();
            if (rb == null)
            {
                rb = go.AddComponent<Rigidbody>();
            }
            
            rb.mass = mass;
            
            return go;
        }

        /// <summary>
        /// Create a trigger collider primitive.
        /// </summary>
        public static GameObject MakeTrigger(Transform parent, string name, PrimitiveType type, Vector3 pos, Vector3 scale, Material mat = null, Quaternion? rot = null)
        {
            var go = MakeWithCollider(parent, name, type, pos, scale, mat, rot);
            
            // Set as trigger
            var col = go.GetComponent<Collider>();
            if (col != null)
            {
                col.isTrigger = true;
            }
            
            return go;
        }

        /// <summary>
        /// Apply material to all renderers in a transform hierarchy.
        /// </summary>
        public static void ApplyMaterialToHierarchy(Transform root, Material material)
        {
            if (root == null || material == null) return;

            var renderers = root.GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in renderers)
            {
                renderer.sharedMaterial = material;
            }
        }

        /// <summary>
        /// Set layer for entire hierarchy.
        /// </summary>
        public static void SetLayerRecursively(Transform root, int layer)
        {
            if (root == null) return;

            root.gameObject.layer = layer;
            
            for (int i = 0; i < root.childCount; i++)
            {
                SetLayerRecursively(root.GetChild(i), layer);
            }
        }

        /// <summary>
        /// Set tag for entire hierarchy.
        /// </summary>
        public static void SetTagRecursively(Transform root, string tag)
        {
            if (root == null || string.IsNullOrEmpty(tag)) return;

            root.gameObject.tag = tag;
            
            for (int i = 0; i < root.childCount; i++)
            {
                SetTagRecursively(root.GetChild(i), tag);
            }
        }

        /// <summary>
        /// Create a simple box collider trigger.
        /// </summary>
        public static GameObject CreateBoxTrigger(Transform parent, string name, Vector3 center, Vector3 size, string tag = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = center;
            
            var collider = go.AddComponent<BoxCollider>();
            collider.isTrigger = true;
            collider.size = size;
            collider.center = Vector3.zero;
            
            if (!string.IsNullOrEmpty(tag))
            {
                go.tag = tag;
            }
            
            return go;
        }

        /// <summary>
        /// Create a simple sphere collider trigger.
        /// </summary>
        public static GameObject CreateSphereTrigger(Transform parent, string name, Vector3 center, float radius, string tag = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = center;
            
            var collider = go.AddComponent<SphereCollider>();
            collider.isTrigger = true;
            collider.radius = radius;
            collider.center = Vector3.zero;
            
            if (!string.IsNullOrEmpty(tag))
            {
                go.tag = tag;
            }
            
            return go;
        }

        /// <summary>
        /// Create a simple capsule collider trigger.
        /// </summary>
        public static GameObject CreateCapsuleTrigger(Transform parent, string name, Vector3 center, float radius, float height, string tag = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = center;
            
            var collider = go.AddComponent<CapsuleCollider>();
            collider.isTrigger = true;
            collider.radius = radius;
            collider.height = height;
            collider.center = Vector3.zero;
            
            if (!string.IsNullOrEmpty(tag))
            {
                go.tag = tag;
            }
            
            return go;
        }

        /// <summary>
        /// Get or create a material with default properties.
        /// </summary>
        public static Material GetDefaultMaterial(string name, Color color)
        {
            // Try to find existing material
            var materials = Resources.FindObjectsOfTypeAll<Material>();
            foreach (var mat in materials)
            {
                if (mat.name == name)
                {
                    return mat;
                }
            }

            // Create new material
            var newMat = new Material(Shader.Find("Standard"));
            newMat.name = name;
            newMat.color = color;
            
            return newMat;
        }

        /// <summary>
        /// Create a simple directional light.
        /// </summary>
        public static Light CreateDirectionalLight(Transform parent, string name, Vector3 position, Quaternion rotation, Color color, float intensity = 1f)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = position;
            go.transform.rotation = rotation;
            
            var light = go.AddComponent<Light>();
            light.type = LightType.Directional;
            light.color = color;
            light.intensity = intensity;
            
            return light;
        }

        /// <summary>
        /// Create a simple camera.
        /// </summary>
        public static Camera CreateCamera(Transform parent, string name, Vector3 position, Quaternion rotation, float fieldOfView = 60f)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.position = position;
            go.transform.rotation = rotation;
            
            var camera = go.AddComponent<Camera>();
            camera.fieldOfView = fieldOfView;
            
            return camera;
        }
    }
}
