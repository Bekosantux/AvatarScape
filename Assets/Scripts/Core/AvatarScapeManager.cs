using UnityEngine;

namespace AvatarScape.Core
{
    /// <summary>
    /// AvatarScapeアプリケーションのメインマネージャー
    /// </summary>
    public class AvatarScapeManager : MonoBehaviour
    {
        public static AvatarScapeManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private AvatarLoader avatarLoader;

        [Header("Debug")]
        [SerializeField] private bool enableDebugLogs = true;

        public AvatarLoader AvatarLoader => avatarLoader;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            Initialize();
        }

        private void Initialize()
        {
            Log("AvatarScape initializing...");

            if (avatarLoader == null)
            {
                avatarLoader = FindObjectOfType<AvatarLoader>();
            }

            if (avatarLoader != null)
            {
                avatarLoader.OnAvatarLoaded += OnAvatarLoaded;
                avatarLoader.OnAvatarLoadFailed += OnAvatarLoadFailed;
                avatarLoader.OnLoadProgress += OnLoadProgress;
            }

            Log("AvatarScape initialized.");
        }

        private void OnDestroy()
        {
            if (avatarLoader != null)
            {
                avatarLoader.OnAvatarLoaded -= OnAvatarLoaded;
                avatarLoader.OnAvatarLoadFailed -= OnAvatarLoadFailed;
                avatarLoader.OnLoadProgress -= OnLoadProgress;
            }
        }

        private void OnAvatarLoaded(GameObject avatar, AvatarMetadata metadata)
        {
            Log($"Avatar loaded: {metadata.name}");
        }

        private void OnAvatarLoadFailed(string error)
        {
            Log($"Avatar load failed: {error}", LogType.Error);
        }

        private void OnLoadProgress(float progress)
        {
            Log($"Loading progress: {progress:P0}");
        }

        private void Log(string message, LogType type = LogType.Log)
        {
            if (!enableDebugLogs) return;

            switch (type)
            {
                case LogType.Error:
                    Debug.LogError($"[AvatarScape] {message}");
                    break;
                case LogType.Warning:
                    Debug.LogWarning($"[AvatarScape] {message}");
                    break;
                default:
                    Debug.Log($"[AvatarScape] {message}");
                    break;
            }
        }
    }
}
