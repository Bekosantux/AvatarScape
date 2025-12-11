using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace AvatarScape.WebGL
{
    /// <summary>
    /// WebGL用のファイル読み込みブリッジ
    /// JavaScript側の関数を呼び出す
    /// </summary>
    public class WebGLFileBridge : MonoBehaviour
    {
        public static WebGLFileBridge Instance { get; private set; }

#if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void OpenFileDialog(string gameObjectName, string methodName, string accept);

        [DllImport("__Internal")]
        private static extern void SetupDragAndDrop(string gameObjectName);

        [DllImport("__Internal")]
        private static extern void DownloadFile(string filename, string base64Data);
#endif

        /// <summary>
        /// ファイルが読み込まれたときのイベント
        /// </summary>
        public event Action<byte[]> OnFileLoaded;

        /// <summary>
        /// ファイル読み込みエラー時のイベント
        /// </summary>
        public event Action<string> OnFileError;

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
        }

        private void Start()
        {
            InitializeDragAndDrop();
        }

        /// <summary>
        /// ドラッグ&ドロップを初期化
        /// </summary>
        public void InitializeDragAndDrop()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            SetupDragAndDrop(gameObject.name);
            Debug.Log("[WebGLFileBridge] Drag and drop initialized");
#else
            Debug.Log("[WebGLFileBridge] Drag and drop is WebGL only");
#endif
        }

        /// <summary>
        /// ファイル選択ダイアログを開く
        /// </summary>
        /// <param name="accept">受け入れるファイル拡張子 (例: ".unity3d")</param>
        public void OpenFileSelector(string accept = ".unity3d,.assetbundle")
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            OpenFileDialog(gameObject.name, "OnFileSelected", accept);
#else
            Debug.LogWarning("[WebGLFileBridge] OpenFileSelector is WebGL only. Use UnityEditor's file browser in editor.");
#endif
        }

        /// <summary>
        /// ファイルをダウンロード
        /// </summary>
        public void SaveFile(string filename, byte[] data)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            string base64 = Convert.ToBase64String(data);
            DownloadFile(filename, base64);
#else
            Debug.LogWarning("[WebGLFileBridge] SaveFile is WebGL only.");
#endif
        }

        /// <summary>
        /// JavaScript側からのコールバック: ファイル選択時
        /// </summary>
        public void OnFileSelected(string base64Data)
        {
            ProcessBase64File(base64Data);
        }

        /// <summary>
        /// JavaScript側からのコールバック: ファイルドロップ時
        /// </summary>
        public void OnFileDropped(string base64Data)
        {
            ProcessBase64File(base64Data);
        }

        /// <summary>
        /// JavaScript側からのコールバック: エラー時
        /// </summary>
        public void OnFileLoadError(string error)
        {
            Debug.LogError($"[WebGLFileBridge] File load error: {error}");
            OnFileError?.Invoke(error);
        }

        private void ProcessBase64File(string base64Data)
        {
            try
            {
                byte[] data = Convert.FromBase64String(base64Data);
                Debug.Log($"[WebGLFileBridge] File loaded: {data.Length} bytes");
                OnFileLoaded?.Invoke(data);
            }
            catch (Exception e)
            {
                Debug.LogError($"[WebGLFileBridge] Failed to process file: {e.Message}");
                OnFileError?.Invoke(e.Message);
            }
        }
    }
}
