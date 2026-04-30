using UnityEngine;

public class CameraDrag : MonoBehaviour
{
    public float dragSpeed = 1f;

    private Vector3 lastMousePos;
    private bool isDragging;

    void Update()
    {
        if (!GameManager.Instance.camCanDrag)
        {
            return;
        }
        // ===== 鼠标 =====
        HandleMouse();

        // ===== 触屏 =====
        HandleTouch();
    }

    // =========================
    // 鼠标拖动
    // =========================
    void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging)
        {
            Vector3 delta = Input.mousePosition - lastMousePos;

            MoveCamera(delta);

            lastMousePos = Input.mousePosition;
        }
    }

    // =========================
    // 触屏拖动
    // =========================
    void HandleTouch()
    {
        if (Input.touchCount == 0) return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            lastMousePos = touch.position;
        }
        else if (touch.phase == TouchPhase.Moved)
        {
            Vector3 delta = (Vector3)touch.position - lastMousePos;

            MoveCamera(delta);

            lastMousePos = touch.position;
        }
    }

    // =========================
    // 移动相机
    // =========================
    void MoveCamera(Vector3 delta)
    {
        // 让拖动更自然（跟相机缩放无关）
        Vector3 move = new Vector3(-delta.x, -delta.y, 0) * dragSpeed * Time.deltaTime;

        transform.Translate(move, Space.World);
    }
}