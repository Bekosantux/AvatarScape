using System;
using UnityEngine;

namespace AvatarScape.Core
{
    /// <summary>
    /// アバターのメタデータ
    /// AssetBundle内のJSONから読み込まれる
    /// </summary>
    [Serializable]
    public class AvatarMetadata
    {
        public string version = "1.0.0";
        public string name = "";
        public string author = "";
        public string buildDate = "";
        public string unityVersion = "";
        public string[] shaders = Array.Empty<string>();
        public BoundsInfo bounds = new BoundsInfo();
        public HumanoidInfo humanoid = new HumanoidInfo();

        [Serializable]
        public class BoundsInfo
        {
            public float height = 1.6f;
            public float[] center = { 0f, 0.8f, 0f };

            public Vector3 CenterVector => new Vector3(center[0], center[1], center[2]);
        }

        [Serializable]
        public class HumanoidInfo
        {
            public bool isHumanoid = true;
        }

        /// <summary>
        /// JSONからメタデータをパース
        /// </summary>
        public static AvatarMetadata FromJson(string json)
        {
            try
            {
                return JsonUtility.FromJson<AvatarMetadata>(json);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[AvatarMetadata] Failed to parse metadata: {e.Message}");
                return new AvatarMetadata();
            }
        }

        /// <summary>
        /// メタデータをJSONに変換
        /// </summary>
        public string ToJson(bool prettyPrint = true)
        {
            return JsonUtility.ToJson(this, prettyPrint);
        }
    }
}
