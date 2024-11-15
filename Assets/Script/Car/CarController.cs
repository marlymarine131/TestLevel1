using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private CarSO car; // Thông tin xe từ ScriptableObject
    private Rigidbody carBody;
    private Queue<Path> pathQueue = new Queue<Path>(); // Hàng đợi các đoạn đường (Path)
    private bool isMoving = false;

    private void Start()
    {
        carBody = GetComponent<Rigidbody>();
    }

    // Thiết lập đường đi
    public void SetPath(List<Path> path)
    {
        pathQueue.Clear();
        foreach (Path segment in path)
        {
            pathQueue.Enqueue(segment);
        }

        // Bắt đầu di chuyển nếu chưa chạy
        if (!isMoving)
        {
            StartCoroutine(FollowPath());
        }
    }

    private IEnumerator FollowPath()
    {
        isMoving = true;
        int pathIndex = 1; // Start numbering paths from 1

        while (pathQueue.Count > 0)
        {
            Path currentPath = pathQueue.Peek(); // Check the first item without dequeuing yet

            // Determine the direction and target position based on the current path
            Vector3 targetPosition = currentPath.GetTargetPosition(transform.position);

            // Debugging path index and target name
            Debug.Log("Path " + pathIndex + " Target Position: " + currentPath.name);

            // Move towards the target position
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                Vector3 direction = (targetPosition - transform.position).normalized;
                Vector3 move = direction * car.speed * Time.deltaTime;

                carBody.MovePosition(transform.position + move);
                yield return null;
            }

            // Once we reach the target position, dequeue and move to the next path
            Debug.Log("Reached Target Position: " + targetPosition + " (Path " + pathIndex + ")");
            pathQueue.Dequeue(); // Remove the current path from the queue

            // Increment path index for the next path
            pathIndex++;
        }

        isMoving = false;
    }
}
