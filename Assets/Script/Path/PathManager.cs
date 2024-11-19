using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    //public List<Path> paths = new List<Path>();

    //public void InitializePaths()
    //{
    //    foreach (var path in paths)
    //    {
    //        // Check for intersections
    //        foreach (var otherPath in paths)
    //        {
    //            if (path == otherPath) continue; // Skip self-check
    //            Point intersectionPoint = GetIntersectionPoint(path, otherPath);
    //            if (intersectionPoint != null)
    //            {
    //                // Adjust paths to create a curve at the intersection point
    //                AddCurveAtIntersection(path, otherPath, intersectionPoint);
    //            }
    //        }
    //    }
    //}

    //private Point GetIntersectionPoint(Path path1, Path path2)
    //{
    //    if (path1.pointA == path2.pointA || path1.pointA == path2.pointB)
    //        return path1.pointA;
    //    if (path1.pointB == path2.pointA || path1.pointB == path2.pointB)
    //        return path1.pointB;
    //    return null; // No intersection
    //}

    //private void AddCurveAtIntersection(Path path1, Path path2, Point intersectionPoint)
    //{
    //    // Adjust the LineRenderer of both paths to add curves
    //    Vector3 intersectionPos = intersectionPoint.transform.position;

    //    // Modify the path1's curve near the intersection
    //    Vector3 offset1 = (path1.GetOtherPoint(intersectionPoint).transform.position - intersectionPos).normalized * path1.curveOffset;
    //    Vector3 controlPoint1 = intersectionPos + offset1;

    //    // Modify the path2's curve near the intersection
    //    Vector3 offset2 = (path2.GetOtherPoint(intersectionPoint).transform.position - intersectionPos).normalized * path2.curveOffset;
    //    Vector3 controlPoint2 = intersectionPos + offset2;

    //    // Adjust line renderers for curves
    //    path1.DrawPartialCurve(intersectionPos, controlPoint1);
    //    path2.DrawPartialCurve(intersectionPos, controlPoint2);
    //}
}
