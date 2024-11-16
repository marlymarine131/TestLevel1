using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Point pointA, pointB;
    public float length;
    private LineRenderer lineRenderer;
    [SerializeField] private int curveResolution = 50; // Number of points for the curve
    [SerializeField] private float curveOffset = 0.5f;   // Offset distance for the curve

    // Khởi tạo đoạn đường giữa hai điểm
    public void Initialize(Point start, Point end)
    {
        pointA = start;
        pointB = end;
        length = Vector3.Distance(pointA.transform.position, pointB.transform.position);
        lineRenderer = GetComponent<LineRenderer>();

        DrawCurvedLine();

        pointA.Connect(pointB); // Kết nối hai điểm

        SetLineWidth(0.2f);
    }
    public List<Vector3> GetCurvePoints()
    {
        // Lấy các điểm trên đường cong
        Vector3 startPos = pointA.transform.position;
        Vector3 endPos = pointB.transform.position;

        // Tính toán các điểm điều khiển
        Vector3 direction = (endPos - startPos).normalized;
        Vector3 perpendicular = new Vector3(-direction.z, 0, direction.x); // Vectơ vuông góc với mặt đất

        Vector3 controlPointA = startPos + direction * 0.2f * length + perpendicular * curveOffset;
        Vector3 controlPointB = endPos - direction * 0.2f * length + perpendicular * curveOffset;

        // Tính toán đường cong Bezier
        return new List<Vector3>(CalculateBezierCurve(startPos, controlPointA, controlPointB, endPos, curveResolution));
    }

    // Vẽ đường cong giữa hai điểm
    private void DrawCurvedLine()
    {
        if (lineRenderer == null) return;

        Vector3 startPos = pointA.transform.position;
        Vector3 endPos = pointB.transform.position;

        // Calculate control points near the endpoints
        Vector3 direction = (endPos - startPos).normalized;
        Vector3 perpendicular = new Vector3(-direction.z, 0, direction.x); // Perpendicular vector on XZ plane

        Vector3 controlPointA = startPos + direction * 0.2f * length + perpendicular * curveOffset;
        Vector3 controlPointB = endPos - direction * 0.2f * length + perpendicular * curveOffset;

        // Generate points for the curve
        Vector3[] curvePoints = CalculateBezierCurve(startPos, controlPointA, controlPointB, endPos, curveResolution);

        lineRenderer.positionCount = curvePoints.Length;
        lineRenderer.SetPositions(curvePoints);
    }

    // Cài đặt độ rộng của đoạn đường
    public void SetLineWidth(float width)
    {
        if (lineRenderer != null)
        {
            lineRenderer.startWidth = width;
            lineRenderer.endWidth = width;
        }
    }

    // Làm nổi bật đoạn đường
    public void HighlightPath(Color color)
    {
        if (lineRenderer != null)
        {
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;
        }
    }

    // Kiểm tra nếu đoạn đường chứa một điểm nào đó
    public bool ContainsPoint(Point point)
    {
        return pointA == point || pointB == point;
    }
    public bool ContainPointBetween(Point from, Point to)
    {
        return (from == pointA && to == pointB) || (from == pointB && to == pointA); 
    }

    // Lấy điểm còn lại khi cho một điểm
    public Point GetOtherPoint(Point point)
    {
        return point == pointA ? pointB : pointA;
    }

    // Lấy vị trí mục tiêu (Điểm kết thúc dựa vào vị trí hiện tại)
    public Vector3 GetTargetPosition(Vector3 currentPosition)
    {
        // Kiểm tra điểm gần nhất và di chuyển về phía điểm còn lại
        float distanceToA = Vector3.Distance(currentPosition, pointA.transform.position);
        float distanceToB = Vector3.Distance(currentPosition, pointB.transform.position);

        // Nếu gần pointA hơn, di chuyển đến pointB, ngược lại.
        if (distanceToA < distanceToB)
        {
            return pointB.transform.position;
        }
        else
        {
            return pointA.transform.position;
        }
    }

    public void ResetColor()
    {
        if (lineRenderer != null)
        {
            lineRenderer.startColor = Color.white;
            lineRenderer.endColor = Color.white;
        }
    }

    // Tính toán đường cong Bezier bậc ba
    private Vector3[] CalculateBezierCurve(Vector3 start, Vector3 controlA, Vector3 controlB, Vector3 end, int resolution)
    {
        Vector3[] points = new Vector3[resolution + 1];
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            points[i] = Mathf.Pow(1 - t, 3) * start +
                        3 * Mathf.Pow(1 - t, 2) * t * controlA +
                        3 * (1 - t) * Mathf.Pow(t, 2) * controlB +
                        Mathf.Pow(t, 3) * end;
        }
        return points;
    }
}
