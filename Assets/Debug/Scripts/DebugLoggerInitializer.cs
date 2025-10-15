using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;

/// <summary>
/// Global initializer for DebugLogger system
/// Ensures DebugLogger is initialized before any other scripts try to use it
/// </summary>
public class DebugLoggerInitializer : MonoBehaviour
{
    [SerializeField] private AssetReference debugConfigAsset;

    /// <summary>
    /// Initialize DebugLogger as early as possible
    /// This runs before any scene loads
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeBeforeSceneLoad()
    {
        // Create a temporary GameObject to run coroutine
        GameObject temp = new GameObject("DebugLoggerLoader");
        DontDestroyOnLoad(temp);
        temp.AddComponent<DebugLoggerLoader>().StartLoading();
    }

    /// <summary>
    /// Helper class to load DebugLogConfig asynchronously
    /// </summary>
    private class DebugLoggerLoader : MonoBehaviour
    {
        public void StartLoading()
        {
            StartCoroutine(LoadDebugConfig());
        }

        private IEnumerator LoadDebugConfig()
        {
#if UNITY_EDITOR
            // In editor, search assets synchronously
            string[] guids = UnityEditor.AssetDatabase.FindAssets("t:DebugLogConfig");
            if (guids.Length > 0)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guids[0]);
                DebugLogConfig config = UnityEditor.AssetDatabase.LoadAssetAtPath<DebugLogConfig>(path);

                if (config != null)
                {
                    DebugLogger.Initialize(config);
                    Debug.Log($"[DebugLogger] Initialized in editor with config from: {config.name}");
                }
            }
            else
            {
                Debug.LogWarning("[DebugLogger] No DebugLogConfig found in editor");
            }

            // Destroy temporary GameObject after loading
            Destroy(gameObject);
            yield break;
#else
            // In build, try to load from Addressables with timeout
            AsyncOperationHandle<DebugLogConfig> handle = Addressables.LoadAssetAsync<DebugLogConfig>("DebugLogConfig");

            float timeout = 5f; // 5 second timeout
            float elapsed = 0f;

            while (!handle.IsDone && elapsed < timeout)
            {
                elapsed += Time.deltaTime;
                yield return null;
            }

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                DebugLogger.Initialize(handle.Result);
                Debug.Log($"[DebugLogger] Initialized in build with config: {handle.Result.name}");
            }
            else
            {
                Debug.LogWarning($"[DebugLogger] Failed to load DebugLogConfig from Addressables (timeout or error). Debug logging disabled.");
                // Initialize with null - DebugLogger will handle it gracefully
                DebugLogger.Initialize(null);
            }

            // Destroy temporary GameObject after loading
            Destroy(gameObject);
#endif
        }
    }
}
