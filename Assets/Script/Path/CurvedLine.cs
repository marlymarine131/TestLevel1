using UnityEngine;
using System.Collections.Generic;
using System;

public class CurvedLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    [SerializeField] private int segmentCount = 50;
    public List<Vector3> curvePositions = new List<Vector3>(); // Sử dụng List thay vì mảng

    public Path path1;
    public Path path2;

    public void Initialize(Vector3 point1, Vector3 point2, Vector3 point3, Path pathStart, Path pathEnd,MeshGenerator meshGenerator)
    {
        path1 = pathStart;
        path2 = pathEnd;
        DrawCurved(point1, point2, point3, pathStart, pathEnd, meshGenerator);
        SetLineWidth(0.2f);
        HighlightPath(Color.green);
    }

    public CurvedLine GetCurvedLine(Path path1, Path path2)
    {
        if (this.path1 == path1 || this.path1 == path2 || this.path2 == path1 || this.path2 == path2)
        {
            return this;
        }
        return null;
    }

    private void DrawCurved(Vector3 point1, Vector3 point2, Vector3 point3, Path pathStart, Path pathEnd, MeshGenerator meshGenerator)
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer is not assigned!");
            return;
        }

        curvePositions.Clear(); // Xóa danh sách cũ trước khi thêm mới
        for (int i = 0; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            curvePositions.Add(CalculateQuadraticBezierPoint(t, point1, point2, point3));
        }
        if (meshGenerator != null)
        {
            curvePositions = meshGenerator.ProjectPointsOntoMesh(curvePositions);
        }
        lineRenderer.positionCount = curvePositions.Count;
        lineRenderer.SetPositions(curvePositions.ToArray());
    }

    public List<Vector3> GetCurvePoints()
    {
        return curvePositions; // Trả về các điểm trên đường cong
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        return Mathf.Pow(1 - t, 2) * p1 +
               2 * (1 - t) * t * p2 +
               Mathf.Pow(t, 2) * p3;
    }

    public void SetLineWidth(float width)
    {
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }
    }

    public void HighlightPath(Color color)
    {
        if (lineRenderer != null)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }

    internal Vector3 GetStartPoint()
    {
        return curvePositions[0];
    }

    internal Vector3 GetEndPoint()
    {
        return curvePositions[curvePositions.Count - 1];
    }
}
