using UnityEngine;

namespace AvatarScape.Camera
{
    /// <summary>
    /// シンプルなカメラコントローラー
    /// マウス操作でアバターの周りを回転
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 targetOffset = new Vector3(0, 1f, 0);

        [Header("Orbit Settings")]
        [SerializeField] private float distance = 3f;
        [SerializeField] private float minDistance = 1f;
        [SerializeField] private float maxDistance = 10f;
        [SerializeField] private float zoomSpeed = 2f;

        [Header("Rotation Settings")]
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float minVerticalAngle = -30f;
        [SerializeField] private float maxVerticalAngle = 80f;

        [Header("Pan Settings")]
        [SerializeField] private float panSpeed = 0.5f;

        private float currentHorizontalAngle = 0f;
        private float currentVerticalAngle = 20f;
        private Vector3 panOffset = Vector3.zero;

        private bool isDragging = false;
        private bool isPanning = false;
        private Vector3 lastMousePosition;

        private void Start()
        {
            UpdateCameraPosition();
        }

        private void Update()
        {
            HandleInput();
            UpdateCameraPosition();
        }

        private void HandleInput()
        {
            // マウスホイールでズーム
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                distance -= scroll * zoomSpeed;
                distance = Mathf.Clamp(distance, minDistance, maxDistance);
            }

            // 左クリック + ドラッグで回転
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }

            // 中クリック + ドラッグでパン
            if (Input.GetMouseButtonDown(2))
            {
                isPanning = true;
                lastMousePosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(2))
            {
                isPanning = false;
            }

            if (isDragging)
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                currentHorizontalAngle += delta.x * rotationSpeed * 0.1f;
                currentVerticalAngle -= delta.y * rotationSpeed * 0.1f;
                currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minVerticalAngle, maxVerticalAngle);
                lastMousePosition = Input.mousePosition;
            }

            if (isPanning)
            {
                Vector3 delta = Input.mousePosition - lastMousePosition;
                Vector3 right = transform.right;
                Vector3 up = transform.up;
                panOffset -= (right * delta.x + up * delta.y) * panSpeed * 0.01f;
                lastMousePosition = Input.mousePosition;
            }

            // ダブルクリックでリセット
            if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.R))
            {
                ResetCamera();
            }
        }

        private void UpdateCameraPosition()
        {
            // ターゲット位置を計算
            Vector3 targetPosition = target != null
                ? target.position + targetOffset
                : targetOffset;

            targetPosition += panOffset;

            // 球面座標からカメラ位置を計算
            float horizontalRad = currentHorizontalAngle * Mathf.Deg2Rad;
            float verticalRad = currentVerticalAngle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(
                Mathf.Sin(horizontalRad) * Mathf.Cos(verticalRad),
                Mathf.Sin(verticalRad),
                Mathf.Cos(horizontalRad) * Mathf.Cos(verticalRad)
            ) * distance;

            transform.position = targetPosition + offset;
            transform.LookAt(targetPosition);
        }

        /// <summary>
        /// カメラをリセット
        /// </summary>
        public void ResetCamera()
        {
            currentHorizontalAngle = 0f;
            currentVerticalAngle = 20f;
            distance = 3f;
            panOffset = Vector3.zero;
        }

        /// <summary>
        /// ターゲットを設定
        /// </summary>
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
            ResetCamera();
        }

        /// <summary>
        /// アバターの高さに基づいてオフセットを調整
        /// </summary>
        public void AdjustForAvatarHeight(float height)
        {
            targetOffset = new Vector3(0, height * 0.5f, 0);
            distance = height * 2f;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);
        }
    }
}
