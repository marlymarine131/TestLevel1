using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject carPrefab;

    private List<Point> nodes = new List<Point>();
    private List<Path> paths = new List<Path>(); // Danh sách các đoạn đường
    private Point selectedNode = null;
    private Point startNode = null;
    private Point endNode = null;
    private GameObject carInstance;
    private CarController carController;
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
        else if (Input.GetKeyDown(KeyCode.F)) // Chọn điểm bắt đầu
        {
            startNode = GetNodeAtMousePosition();
            Debug.Log("Start Node: " + (startNode != null ? startNode.name : "None"));

            if (startNode != null)
            {
                if (carInstance == null)
                {
                    carInstance = Instantiate(carPrefab, startNode.transform.position, Quaternion.identity);
                    carController = carInstance.GetComponent<CarController>();
                }
                else
                {
                    carInstance.transform.position = startNode.transform.position;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.T)) // Chọn điểm kết thúc
        {
            endNode = GetNodeAtMousePosition();
            Debug.Log("End Node: " + (endNode != null ? endNode.name : "None"));
        }
        // Xe bắt đầu chạy
        else if (Input.GetKeyDown(KeyCode.Space)) // Xe bắt đầu chạy
        {
            if (startNode != null && endNode != null)
            {
                // Tìm đường đi ngắn nhất sử dụng Dijkstra
                DijkstraPathFinding dijkstra = new DijkstraPathFinding(nodes, paths);
                List<Path> shortestPath = dijkstra.FindShortestPath(startNode, endNode);
                string pathRoad = "";
                foreach (var path in shortestPath)
                {
                    pathRoad += path.pointA + "-" + path.pointB + ";";
                }
                Debug.Log(pathRoad);
                if (shortestPath != null && carController != null)
                {
                    carController.SetPath(shortestPath);

                    // Đổi màu các Path trên đường đi thành màu vàng
                    foreach (Path path in shortestPath)
                    {
                        path.HighlightPath(Color.yellow); // Đổi màu các đường đi ngắn nhất
                    }
                }
                else
                {
                    Debug.Log("No path found or car not initialized.");
                }
            }
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
