using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Point pointA, pointB;
    public float length;
    private LineRenderer lineRenderer;

    // Khởi tạo đoạn đường giữa hai điểm
    public void Initialize(Point start, Point end)
    {
        pointA = start;
        pointB = end;
        length = Vector3.Distance(pointA.transform.position, pointB.transform.position);

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 2;
        lineRenderer.SetPositions(new Vector3[] { pointA.transform.position, pointB.transform.position });

        pointA.Connect(pointB); // Kết nối hai điểm

        SetLineWidth(0.02f);
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
}
