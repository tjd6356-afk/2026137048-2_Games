using UnityEngine;
using System;

public class EmployeeMover : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Rotation")]
    [Tooltip("회전 속도(클수록 빨리 회전)")]
    public float rotationSpeed = 10f;

    [Header("Debug")]
    public bool drawTarget = true;

    private WaypointPath currentPath;
    private int currentPointIndex;

    public Action OnPathFinished;

    public bool IsMoving { get; private set; }

    void Update()
    {
        if (!IsMoving)
            return;

        Move();
        Rotate();
    }

    public void MoveAlongPath(WaypointPath path)
    {
        if (path == null)
            return;

        if (path.PointCount == 0)
            return;

        currentPath = path;
        currentPointIndex = 0;

        IsMoving = true;
    }

    private void Move()
    {
        Transform target = currentPath.GetPoint(currentPointIndex);

        if (target == null)
        {
            Stop();
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;

        // 이동
        transform.position += direction * moveSpeed * Time.deltaTime;

        float distance =
            Vector3.Distance(
                transform.position,
                target.position);

        if (distance < 0.05f)
        {
            currentPointIndex++;

            if (currentPointIndex >= currentPath.PointCount)
            {
                if (currentPath.loop)
                {
                    currentPointIndex = 0;
                }
                else
                {
                    Stop();
                }
            }
        }
    }

    private void Rotate()
    {
        Transform target = currentPath.GetPoint(currentPointIndex);

        if (target == null)
            return;

        Vector3 direction = target.position - transform.position;

        // 위아래 회전 방지
        direction.y = 0f;

        // 너무 가까우면 회전하지 않음
        if (direction.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime);
    }

    public void Stop()
    {
        IsMoving = false;

        OnPathFinished?.Invoke();
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawTarget)
            return;

        if (currentPath == null)
            return;

        if (!IsMoving)
            return;

        Transform target = currentPath.GetPoint(currentPointIndex);

        if (target == null)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawLine(
            transform.position,
            target.position);
    }
}