using UnityEngine;
using AvatarScape.Core;

namespace AvatarScape.Lighting
{
    /// <summary>
    /// ライティングコントローラー
    /// アバターの照明を調整
    /// </summary>
    public class LightingController : MonoBehaviour
    {
        [Header("Main Light")]
        [SerializeField] private Light mainLight;
        [SerializeField] private float mainLightIntensity = 1f;
        [SerializeField] private Color mainLightColor = Color.white;

        [Header("Fill Light")]
        [SerializeField] private Light fillLight;
        [SerializeField] private float fillLightIntensity = 0.5f;
        [SerializeField] private Color fillLightColor = new Color(0.8f, 0.9f, 1f);

        [Header("Rim Light")]
        [SerializeField] private Light rimLight;
        [SerializeField] private float rimLightIntensity = 0.3f;
        [SerializeField] private Color rimLightColor = Color.white;

        [Header("Ambient")]
        [SerializeField] private Color ambientColor = new Color(0.2f, 0.2f, 0.25f);

        private void Start()
        {
            ApplyLighting();
        }

        /// <summary>
        /// 現在の設定を適用
        /// </summary>
        public void ApplyLighting()
        {
            // メインライト
            if (mainLight != null)
            {
                mainLight.intensity = mainLightIntensity;
                mainLight.color = mainLightColor;
            }

            // フィルライト
            if (fillLight != null)
            {
                fillLight.intensity = fillLightIntensity;
                fillLight.color = fillLightColor;
            }

            // リムライト
            if (rimLight != null)
            {
                rimLight.intensity = rimLightIntensity;
                rimLight.color = rimLightColor;
            }

            // アンビエント
            RenderSettings.ambientLight = ambientColor;
        }

        /// <summary>
        /// メインライトの角度を設定
        /// </summary>
        public void SetMainLightRotation(Vector2 angles)
        {
            if (mainLight != null)
            {
                mainLight.transform.rotation = Quaternion.Euler(angles.x, angles.y, 0);
            }
        }

        /// <summary>
        /// メインライトの強度を設定
        /// </summary>
        public void SetMainLightIntensity(float intensity)
        {
            mainLightIntensity = intensity;
            if (mainLight != null)
            {
                mainLight.intensity = intensity;
            }
        }

        /// <summary>
        /// メインライトの色を設定
        /// </summary>
        public void SetMainLightColor(Color color)
        {
            mainLightColor = color;
            if (mainLight != null)
            {
                mainLight.color = color;
            }
        }

        /// <summary>
        /// アンビエントカラーを設定
        /// </summary>
        public void SetAmbientColor(Color color)
        {
            ambientColor = color;
            RenderSettings.ambientLight = color;
        }

        /// <summary>
        /// プリセット: スタジオ照明
        /// </summary>
        public void ApplyStudioPreset()
        {
            mainLightIntensity = 1f;
            mainLightColor = Color.white;
            fillLightIntensity = 0.4f;
            fillLightColor = new Color(0.9f, 0.95f, 1f);
            rimLightIntensity = 0.3f;
            rimLightColor = Color.white;
            ambientColor = new Color(0.15f, 0.15f, 0.2f);

            if (mainLight != null)
                mainLight.transform.rotation = Quaternion.Euler(45, -45, 0);

            ApplyLighting();
        }

        /// <summary>
        /// プリセット: 屋外（昼）
        /// </summary>
        public void ApplyOutdoorDayPreset()
        {
            mainLightIntensity = 1.2f;
            mainLightColor = new Color(1f, 0.98f, 0.9f);
            fillLightIntensity = 0.6f;
            fillLightColor = new Color(0.7f, 0.85f, 1f);
            rimLightIntensity = 0.2f;
            rimLightColor = new Color(1f, 0.95f, 0.8f);
            ambientColor = new Color(0.3f, 0.35f, 0.4f);

            if (mainLight != null)
                mainLight.transform.rotation = Quaternion.Euler(50, -30, 0);

            ApplyLighting();
        }

        /// <summary>
        /// プリセット: 夕方
        /// </summary>
        public void ApplySunsetPreset()
        {
            mainLightIntensity = 0.9f;
            mainLightColor = new Color(1f, 0.6f, 0.3f);
            fillLightIntensity = 0.3f;
            fillLightColor = new Color(0.5f, 0.6f, 0.8f);
            rimLightIntensity = 0.4f;
            rimLightColor = new Color(1f, 0.7f, 0.4f);
            ambientColor = new Color(0.2f, 0.15f, 0.2f);

            if (mainLight != null)
                mainLight.transform.rotation = Quaternion.Euler(15, -60, 0);

            ApplyLighting();
        }
    }
}
