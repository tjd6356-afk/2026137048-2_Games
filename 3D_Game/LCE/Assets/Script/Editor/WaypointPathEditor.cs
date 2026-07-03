using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WaypointPath))]
public class WaypointPathEditor : Editor
{
    private WaypointPath path;

    private void OnEnable()
    {
        path = (WaypointPath)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Waypoint Tools", EditorStyles.boldLabel);

        if (GUILayout.Button("Add Point"))
        {
            AddPoint();
        }

        if (GUILayout.Button("Remove Last Point"))
        {
            RemoveLastPoint();
        }

        if (GUILayout.Button("Auto Refresh"))
        {
            RefreshPoints();
        }

        if (GUILayout.Button("Clear List"))
        {
            path.Clear();

            EditorUtility.SetDirty(path);
        }
    }

    private void AddPoint()
    {
        Undo.RegisterCompleteObjectUndo(path.gameObject, "Add Point");

        GameObject point = new GameObject();

        point.name = "Point" + path.PointCount;

        point.transform.parent = path.transform;

        if (path.PointCount == 0)
        {
            point.transform.localPosition = Vector3.zero;
        }
        else
        {
            Transform last = path.GetPoint(path.PointCount - 1);

            point.transform.position =
                last.position + Vector3.forward * 2f;
        }

        path.AddPoint(point.transform);

        EditorUtility.SetDirty(path);
    }

    private void RemoveLastPoint()
    {
        if (path.PointCount == 0)
            return;

        Transform last = path.GetPoint(path.PointCount - 1);

        path.RemovePoint(last);

        Undo.DestroyObjectImmediate(last.gameObject);

        EditorUtility.SetDirty(path);
    }

    private void RefreshPoints()
    {
        path.Clear();

        foreach (Transform child in path.transform)
        {
            path.AddPoint(child);
        }

        EditorUtility.SetDirty(path);
    }

    private void OnSceneGUI()
    {
        if (path == null)
            return;

        GUIStyle style = new GUIStyle(EditorStyles.boldLabel);
        style.normal.textColor = Color.yellow;
        style.fontSize = 14;

        for (int i = 0; i < path.PointCount; i++)
        {
            Transform point = path.GetPoint(i);

            if (point == null)
                continue;

            // 번호 표시
            Handles.Label(
                point.position + Vector3.up * 0.5f,
                $"{i}",
                style);

            // 이동 핸들
            EditorGUI.BeginChangeCheck();

            Vector3 newPos =
                Handles.PositionHandle(
                    point.position,
                    Quaternion.identity);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(point, "Move Waypoint");

                point.position = newPos;

                EditorUtility.SetDirty(point);
                EditorUtility.SetDirty(path);
            }
        }
    }
}