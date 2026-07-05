using UnityEngine;

public class PathTester : MonoBehaviour
{
    public EmployeeMover mover;

    public WaypointPath path;

    void Start()
    {
        mover.MoveAlongPath(path);
    }
}