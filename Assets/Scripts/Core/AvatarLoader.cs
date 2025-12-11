using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace AvatarScape.Core
{
    /// <summary>
    /// AssetBundleからアバターを読み込むローダー
    /// </summary>
    public class AvatarLoader : MonoBehaviour
    {
        public static AvatarLoader Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private Transform avatarSpawnPoint;

        /// <summary>
        /// 現在読み込まれているアバター
        /// </summary>
        public GameObject CurrentAvatar { get; private set; }

        /// <summary>
        /// 現在のアバターのメタデータ
        /// </summary>
        public AvatarMetadata CurrentMetadata { get; private set; }

        /// <summary>
        /// 読み込み中かどうか
        /// </summary>
        public bool IsLoading { get; private set; }

        /// <summary>
        /// アバター読み込み完了時のイベント
        /// </summary>
        public event Action<GameObject, AvatarMetadata> OnAvatarLoaded;

        /// <summary>
        /// アバター読み込み失敗時のイベント
        /// </summary>
        public event Action<string> OnAvatarLoadFailed;

        /// <summary>
        /// 読み込み進捗更新時のイベント (0.0 - 1.0)
        /// </summary>
        public event Action<float> OnLoadProgress;

        private AssetBundle currentBundle;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            UnloadCurrentAvatar();
        }

        /// <summary>
        /// URLからAssetBundleを読み込む
        /// </summary>
        public void LoadFromUrl(string url)
        {
            if (IsLoading)
            {
                Debug.LogWarning("[AvatarLoader] Already loading an avatar.");
                return;
            }

            StartCoroutine(LoadFromUrlCoroutine(url));
        }

        /// <summary>
        /// バイト配列からAssetBundleを読み込む
        /// </summary>
        public void LoadFromBytes(byte[] data)
        {
            if (IsLoading)
            {
                Debug.LogWarning("[AvatarLoader] Already loading an avatar.");
                return;
            }

            StartCoroutine(LoadFromBytesCoroutine(data));
        }

        private IEnumerator LoadFromUrlCoroutine(string url)
        {
            IsLoading = true;
            OnLoadProgress?.Invoke(0f);

            Debug.Log($"[AvatarLoader] Loading from URL: {url}");

            using (var request = UnityWebRequestAssetBundle.GetAssetBundle(url))
            {
                var operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    OnLoadProgress?.Invoke(operation.progress * 0.5f);
                    yield return null;
                }

                if (request.result != UnityWebRequest.Result.Success)
                {
                    HandleLoadError($"Failed to download AssetBundle: {request.error}");
                    yield break;
                }

                var bundle = DownloadHandlerAssetBundle.GetContent(request);
                yield return StartCoroutine(ProcessBundle(bundle));
            }
        }

        private IEnumerator LoadFromBytesCoroutine(byte[] data)
        {
            IsLoading = true;
            OnLoadProgress?.Invoke(0f);

            Debug.Log($"[AvatarLoader] Loading from bytes: {data.Length} bytes");

            var bundleRequest = AssetBundle.LoadFromMemoryAsync(data);

            while (!bundleRequest.isDone)
            {
                OnLoadProgress?.Invoke(bundleRequest.progress * 0.5f);
                yield return null;
            }

            if (bundleRequest.assetBundle == null)
            {
                HandleLoadError("Failed to load AssetBundle from memory.");
                yield break;
            }

            yield return StartCoroutine(ProcessBundle(bundleRequest.assetBundle));
        }

        private IEnumerator ProcessBundle(AssetBundle bundle)
        {
            OnLoadProgress?.Invoke(0.5f);

            // 古いアバターをアンロード
            UnloadCurrentAvatar();

            currentBundle = bundle;

            // メタデータを読み込む
            AvatarMetadata metadata = null;
            var metaRequest = bundle.LoadAssetAsync<TextAsset>("avatar_meta");
            yield return metaRequest;

            if (metaRequest.asset != null)
            {
                var metaText = metaRequest.asset as TextAsset;
                metadata = AvatarMetadata.FromJson(metaText.text);
                Debug.Log($"[AvatarLoader] Loaded metadata: {metadata.name} by {metadata.author}");
            }
            else
            {
                Debug.LogWarning("[AvatarLoader] No metadata found in bundle, using defaults.");
                metadata = new AvatarMetadata();
            }

            OnLoadProgress?.Invoke(0.7f);

            // アバタープレハブを読み込む
            var allAssetNames = bundle.GetAllAssetNames();
            string prefabPath = null;

            foreach (var assetName in allAssetNames)
            {
                if (assetName.EndsWith(".prefab"))
                {
                    prefabPath = assetName;
                    break;
                }
            }

            if (string.IsNullOrEmpty(prefabPath))
            {
                HandleLoadError("No prefab found in AssetBundle.");
                yield break;
            }

            var prefabRequest = bundle.LoadAssetAsync<GameObject>(prefabPath);
            yield return prefabRequest;

            if (prefabRequest.asset == null)
            {
                HandleLoadError("Failed to load avatar prefab.");
                yield break;
            }

            OnLoadProgress?.Invoke(0.9f);

            // アバターをインスタンス化
            var prefab = prefabRequest.asset as GameObject;
            var spawnPosition = avatarSpawnPoint != null ? avatarSpawnPoint.position : Vector3.zero;
            var spawnRotation = avatarSpawnPoint != null ? avatarSpawnPoint.rotation : Quaternion.identity;

            CurrentAvatar = Instantiate(prefab, spawnPosition, spawnRotation);
            CurrentAvatar.name = string.IsNullOrEmpty(metadata.name) ? "Avatar" : metadata.name;
            CurrentMetadata = metadata;

            IsLoading = false;
            OnLoadProgress?.Invoke(1f);

            Debug.Log($"[AvatarLoader] Successfully loaded avatar: {CurrentAvatar.name}");
            OnAvatarLoaded?.Invoke(CurrentAvatar, CurrentMetadata);
        }

        private void HandleLoadError(string message)
        {
            Debug.LogError($"[AvatarLoader] {message}");
            IsLoading = false;
            OnAvatarLoadFailed?.Invoke(message);
        }

        /// <summary>
        /// 現在のアバターをアンロード
        /// </summary>
        public void UnloadCurrentAvatar()
        {
            if (CurrentAvatar != null)
            {
                Destroy(CurrentAvatar);
                CurrentAvatar = null;
            }

            if (currentBundle != null)
            {
                currentBundle.Unload(true);
                currentBundle = null;
            }

            CurrentMetadata = null;
        }
    }
}
