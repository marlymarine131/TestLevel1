using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BreadthFirstSearch
{
    private List<Point> nodes;  // Các node trong đồ thị
    private List<Path> paths;   // Các đoạn đường (cạnh) nối giữa các node

    public BreadthFirstSearch(List<Point> nodes, List<Path> paths)
    {
        this.nodes = nodes;
        this.paths = paths;
    }

    public List<Path> FindShortestPath(Point startNode, Point endNode)
    {
        // Lưu trữ các node đã thăm và các node kế tiếp
        Dictionary<Point, Point> previousNodes = new Dictionary<Point, Point>();
        Queue<Point> queue = new Queue<Point>();
        HashSet<Point> visited = new HashSet<Point>();

        queue.Enqueue(startNode);
        visited.Add(startNode);

        while (queue.Count > 0)
        {
            Point currentNode = queue.Dequeue();

            // Nếu tìm thấy đích, tái tạo đường đi
            if (currentNode == endNode)
            {
                return ReconstructPath(previousNodes, endNode);
            }

            // Duyệt qua các node liền kề
            foreach (Point neighbor in currentNode.connectedNodes)
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                    previousNodes[neighbor] = currentNode;
                }
            }
        }

        return null; // Không có đường đi
    }

    private List<Path> ReconstructPath(Dictionary<Point, Point> previousNodes, Point endNode)
    {
        List<Path> path = new List<Path>();
        Point currentNode = endNode;

        while (previousNodes.ContainsKey(currentNode))
        {
            Point previousNode = previousNodes[currentNode];
            Path pathSegment = GetPathBetweenNodes(previousNode, currentNode);
            path.Insert(0, pathSegment);
            currentNode = previousNode;
        }

        return path;
    }

    private Path GetPathBetweenNodes(Point start, Point end)
    {
        foreach (Path path in paths)
        {
            if (path.ContainsPoint(start) && path.ContainsPoint(end))
            {
                return path;
            }
        }

        return null;
    }
}
