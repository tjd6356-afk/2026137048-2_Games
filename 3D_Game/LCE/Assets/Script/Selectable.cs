using UnityEngine;
//using UnityEngine.AI; // 실패 한 시스템 ㅠㅠ

// 선택 가능한 모든 오브젝트(직원 등)에 부착하는 컴포넌트입니다.
// SelectionManager는 이 컴포넌트가 붙어있는 오브젝트만 선택 대상으로 인식합니다.
// NavMeshAgent를 통한 이동 명령도 이 컴포넌트에서 처리합니다.
//[RequireComponent(typeof(NavMeshAgent))] // ㅠㅠㅠㅠㅠㅠㅠㅠ 실패한 시스템 ㅠㅠ
public class Selectable : MonoBehaviour
{
    [Header("선택 표시 오브젝트")]
    [Tooltip("선택되었을 때 활성화할 오브젝트 (예: 발밑 선택 링). 비워두면 자동으로 자식에서 'SelectionIndicator' 이름을 찾습니다.")]
    public GameObject selectionIndicator;

    public bool IsSelected { get; private set; } = false;

    // 현재 작업 중인 working 오브젝트 (작업 배치 시 사용, 없으면 null)
    public Transform CurrentWorkplace { get; private set; }

    private Vector3 startPosition;
    private Quaternion startRotation;

    //private NavMeshAgent agent;

    void Awake()
    {
        //agent = GetComponent<NavMeshAgent>();
        // 나중에 Transform 이동 코드가 들어갈 예정

        startPosition = transform.position;
        startRotation = transform.rotation;

        // 인스펙터에서 지정하지 않았다면 이름으로 자동 탐색 (선택 사항)
        if (selectionIndicator == null)
        {
            Transform found = transform.Find("SelectionIndicator");
            if (found != null)
            {
                selectionIndicator = found.gameObject;
            }
        }

        // 시작할 때는 선택 표시 꺼두기
        SetSelected(false);
    }

    // 선택 상태를 설정하고 표시 오브젝트를 갱신합니다.
    public void SetSelected(bool selected)
    {
        IsSelected = selected;

        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(selected);
        }
    }

    // 우클릭 등으로 임의의 지점으로 이동시킬 때 사용합니다.
    // 자유 이동이므로 기존에 하던 작업(working)은 해제됩니다.
    public void MoveTo(Vector3 destination)
    {
        CurrentWorkplace = null;

        // 나중에 Transform 이동 코드가 들어갈 예정
        // if (agent != null && agent.isOnNavMesh)
        // {
        //     agent.SetDestination(destination);
        // }
    }

    // working 오브젝트 등 특정 작업 위치로 이동시킬 때 사용합니다.
    // workplace를 기록해두면, 도착 후 다른 스크립트(예: 작업 처리 로직)에서 참조할 수 있습니다.
    public void MoveToWorkplace(Vector3 destination, Transform workplace)
    {
        CurrentWorkplace = workplace;

        // 나중에 Transform 이동 코드가 들어갈 예정
        //if (agent != null && agent.isOnNavMesh)
        //{
        //    agent.SetDestination(destination);
        //}
    }

    public WorkType CurrentWorkType { get; private set; }

    public void MoveToWorkplace(
        Vector3 destination,
        Transform workplace,
        WorkType workType)
    {
        CurrentWorkplace = workplace;
        CurrentWorkType = workType;

        // 나중에 Transform 이동 코드가 들어갈 예정
        //if (agent != null && agent.isOnNavMesh)
        //{
        //    agent.SetDestination(destination);
        //}
    }

    public void MoveToWorkplace(
    WaypointPath path,
    Transform workplace,
    WorkType workType)
    {
        CurrentWorkplace = workplace;
        CurrentWorkType = workType;

        EmployeeMover mover = GetComponent<EmployeeMover>();

        if (mover != null)
        {
            mover.MoveAlongPath(path);
        }
    }

    public void ReturnToStart()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
    }
}