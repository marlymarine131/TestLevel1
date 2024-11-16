using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject carPrefab;  // First car prefab
    [SerializeField] private GameObject secondCarPrefab;  // Second car prefab
    [SerializeField] private float targetTime = 5f;

    private List<Point> nodes = new List<Point>();
    private List<Path> paths = new List<Path>(); // Danh sách các đoạn đường
    private Point selectedNode = null;
    private Point fistStartNode = null;
    private Point secondStartNode = null;
    private Point endNode = null;
    private GameObject carInstance;
    private CarController carController;
    private GameObject secondCarInstance;
    private CarController secondCarController;
    private int nodeID = 0;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Tạo node
        {
            Vector3 spawnPosition = GetMousePosition();
            GameObject newNodeObj = Instantiate(nodePrefab, spawnPosition, Quaternion.identity);
            Point newNode = newNodeObj.AddComponent<Point>();
            newNode.name = "Node_" + nodeID++;
            nodes.Add(newNode);
        }
        else if (Input.GetMouseButtonDown(1)) // Kết nối node
        {
            Point clickedNode = GetNodeAtMousePosition();
            if (clickedNode != null)
            {
                if (selectedNode == null)
                {
                    selectedNode = clickedNode;
                }
                else
                {
                    if (selectedNode != clickedNode && !selectedNode.IsConnectedTo(clickedNode))
                    {
                        GameObject lineObj = Instantiate(linePrefab);
                        Path line = lineObj.AddComponent<Path>();
                        line.Initialize(selectedNode, clickedNode);

                        paths.Add(line); // Thêm Path vào danh sách

                        selectedNode = null; // Reset sau khi nối
                    }
                    else
                    {
                        selectedNode = null;
                    }
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.F)) // Chọn điểm bắt đầu cho xe đầu tiên
        {
            fistStartNode = GetNodeAtMousePosition();
            Debug.Log("Start Node: " + (fistStartNode != null ? fistStartNode.name : "None"));

            if (fistStartNode != null)
            {
                if (carInstance == null)
                {
                    carInstance = Instantiate(carPrefab, fistStartNode.transform.position, Quaternion.identity);
                    carController = carInstance.GetComponent<CarController>();
                }
                else
                {
                    carInstance.transform.position = fistStartNode.transform.position;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.M)) // Chọn điểm bắt đầu cho xe thứ hai tại một điểm khác
        {
            secondStartNode = GetNodeAtMousePosition();
            if (secondStartNode == null)
            {
                Debug.Log("No second car start node selected. Defaulting to a random node.");
                // Assign second car to a random point if no node selected
                secondStartNode = nodes[Random.Range(0, nodes.Count)];
            }

            Debug.Log("Start Node for second car: " + (secondStartNode != null ? secondStartNode.name : "None"));

            if (secondStartNode != null)
            {
                if (secondCarInstance == null)
                {
                    secondCarInstance = Instantiate(secondCarPrefab, secondStartNode.transform.position, Quaternion.identity);
                    secondCarController = secondCarInstance.GetComponent<CarController>();
                }
                else
                {
                    secondCarInstance.transform.position = secondStartNode.transform.position;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.T)) // Chọn điểm kết thúc
        {
            endNode = GetNodeAtMousePosition();
            Debug.Log("End Node: " + (endNode != null ? endNode.name : "None"));
        }
        else if (Input.GetKeyDown(KeyCode.Space)) // Xe bắt đầu chạy
        {
            if ((secondStartNode != null || fistStartNode) && endNode != null)
            {
                // Tìm đường đi ngắn nhất sử dụng Dijkstra
                DijkstraPathFinding dijkstra = new DijkstraPathFinding(nodes, paths);
                if (carController != null)
                {
                    List<Path> shortestPath = dijkstra.FindShortestPath(fistStartNode, endNode);
                    moveCar(shortestPath,carController,Color.yellow);
                }

                if (secondCarController != null)
                {
                    List<Path> shortestPath = dijkstra.FindShortestPath(secondStartNode, endNode);
                    moveCar(shortestPath, secondCarController, Color.red);
                }

            }
        }
    }


    private void moveCar(List<Path> shortestPath, CarController car, Color color)
    {
        car.SetPath(shortestPath, targetTime);
        foreach (Path path in shortestPath)
        {
            path.HighlightPath(color); // Đổi màu các đường đi ngắn nhất
        }
    }
    // Hàm lấy vị trí con trỏ chuột trên mặt phẳng 3D
    private Vector3 GetMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }

    // Hàm lấy node tại vị trí con trỏ chuột
    private Point GetNodeAtMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.collider.GetComponent<Point>();
        }
        return null;
    }
}
