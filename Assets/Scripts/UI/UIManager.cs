using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AvatarScape.Core;
using AvatarScape.WebGL;

namespace AvatarScape.UI
{
    /// <summary>
    /// UIを管理するマネージャー
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private GameObject mainPanel;

        [Header("Loading UI")]
        [SerializeField] private Slider progressSlider;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("Main UI")]
        [SerializeField] private Button loadAvatarButton;
        [SerializeField] private TextMeshProUGUI avatarNameText;
        [SerializeField] private TextMeshProUGUI avatarInfoText;

        [Header("References")]
        [SerializeField] private AvatarLoader avatarLoader;
        [SerializeField] private WebGLFileBridge fileBridge;

        private void Start()
        {
            // 参照を自動取得
            if (avatarLoader == null)
                avatarLoader = FindObjectOfType<AvatarLoader>();
            if (fileBridge == null)
                fileBridge = FindObjectOfType<WebGLFileBridge>();

            // イベント登録
            if (avatarLoader != null)
            {
                avatarLoader.OnAvatarLoaded += OnAvatarLoaded;
                avatarLoader.OnAvatarLoadFailed += OnAvatarLoadFailed;
                avatarLoader.OnLoadProgress += OnLoadProgress;
            }

            if (fileBridge != null)
            {
                fileBridge.OnFileLoaded += OnFileLoaded;
                fileBridge.OnFileError += OnFileError;
            }

            // ボタンイベント
            if (loadAvatarButton != null)
            {
                loadAvatarButton.onClick.AddListener(OnLoadAvatarButtonClicked);
            }

            // 初期状態
            ShowMainPanel();
            UpdateAvatarInfo(null, null);
        }

        private void OnDestroy()
        {
            if (avatarLoader != null)
            {
                avatarLoader.OnAvatarLoaded -= OnAvatarLoaded;
                avatarLoader.OnAvatarLoadFailed -= OnAvatarLoadFailed;
                avatarLoader.OnLoadProgress -= OnLoadProgress;
            }

            if (fileBridge != null)
            {
                fileBridge.OnFileLoaded -= OnFileLoaded;
                fileBridge.OnFileError -= OnFileError;
            }
        }

        private void OnLoadAvatarButtonClicked()
        {
            if (fileBridge != null)
            {
                fileBridge.OpenFileSelector(".unity3d,.assetbundle");
            }
            else
            {
                Debug.LogWarning("[UIManager] WebGLFileBridge not found");
            }
        }

        private void OnFileLoaded(byte[] data)
        {
            ShowLoadingPanel("Loading Avatar...");

            if (avatarLoader != null)
            {
                avatarLoader.LoadFromBytes(data);
            }
        }

        private void OnFileError(string error)
        {
            ShowMainPanel();
            SetStatus($"Error: {error}");
        }

        private void OnAvatarLoaded(GameObject avatar, AvatarMetadata metadata)
        {
            ShowMainPanel();
            UpdateAvatarInfo(avatar, metadata);
            SetStatus("Avatar loaded successfully!");
        }

        private void OnAvatarLoadFailed(string error)
        {
            ShowMainPanel();
            SetStatus($"Failed to load: {error}");
        }

        private void OnLoadProgress(float progress)
        {
            if (progressSlider != null)
            {
                progressSlider.value = progress;
            }

            if (progressText != null)
            {
                progressText.text = $"{progress:P0}";
            }
        }

        private void ShowLoadingPanel(string message = "Loading...")
        {
            if (loadingPanel != null)
                loadingPanel.SetActive(true);
            if (mainPanel != null)
                mainPanel.SetActive(false);

            if (statusText != null)
                statusText.text = message;

            if (progressSlider != null)
                progressSlider.value = 0;
        }

        private void ShowMainPanel()
        {
            if (loadingPanel != null)
                loadingPanel.SetActive(false);
            if (mainPanel != null)
                mainPanel.SetActive(true);
        }

        private void UpdateAvatarInfo(GameObject avatar, AvatarMetadata metadata)
        {
            if (avatarNameText != null)
            {
                avatarNameText.text = metadata != null && !string.IsNullOrEmpty(metadata.name)
                    ? metadata.name
                    : "No Avatar Loaded";
            }

            if (avatarInfoText != null)
            {
                if (metadata != null)
                {
                    string info = "";
                    if (!string.IsNullOrEmpty(metadata.author))
                        info += $"Author: {metadata.author}\n";
                    if (!string.IsNullOrEmpty(metadata.version))
                        info += $"Version: {metadata.version}\n";
                    if (metadata.shaders != null && metadata.shaders.Length > 0)
                        info += $"Shaders: {string.Join(", ", metadata.shaders)}";

                    avatarInfoText.text = info;
                }
                else
                {
                    avatarInfoText.text = "Drop an avatar file or click 'Load Avatar'";
                }
            }
        }

        private void SetStatus(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
            Debug.Log($"[UIManager] {message}");
        }
    }
}
