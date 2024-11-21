using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private CarSO car;
    private Rigidbody carBody;
    private Queue<Path> pathQueue = new Queue<Path>();
    private List<Path> allPaths = new List<Path>();
    private bool isMoving = false;
    Vector3 currentPos;
    private void Start()
    {
        carBody = GetComponent<Rigidbody>();
    }

    public void SetPath(List<Path> path, float targetTime, Vector3 finalPoint)
    {
        pathQueue.Clear();
        allPaths.Clear();

        foreach (Path segment in path)
        {
            pathQueue.Enqueue(segment);
            allPaths.Add(segment);
        }

        if (!isMoving)
        {
            StartCoroutine(FollowPath(targetTime, finalPoint));
        }
    }
    private CurvedLine FindCurvedLine(Path path1, Path path2)
    {
        CurvedLine[] curvedLines = FindObjectsOfType<CurvedLine>();
        foreach (CurvedLine curvedLine in curvedLines)
        {
            if ((curvedLine.path1 == path1 && curvedLine.path2 == path2) ||
                (curvedLine.path1 == path2 && curvedLine.path2 == path1))
            {
                return curvedLine;
            }
        }

        return null; // Không tìm thấy
    }
    private IEnumerator FollowPath(float targetTime, Vector3 finalPoint)
    {
        isMoving = true;
        currentPos = transform.position;

        if (pathQueue.Count == 1)
        {
            yield return MoveAlongSinglePath(pathQueue.Dequeue());
        }
        else
        {
            yield return MoveAlongMultiplePaths();
        }

        ResetPathColors();
        isMoving = false;
    }

    // Move along a single path
    private IEnumerator MoveAlongSinglePath(Path currentPath)
    {
        List<Vector3> points = GetReorderedPathPoints(currentPath);

        foreach (Vector3 targetPoint in points)
        {
            yield return MoveToPoint(targetPoint);
        }

        Debug.Log($"Path {currentPath.name} has been completed.");
    }

    // Move along multiple paths
    private IEnumerator MoveAlongMultiplePaths()
    {
        while (pathQueue.Count > 0)
        {
            Path currentPath = pathQueue.Dequeue();
            List<Vector3> points = GetReorderedPathPoints(currentPath);


            foreach (Vector3 targetPoint in points)
            {
                yield return MoveToPoint(targetPoint);

                if (IsNearCurvedLine(currentPath))
                {
                    yield return MoveAlongCurvedLine(currentPath);
                    break;
                }
            }

        }
    }

    // Move to a specific point
    private IEnumerator MoveToPoint(Vector3 targetPoint)
    {
        while (Vector3.Distance(transform.position, targetPoint) > 1f)
        {
            Vector3 direction = (targetPoint - transform.position).normalized;

            if (direction.magnitude > 0.01f)
            {
                RotateTowardsDirection(direction);
                MoveInDirection(direction);
            }

            yield return null;
        }
        currentPos = transform.position;
    }

    // Move along a curved line
    private IEnumerator MoveAlongCurvedLine(Path currentPath)
    {

        CurvedLine curvedLine = FindCurvedLine(currentPath, pathQueue.Peek());
        if (curvedLine != null)
        {
            List<Vector3> curvedPoints = GetReorderedCurvePoints(curvedLine);

            foreach (Vector3 curvedPoint in curvedPoints)
            {
                yield return MoveToPoint(curvedPoint);
            }
        }
    }

    // Get reordered points for a path based on proximity
    private List<Vector3> GetReorderedPathPoints(Path path)
    {
        Debug.Log(currentPos);
        List<Vector3> points = path.GetAllPointOfPath();
        int index = points.FindIndex(point => Vector3.Distance(point, currentPos) < 1f);
        Debug.Log("index: " + index);

        float distanceToEnd = Vector3.Distance(currentPos, points.Last());
        float distanceToStart = Vector3.Distance(currentPos, points.First());
        Debug.Log("toend: " + distanceToEnd + " tostart" + distanceToStart);
        if (distanceToEnd <1 || distanceToStart < 1)
        {
            if (distanceToEnd < distanceToStart)
            {
                points.Reverse();
            }
            return points;
        }
        if (distanceToEnd > distanceToStart)
        {
            Debug.Log("get end");
            points = points.GetRange(index + 10, points.Count - index- 10 );
        }
        else
        {
            Debug.Log("get start");
            points = points.GetRange(0,index + 1);
            points.Reverse();
        }

        return points;
    }

    // Get reordered points for a curved line based on proximity
    private List<Vector3> GetReorderedCurvePoints(CurvedLine curvedLine)
    {
        List<Vector3> curvedPoints = curvedLine.GetCurvePoints();
        if (Vector3.Distance(transform.position, curvedPoints.Last()) < Vector3.Distance(transform.position, curvedPoints.First()))
        {
            curvedPoints.Reverse();
        }
        return curvedPoints;
    }

    // Check if near a curved line
    private bool IsNearCurvedLine(Path currentPath)
    {
        if (pathQueue.Count == 0) return false;
        CurvedLine curvedLine = FindCurvedLine(currentPath, pathQueue.Peek());
        if (curvedLine != null)
        {
            return Vector3.Distance(transform.position, curvedLine.GetStartPoint()) < 1f ||
                   Vector3.Distance(transform.position, curvedLine.GetEndPoint()) < 1f;
        }
        return false;
    }

    // Rotate the object towards a direction
    private void RotateTowardsDirection(Vector3 direction)
    {
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        targetRotation = Quaternion.Euler(0f, targetRotation.eulerAngles.y, 0f); // Lock rotation to Y-axis
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 20f);
    }

    // Move the object in a direction
    private void MoveInDirection(Vector3 direction)
    {
        Vector3 move = direction * 5f * Time.deltaTime;
        carBody.MovePosition(transform.position + move);
    }

    // Reset colors of all paths
    private void ResetPathColors()
    {
        foreach (Path path in allPaths)
        {
            path.ResetColor();
        }
    }


}
