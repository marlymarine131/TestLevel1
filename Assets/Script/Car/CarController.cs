using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] private CarSO car; // Thông tin xe từ ScriptableObject
    private Rigidbody carBody;
    private Queue<Path> pathQueue = new Queue<Path>(); // Hàng đợi các đoạn đường (Path)
    private List<Path> allPaths = new List<Path>(); // Lưu lại tất cả các paths
    private bool isMoving = false;

    private void Start()
    {
        carBody = GetComponent<Rigidbody>();
    }

    // Thiết lập đường đi
    public void SetPath(List<Path> path, float targetTime)
    {
        pathQueue.Clear();
        allPaths.Clear(); // Reset all paths list

        foreach (Path segment in path)
        {
            pathQueue.Enqueue(segment);
            allPaths.Add(segment); // Add to all paths for later color reset
        }

        // Bắt đầu di chuyển nếu chưa chạy
        if (!isMoving)
        {
            StartCoroutine(FollowPath(targetTime));
        }
    }

    private IEnumerator FollowPath(float targetTime)
    {
        isMoving = true;
        int pathIndex = 1; // Bắt đầu đánh số từ 1
        float distance = allPaths.Sum(q => q.length); // Tính tổng chiều dài của tất cả các path

        while (pathQueue.Count > 0)
        {
            Path currentPath = pathQueue.Peek(); // Lấy path đầu tiên trong hàng đợi mà chưa dequeue

            // Lấy các điểm trên đường cong của path hiện tại
            List<Vector3> curvePoints = currentPath.GetCurvePoints(transform.position);

            // Kiểm tra hướng di chuyển, từ điểm gần nhất tới điểm còn lại
            Vector3 currentPos = transform.position;
            Vector3 targetPosition = curvePoints.First(); // Mặc định bắt đầu di chuyển tới điểm đầu tiên của đường cong

            // Nếu xe đang ở gần điểm cuối, đảo ngược hướng di chuyển
            if (Vector3.Distance(currentPos, curvePoints.Last()) < Vector3.Distance(currentPos, targetPosition))
            {
                // Xe di chuyển từ điểm cuối đến điểm đầu của đường cong
                curvePoints.Reverse(); // Đảo ngược danh sách điểm đường cong
            }

            // Di chuyển xe qua các điểm này
            foreach (Vector3 point in curvePoints)
            {
                // Tính toán tốc độ di chuyển
                float requiredSpeed = distance / targetTime;

                // Di chuyển đến điểm mục tiêu
                while (Vector3.Distance(transform.position, point) > 0.1f)
                {
                    Vector3 direction = (point - transform.position).normalized;
                    Vector3 move = direction * requiredSpeed * Time.deltaTime;

                    carBody.MovePosition(transform.position + move);
                    yield return null;
                }
            }

            // Khi đã đi hết tất cả các điểm trên path hiện tại, dequeue và tiếp tục với path tiếp theo
            Debug.Log("Reached Target Position: " + curvePoints.Last() + " (Path " + pathIndex + ")");
            pathQueue.Dequeue();

            // Tăng chỉ số path
            pathIndex++;
        }

        // Reset màu sắc của tất cả các path sau khi xe đã di chuyển xong
        foreach (Path path in allPaths)
        {
            path.ResetColor(); // Reset màu sắc của mỗi path
        }

        isMoving = false;
    }



}
