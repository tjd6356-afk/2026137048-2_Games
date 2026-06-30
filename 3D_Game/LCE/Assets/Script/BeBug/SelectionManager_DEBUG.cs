//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.UI;

//// 좌클릭으로 직원 단일 선택, 좌클릭 드래그로 범위 내 다중 선택을 처리합니다.
//// 새로운 Input System 기준이며, 메인 카메라(Camera.main)를 사용해 화면 좌표를 계산합니다.
//public class SelectionManager : MonoBehaviour
//{
//    [Header("드래그 판정")]
//    [Tooltip("이 거리(픽셀) 이상 움직여야 드래그로 인식합니다. 그 이하면 단일 클릭으로 처리됩니다.")]
//    public float dragThreshold = 5f;

//    [Header("드래그 박스 UI")]
//    [Tooltip("드래그 중 화면에 표시할 박스 UI의 RectTransform (Canvas 자식). 처음엔 비활성화 상태여야 합니다.")]
//    public RectTransform selectionBoxUI;

//    [Header("선택 옵션")]
//    [Tooltip("켜두면 Shift를 누른 상태로 클릭/드래그 시 기존 선택에 추가합니다. 꺼두면 항상 새로 선택합니다.")]
//    public bool allowShiftAddToSelection = true;

//    // 다른 스크립트(PlayerCameraSmoothed 등)에서 드래그 여부를 읽을 수 있도록 공개합니다.
//    public static bool IsDragging { get; private set; } = false;

//    private Camera mainCamera;
//    private Vector2 dragStartScreenPos;
//    private bool isDragging = false;
//    private bool mouseHeld = false;

//    private readonly List<Selectable> currentlySelected = new List<Selectable>();

//    void Start()
//    {
//        mainCamera = Camera.main;

//        if (selectionBoxUI != null)
//        {
//            selectionBoxUI.gameObject.SetActive(false);
//        }
//    }

//    void Update()
//    {
//        if (Mouse.current == null) return;

//        // 좌클릭이 눌린 순간
//        if (Mouse.current.leftButton.wasPressedThisFrame)
//        {
//            mouseHeld = true;
//            isDragging = false;
//            dragStartScreenPos = Mouse.current.position.ReadValue();
//        }

//        // 좌클릭을 누르고 있는 동안
//        if (mouseHeld)
//        {
//            Vector2 currentScreenPos = Mouse.current.position.ReadValue();
//            float dragDistance = Vector2.Distance(dragStartScreenPos, currentScreenPos);

//            // 일정 거리 이상 움직이면 드래그로 인식 시작
//            if (!isDragging && dragDistance >= dragThreshold)
//            {
//                isDragging = true;
//                IsDragging = true; // 카메라 등 외부 스크립트에 드래그 시작 알림

//                if (selectionBoxUI != null)
//                {
//                    selectionBoxUI.gameObject.SetActive(true);
//                }
//            }

//            if (isDragging)
//            {
//                UpdateSelectionBoxUI(dragStartScreenPos, currentScreenPos);
//            }
//        }

//        // 좌클릭을 뗀 순간 -> 실제 선택 처리
//        if (Mouse.current.leftButton.wasReleasedThisFrame)
//        {
//            Vector2 currentScreenPos = Mouse.current.position.ReadValue();
//            bool additive = allowShiftAddToSelection &&
//                             Keyboard.current != null &&
//                             (Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed);

//            if (isDragging)
//            {
//                // 드래그 박스 범위 안의 모든 직원 선택
//                SelectInBox(dragStartScreenPos, currentScreenPos, additive);
//            }
//            else
//            {
//                // 단일 클릭 선택 (레이캐스트)
//                SelectSingleAtScreenPos(currentScreenPos, additive);
//            }

//            // 상태 초기화
//            mouseHeld = false;
//            isDragging = false;
//            IsDragging = false; // 카메라 등 외부 스크립트에 드래그 종료 알림

//            if (selectionBoxUI != null)
//            {
//                selectionBoxUI.gameObject.SetActive(false);
//            }
//        }
//    }

//    // 드래그 박스 UI(RectTransform)의 위치와 크기를 두 화면 좌표 기준으로 갱신합니다.
//    void UpdateSelectionBoxUI(Vector2 start, Vector2 end)
//    {
//        if (selectionBoxUI == null) return;

//        Vector2 boxMin = Vector2.Min(start, end);
//        Vector2 boxMax = Vector2.Max(start, end);

