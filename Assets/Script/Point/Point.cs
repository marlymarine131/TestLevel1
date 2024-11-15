using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    public List<Point> connectedNodes = new List<Point>();
    public Dictionary<Point, float> distances = new Dictionary<Point, float>();

    // Kết nối điểm này với một điểm khác
    public void Connect(Point point)
    {
        if (!connectedNodes.Contains(point))
        {
            connectedNodes.Add(point);
            point.connectedNodes.Add(this);

            // Tính khoảng cách giữa hai điểm
            float distance = Vector3.Distance(this.transform.position, point.transform.position);
            distances[point] = distance;
            point.distances[this] = distance;
        }
    }

    // Kiểm tra nếu điểm này đã được kết nối
    public bool IsConnectedTo(Point point)
    {
        return connectedNodes.Contains(point);
    }
}
