using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Point pointA, pointB;
    public float length;
    private LineRenderer lineRenderer;
    [SerializeField] public int curveResolution = 20; // Number of points on the curve
    [SerializeField] private float curveOffset = 1f;
    [SerializeField] private int segmentCount = 50;
    [SerializeField] private int density = 50;

    public List<Vector3> points = new List<Vector3>(); // List to store points on the path
    private bool isCurved;
    public float curvatureFactor = 1f;

    // Initialize path between two points
    public void Initialize(Point start, Point end,MeshGenerator meshGenerator)
    {
        pointA = start;
        pointB = end;
        // Sinh các điểm trên đoạn thẳng
        points = new List<Vector3>();
        for (int i = 0; i <= segmentCount; i++)
        {
            float t = i / (float)segmentCount;
            Vector3 point = Vector3.Lerp(start.transform.position, end.transform.position, t);
            points.Add(point);
        }

        // Chiếu các điểm lên bề mặt Mesh
        if (meshGenerator != null && meshGenerator.gameObject.activeSelf)
        {
            points = meshGenerator.ProjectPointsOntoMesh(points);
        }

        // Cập nhật LineRenderer
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
        pointA.Connect(pointB);
        length = CalculateTotalLength(points);
        SetLineWidth(0.2f);
        
    }
    public List<Vector3> GetAllPointOfPath()
    {
        return points;
    }
    // Calculate the total length of the path
    private float CalculateTotalLength(List<Vector3> points)
    {
        Debug.Log(points.Count);
        float totalLength = 0f;
        for (int i = 1; i < points.Count; i++) // Start from the second point
        {
            totalLength += Vector3.Distance(points[i - 1], points[i]);
        }
        return totalLength;
    }

    // Set the width of the path line
    public void SetLineWidth(float width)
    {
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }
    }

    // Highlight the path with a specified color
    public void HighlightPath(Color color)
    {
        if (lineRenderer != null)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }

    // Check if the path contains a specific point
    public bool ContainsPoint(Point point)
    {
        return pointA == point || pointB == point;
    }

    // Check if the path contains a point between two others
    public bool ContainPointBetween(Point from, Point to)
    {
        return (from == pointA && to == pointB) || (from == pointB && to == pointA);
    }

    // Reset the color of the path
    public void ResetColor()
    {
        if (lineRenderer != null)
        {
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
        }
    }
}
