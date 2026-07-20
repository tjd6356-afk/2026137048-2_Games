using System.Collections.Generic;
using UnityEngine;

// Canvas 안의 "WorkingSelectPanel" 오브젝트에 부착합니다.
// WorkingClickable이 working 오브젝트 클릭을 감지하면 이 패널의 Open()을 호출해 패널을 띄웁니다.
// 패널 안의 각 버튼(WorkButton, WHITEWorkButton, BLACKWorkButton, PALEWorkButton 등)의
// OnClick 이벤트에 OnAnyWorkButtonClicked()를 연결하면, 어떤 버튼을 누르든
// 선택되어 있던 직원들이 working 위치로 이동하고 패널이 닫힙니다.
public class WorkingSelectPanelUI : MonoBehaviour
{
    [Header("이동 목표 지점")]
    [Tooltip("직원이 작업하러 이동할 실제 지점입니다. 비워두면 working 오브젝트 자신의 Transform 위치를 사용합니다. working 자식에 빈 오브젝트(예: WorkSlot)를 만들어 지정하면 더 정확한 위치로 보낼 수 있습니다.")]
    public Transform workDestinationOverride;

    [Tooltip("여러 직원이 함께 작업하러 갈 때 한 지점에 겹치지 않도록 살짝 흩어지게 할 반경입니다.")]
    public float scatterRadius = 0.8f;

    private WorkingClickable currentWorking;
    private List<Selectable> pendingEmployees = new List<Selectable>();

    void Awake()
    {
        // 시작할 때는 항상 닫힌 상태로 둡니다.
        gameObject.SetActive(false);
    }

    // WorkingClickable에서 호출합니다. 패널을 열고, 어떤 직원들에게 작업 명령을 내릴지 기록해둡니다.
    public void Open(WorkingClickable working, List<Selectable> employees)
    {
        currentWorking = working;

        pendingEmployees.Clear();
        pendingEmployees.AddRange(employees);

        gameObject.SetActive(true);
    }

    public void AssignRed()
    {
        Assign(WorkType.Red);
    }

    public void AssignWhite()
    {
        Assign(WorkType.White);
    }

    public void AssignBlack()
    {
        Assign(WorkType.Black);
    }

    public void AssignPale()
    {
        Assign(WorkType.Pale);
    }

    private void Assign(WorkType type)
    {
        if (currentWorking == null)
            return;

        

        ClosePanel();
    }

    // 패널 바깥(취소 등)을 닫을 때 사용할 수 있는 메서드입니다. 필요 시 닫기 버튼에 연결하세요.
    public void ClosePanel()
    {
        currentWorking = null;
        pendingEmployees.Clear();
        gameObject.SetActive(false);
    }

    //void MoveEmployeesToWorking() 망한거.
    //{
    //    if (currentWorking == null || pendingEmployees.Count == 0) return;

    //    Transform destinationTransform = workDestinationOverride != null
    //        ? workDestinationOverride
    //        : currentWorking.transform;

    //    Vector3 basePoint = destinationTransform.position;
    //    int total = pendingEmployees.Count;
    //    int index = 0;

    //    foreach (Selectable employee in pendingEmployees)
    //    {
    //        if (employee == null) continue;

    //        Vector3 destination = basePoint;

    //        // 여러 명이 함께 작업 장소로 갈 때 겹치지 않도록 원형으로 살짝 흩어줍니다.
    //        if (total > 1 && scatterRadius > 0f)
    //        {
    //            float angle = (360f / total) * index * Mathf.Deg2Rad;
    //            Vector3 offset = new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * scatterRadius;
    //            destination = basePoint + offset;
    //        }

    //        employee.MoveToWorkplace(destination, currentWorking.transform);
    //        index++;
    //    }
    //}
}
