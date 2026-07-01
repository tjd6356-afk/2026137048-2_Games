using UnityEngine;
using UnityEngine.InputSystem;

// "working" 같은 작업 장소 오브젝트에 부착합니다.
// 선택된 직원이 있는 상태에서 이 오브젝트를 좌클릭하면 작업 배치 패널(WorkingSelectPanel)을 띄웁니다.
// 콜라이더가 반드시 필요합니다 (레이캐스트로 클릭을 감지하기 때문).
public class WorkingClickable : MonoBehaviour
{
    [Header("연결할 패널")]
    [Tooltip("이 working을 클릭했을 때 활성화할 패널 컴포넌트입니다. (WorkingSelectPanelUI)")]
    public WorkingSelectPanelUI targetPanel;

    [Header("참조 대상")]
    [Tooltip("선택된 직원의 이동 명령을 위해 필요한 SelectionManager입니다. 비워두면 씬에서 자동으로 찾습니다.")]
    public SelectionManager selectionManager;

    public Transform redPoint;
    public Transform whitePoint;
    public Transform blackPoint;
    public Transform palePoint;

    public Transform GetPoint(WorkType type)
    {
        switch (type)
        {
            case WorkType.Red:
                return redPoint;

            case WorkType.White:
                return whitePoint;

            case WorkType.Black:
                return blackPoint;

            case WorkType.Pale:
                return palePoint;
        }

        return transform;
    }

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        if (selectionManager == null)
        {
            selectionManager = FindObjectOfType<SelectionManager>();
        }
    }

    void Update()
    {
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasReleasedThisFrame) return;

        // 드래그 중이었다면(박스 선택) 이 클릭은 working 클릭으로 처리하지 않습니다.
        if (SelectionManager.IsDragging) return;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            // 이 working 오브젝트(자식 포함) 콜라이더를 클릭했는지 확인
            WorkingClickable clicked = hit.collider.GetComponentInParent<WorkingClickable>();
            if (clicked == this)
            {
                TryOpenPanel();
            }
        }
    }

    void TryOpenPanel()
    {
        if (selectionManager == null || targetPanel == null) return;

        var selected = selectionManager.GetSelected();

        // 선택된 직원이 없으면 패널을 열지 않습니다.
        if (selected == null || selected.Count == 0) return;

        targetPanel.Open(this, selected);
    }
}
