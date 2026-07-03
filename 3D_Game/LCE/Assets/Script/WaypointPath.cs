using System.Collections.Generic;
using UnityEngine;

public class WaypointPath : MonoBehaviour
{
    [Header("Path Settings")]

    [Tooltip("직원이 이동할 속도입니다.")]
    public float moveSpeed = 3f;

    [Tooltip("마지막 Point에 도착하면 다시 처음부터 반복합니다.")]
    public bool loop = false;

    [Tooltip("Scene에서 경로를 표시합니다.")]
    public bool showPath = true;

    [Header("Way Points")]

    [SerializeField]
    private List<Transform> points = new List<Transform>();

    public int PointCount => points.Count;

    public Transform GetPoint(int index)
    {
        if (index < 0 || index >= points.Count)
            return null;

        return points[index];
    }

    public List<Transform> GetPoints()
    {
        return points;
    }

    public void AddPoint(Transform point)
    {
        if (!points.Contains(point))
            points.Add(point);
    }

    public void RemovePoint(Transform point)
    {
        if (points.Contains(point))
            points.Remove(point);
    }

    public void Clear()
    {
        points.Clear();
    }

    private void OnDrawGizmos()
    {
        if (!showPath)
            return;

        Gizmos.color = Color.green;

        for (int i = 0; i < points.Count; i++)
        {
            if (points[i] == null)
                continue;

            Gizmos.DrawSphere(points[i].position, 0.15f);

            if (i < points.Count - 1 && points[i + 1] != null)
            {
                Gizmos.DrawLine(
                    points[i].position,
                    points[i + 1].position);
            }
        }

        if (loop && points.Count > 1)
        {
            Gizmos.DrawLine(
                points[points.Count - 1].position,
                points[0].position);
        }
    }

    public void Reverse()
    {
        points.Reverse();
    }

    public void InsertPoint(int index, Transform point)
    {
        if (index < 0)
            index = 0;

        if (index > points.Count)
            index = points.Count;

        points.Insert(index, point);
    }

    public void MovePoint(int from, int to)
    {
        if (from < 0 || from >= points.Count)
            return;

        if (to < 0 || to >= points.Count)
            return;

        Transform temp = points[from];

        points.RemoveAt(from);

        points.Insert(to, temp);
    }
}