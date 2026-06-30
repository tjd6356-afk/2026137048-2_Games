using UnityEngine;
using UnityEngine.InputSystem; // 새로운 입력 시스템 사용

public class PlayerCameraSmoothed : MonoBehaviour
{
    [Header("이동 및 회전 속도")]
    public float moveSpeed = 10f;
    public float rotationSensitivity = 0.1f;
    [Header("회전 부드러움")]
    [Range(1f, 10f)]
    public float rotationSmoothness = 5f; // 값이 클수록 더 부드럽고 느리게 추적

    private float xRotation = 0f;
    private float yRotation = 0f;
    private Vector2 smoothedMouseDelta; // 부드러워진 마우스 델타 값
    private float verticalLookLimit = 85f; // 회전 제한 (약간 덜 타이트하게)

    void Start()
    {
        // 마우스 고정 및 숨기기
        //Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 1. 부드러운 마우스 회전 (새 Input System)
        RotateCameraSmoothed();

        // 2. WASD (수평 고정) + Shift/Space (수직) 이동 (새 Input System)
        MoveCameraCorrected();

        // ESC 키로 마우스 해제 (유지)
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }

    // [기능 1] 부드러운 마우스 회전 로직
    void RotateCameraSmoothed()
    {
        if (SelectionManager.IsDragging) return;

        if (Mouse.current == null) return;

        // 마우스의 움직임(Delta)을 읽어옵니다.
        Vector2 rawMouseDelta = Mouse.current.delta.ReadValue() * rotationSensitivity;

        // 보간을 사용하여 부드러운 마우스 움직임을 만듭니다. (부드러움의 핵심!)
        smoothedMouseDelta = Vector2.Lerp(smoothedMouseDelta, rawMouseDelta, rotationSmoothness * Time.deltaTime);

        // 보간된 값을 사용하여 회전 계산
        xRotation -= smoothedMouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, -verticalLookLimit, verticalLookLimit);
        yRotation += smoothedMouseDelta.x;

        // 부드러운 회전 적용
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }

    // [기능 2] WASD (Y축 고정) + Shift(하강)/Space(상승) 이동 로직
    void MoveCameraCorrected()
    {
        if (Keyboard.current == null) return;

        Vector3 moveDirection = Vector3.zero;

        // [핵심] 카메라가 바라보는 forward 방향에서 Y성분을 제거하여 항상 수평 평면상에서 이동하게 만듭니다.
        Vector3 forwardFlattened = transform.forward;
        forwardFlattened.y = 0f;
        forwardFlattened.Normalize(); // 크기를 1로 다시 정규화

        // WASD 수평 이동 계산
        if (Keyboard.current.wKey.isPressed) moveDirection += forwardFlattened;
        if (Keyboard.current.sKey.isPressed) moveDirection -= forwardFlattened;
        if (Keyboard.current.aKey.isPressed) moveDirection -= transform.right; // transform.right는 항상 수평이므로 그대로 사용
        if (Keyboard.current.dKey.isPressed) moveDirection += transform.right;

        // [요청] 키 변경: Space (상승), Ctrl (하강)
        if (Keyboard.current.spaceKey.isPressed) moveDirection += Vector3.up; // 항상 수직 위
        if (Keyboard.current.ctrlKey.isPressed) moveDirection -= Vector3.up; // 항상 수직 아래

        // 대각선 이동 속도 일정하게 정규화
        if (moveDirection != Vector3.zero)
        {
            transform.position += moveDirection.normalized * moveSpeed * Time.deltaTime;
        }
    }
}