using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Point pointA, pointB;
    public float length;
    private LineRenderer lineRenderer;
    [SerializeField] public int curveResolution = 20; // Số điểm trên đường cong
    [SerializeField] private float curveOffset = 1f;
    public List<Vector3> curvePoints = new List<Vector3>(); // Danh sách các điểm trên đường cong
    private bool isCurved;
    public float curvatureFactor = 1f;

    // Khởi tạo đường nối giữa hai điểm
    public void Initialize(Point start, Point end)
    {
        pointA = start;
        pointB = end;
        length = Vector3.Distance(pointA.transform.position, pointB.transform.position);
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(new Vector3[] { pointA.transform.position, pointB.transform.position });

        pointA.Connect(pointB); // Kết nối hai điểm

        SetLineWidth(0.2f);
    }

    // Lấy các điểm đường cong từ danh sách
    public List<Vector3> GetCurvePoints(Vector3 startPoint)
    {
        if (!isCurved)
        {
            curvePoints.Add(GetTargetPosition(startPoint));
        }
        return curvePoints;
    }

    // Vẽ đường cong Bezier giữa hai điểm
    public void DrawCurvedLine(Vector3 overlapPoint)
    {
        if (lineRenderer == null) return;

        Vector3 startPos = pointA.transform.position;
        Vector3 endPos = pointB.transform.position;

        // Tính toán hướng và vectơ vuông góc
        Vector3 direction = (endPos - startPos).normalized;
        Vector3 perpendicular = new Vector3(-direction.z, 0, direction.x); // Vuông góc trên mặt XZ

        // Tạo điểm điều khiển dựa trên điểm overlap
        // Điều chỉnh theo hệ số curvatureFactor để kiểm soát độ cong
        Vector3 controlPoint = overlapPoint + perpendicular * curvatureFactor;

        // Tính toán các điểm trên đường cong Bezier
        curvePoints = new List<Vector3>(CalculateBezierCurve(startPos, controlPoint, endPos, curveResolution));

        // Vẽ đường cong
        lineRenderer.positionCount = curvePoints.Count;
        lineRenderer.SetPositions(curvePoints.ToArray());
        isCurved = true;
    }

    // Vẽ nửa parabol giữa điểm overlap và điểm mục tiêu
    public void DrawHalfParabolicLine(Vector3 overlapPoint, Vector3 targetPoint)
    {
        if (lineRenderer == null) return;

        // Tính vector từ overlapPoint đến targetPoint
        Vector3 direction = (targetPoint - overlapPoint).normalized;
        Vector3 perpendicular = new Vector3(-direction.z, 0, direction.x); // Vuông góc trên mặt XZ

        // Tạo điểm điều khiển (control point) bằng cách dịch chuyển một chút theo hướng ngược lại
        Vector3 controlPoint = overlapPoint + perpendicular;

        // Tính toán các điểm trên nửa parabol
        curvePoints = new List<Vector3>(CalculateParabola(overlapPoint, controlPoint, targetPoint, curveResolution));

        // Vẽ nửa parabol
        lineRenderer.positionCount = curvePoints.Count;
        lineRenderer.SetPositions(curvePoints.ToArray());

        isCurved = true;
    }

    // Cài đặt độ rộng của đường
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

    // Lấy vị trí mục tiêu
    public Vector3 GetTargetPosition(Vector3 currentPosition)
    {
        float distanceToA = Vector3.Distance(currentPosition, pointA.transform.position);
        float distanceToB = Vector3.Distance(currentPosition, pointB.transform.position);

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
    private List<Vector3> CalculateBezierCurve(Vector3 start, Vector3 control, Vector3 end, int resolution)
    {
        List<Vector3> points = new List<Vector3>();
        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;
            Vector3 point = Mathf.Pow(1 - t, 2) * start +
                            2 * (1 - t) * t * control +
                            Mathf.Pow(t, 2) * end;
            points.Add(point);
        }
        return points;
    }

    // Tính toán nửa parabol
    private List<Vector3> CalculateParabola(Vector3 start, Vector3 vertex, Vector3 end, int resolution)
    {
        List<Vector3> points = new List<Vector3>();

        // Tính toán chỉ nửa parabol
        for (int i = 0; i <= resolution / 2; i++)
        {
            float t = i / (float)(resolution / 2); // t từ 0 đến 1 cho nửa đường cong

            // Công thức parabol
            Vector3 point = Mathf.Pow(1 - t, 2) * start +
                            2 * (1 - t) * t * vertex +
                            Mathf.Pow(t, 2) * end;
            points.Add(point);
        }

        return points;
    }

}