//        // 박스의 중심과 크기를 계산해 anchoredPosition / sizeDelta로 설정합니다.
//        // 이 방식은 selectionBoxUI의 Canvas가 Screen Space - Overlay 또는
//        // Screen Space - Camera 모드일 때 잘 동작합니다.
//        Vector2 size = boxMax - boxMin;
//        Vector2 center = boxMin + size * 0.5f;

//        selectionBoxUI.position = center;
//        selectionBoxUI.sizeDelta = size;
//    }

//    // 단일 클릭 지점에서 레이캐스트로 직원 1명만 선택합니다.
//    void SelectSingleAtScreenPos(Vector2 screenPos, bool additive)
//    {
//        if (mainCamera == null)
//        {
//            mainCamera = Camera.main;
//            if (mainCamera == null) return;
//        }

//        if (!additive)
//        {
//            ClearSelection();
//        }

//        Ray ray = mainCamera.ScreenPointToRay(screenPos);
//        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
//        {
//            Selectable selectable = hit.collider.GetComponentInParent<Selectable>();
//            if (selectable != null)
//            {
//                if (additive && selectable.IsSelected)
//                {
//                    // Shift + 이미 선택된 대상 클릭 -> 선택 해제 (토글)
//                    Deselect(selectable);
//                }
//                else
//                {
//                    Select(selectable);
//                }
//                return;
//            }
//        }

//        // 빈 공간 클릭 시(additive가 아니면) 이미 위에서 ClearSelection 처리됨
//    }

//    // 드래그 박스(화면 좌표 두 점) 범위 안에 들어오는 모든 Selectable을 찾아 선택합니다.
//    void SelectInBox(Vector2 start, Vector2 end, bool additive)
//    {
//        if (mainCamera == null)
//        {
//            mainCamera = Camera.main;
//            if (mainCamera == null) return;
//        }

//        if (!additive)
//        {
//            ClearSelection();
//        }

//        Vector2 boxMin = Vector2.Min(start, end);
//        Vector2 boxMax = Vector2.Max(start, end);

//        // 씬에 존재하는 모든 Selectable을 검사합니다.
//        // 직원 수가 매우 많아질 경우(수백~수천), 별도의 등록 리스트(static List)로
//        // 관리하도록 최적화할 수 있습니다.
//        Selectable[] allSelectables = FindObjectsOfType<Selectable>();
//        Debug.Log($"[디버그] 씬에서 찾은 Selectable 개수: {allSelectables.Length}, 박스 범위: min={boxMin}, max={boxMax}");

//        foreach (Selectable selectable in allSelectables)
//        {
//            Vector3 worldPos = selectable.transform.position;
//            Vector3 screenPoint = mainCamera.WorldToScreenPoint(worldPos);

//            // 카메라 뒤쪽(화면 밖)에 있는 대상은 제외
//            if (screenPoint.z < 0f)
//            {
//                Debug.Log($"[디버그] {selectable.name} 은 카메라 뒤쪽(z={screenPoint.z})이라 제외됨");
//                continue;
//            }

//            bool insideBox = screenPoint.x >= boxMin.x && screenPoint.x <= boxMax.x &&
//                              screenPoint.y >= boxMin.y && screenPoint.y <= boxMax.y;

//            Debug.Log($"[디버그] {selectable.name} screenPoint={screenPoint}, insideBox={insideBox}");

//            if (insideBox)
//            {
//                Select(selectable);
//            }
//        }
//    }

//    void Select(Selectable selectable)
//    {
//        Debug.Log($"[디버그] Select() 호출됨: {selectable.name}, selectionIndicator={(selectable.selectionIndicator != null ? selectable.selectionIndicator.name : "NULL!!")}");

//        if (selectable.IsSelected) return;

//        selectable.SetSelected(true);
//        currentlySelected.Add(selectable);
//    }

//    void Deselect(Selectable selectable)
//    {
//        selectable.SetSelected(false);
//        currentlySelected.Remove(selectable);
//    }

//    void ClearSelection()
//    {
//        foreach (Selectable selectable in currentlySelected)
//        {
//            if (selectable != null)
//            {
//                selectable.SetSelected(false);
//            }
//        }
//        currentlySelected.Clear();
//    }

//    // 외부(다른 스크립트, UI 등)에서 현재 선택된 직원 목록을 참조할 때 사용합니다.
//    public List<Selectable> GetSelected()
//    {
//        return currentlySelected;
//    }
//}
