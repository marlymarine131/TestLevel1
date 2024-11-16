using System.Collections.Generic;
using UnityEngine;

public class DijkstraPathFinding
{
    private List<Point> nodes;
    private List<Path> paths;
   
    public DijkstraPathFinding(List<Point> nodes, List<Path> paths)
    {
        this.nodes = nodes;
        this.paths = paths;
    }

    public List<Path> FindShortestPath(Point startNode, Point endNode)
    {
        Dictionary<Point, float> distances = new Dictionary<Point, float>();
        Dictionary<Point, Point> previousNodes = new Dictionary<Point, Point>();
        List<Point> unvisitedNodes = new List<Point>(nodes);

        foreach (var node in nodes)
        {
            distances[node] = float.MaxValue;
        }
        distances[startNode] = 0;

        while (unvisitedNodes.Count > 0)
        {
            // Lấy node có khoảng cách nhỏ nhất
            Point currentNode = GetNodeWithSmallestDistance(unvisitedNodes, distances);
            unvisitedNodes.Remove(currentNode);

            if (currentNode == endNode)
            {
                return ReconstructPath(previousNodes, endNode);
            }

            foreach (Point neighbor in currentNode.connectedNodes)
            {
                if (unvisitedNodes.Contains(neighbor))
                {
                    float tentativeDistance = distances[currentNode] + currentNode.distances[neighbor];

                    if (tentativeDistance < distances[neighbor])
                    {
                        distances[neighbor] = tentativeDistance;
                        previousNodes[neighbor] = currentNode;
                    }
                }
            }
        }

        return null; // Không có đường đi
    }

    private Point GetNodeWithSmallestDistance(List<Point> unvisitedNodes, Dictionary<Point, float> distances)
    {
        float smallestDistance = float.MaxValue;
        Point smallestNode = null;

        foreach (Point node in unvisitedNodes)
        {
            if (distances[node] < smallestDistance)
            {
                smallestDistance = distances[node];
                smallestNode = node;
            }
        }

        return smallestNode;
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
