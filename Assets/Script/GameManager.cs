using System;
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
    private Point firstStartNode = null;
    private Point secondStartNode = null;
    private Point endNode = null;
    private GameObject carInstance;
    private CarController carController;
    private GameObject secondCarInstance;
    private CarController secondCarController;
    private int nodeID = 0;

    public EventHandler<OnStartAChange_Args> onStartAChange;
    public EventHandler<OnStartBChange_Args> onStartBChange;
    public EventHandler<OnEndChange_Args> onEndChange;

    public class OnStartAChange_Args : EventArgs
    {
        public string startAName;
    }
    public class OnStartBChange_Args : EventArgs
    {
        public string startBName;
    }
    public class OnEndChange_Args : EventArgs
    {
        public string endName;
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButtonDown(0)) // Tạo node
        {
            CreateNodeAtMousePosition();
        }
        else if (Input.GetMouseButtonDown(1)) // Kết nối node
        {
            ConnectNodesAtMousePosition();
        }
        else if (Input.GetKeyDown(KeyCode.F)) // Chọn điểm bắt đầu cho xe đầu tiên
        {
            SelectFirstCarStartNode();
        }
        else if (Input.GetKeyDown(KeyCode.M)) // Chọn điểm bắt đầu cho xe thứ hai
        {
            SelectSecondCarStartNode();
        }
        else if (Input.GetKeyDown(KeyCode.T)) // Chọn điểm kết thúc
        {
            SelectEndNode();
        }
        else if (Input.GetKeyDown(KeyCode.Space)) // Xe bắt đầu chạy
        {
            StartCarsMovement();
        }
    }

    // Tạo node mới tại vị trí con trỏ chuột
    private void CreateNodeAtMousePosition()
    {
        Vector3 spawnPosition = GetMousePosition();
        GameObject newNodeObj = Instantiate(nodePrefab, spawnPosition, Quaternion.identity);
        Point newNode = newNodeObj.AddComponent<Point>();
        newNode.name = "Node_" + nodeID++;
        nodes.Add(newNode);
    }

    // Kết nối các node khi nhấp chuột phải
    private void ConnectNodesAtMousePosition()
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
                    CreatePathBetweenNodes(selectedNode, clickedNode);
                    selectedNode = null; // Reset sau khi nối
                }
                else
                {
                    selectedNode = null;
                }
            }
        }
    }

    // Tạo đoạn đường nối hai node
    private void CreatePathBetweenNodes(Point startNode, Point endNode)
    {
        GameObject lineObj = Instantiate(linePrefab);
        Path line = lineObj.AddComponent<Path>();
        line.Initialize(startNode, endNode);
        paths.Add(line); // Thêm Path vào danh sách
    }

    // Chọn điểm bắt đầu cho xe đầu tiên
    private void SelectFirstCarStartNode()
    {
        firstStartNode = GetNodeAtMousePosition();
        Debug.Log("Start Node: " + (firstStartNode != null ? firstStartNode.name : "None"));

        if (firstStartNode != null)
        {
            if (carInstance == null)
            {
                carInstance = Instantiate(carPrefab, firstStartNode.transform.position, Quaternion.identity);
                carController = carInstance.GetComponent<CarController>();
            }
            else
            {
                carInstance.transform.position = firstStartNode.transform.position;
            }
        }
        CallOnAChange(firstStartNode);
    }

    // Chọn điểm bắt đầu cho xe thứ hai
    private void SelectSecondCarStartNode()
    {
        secondStartNode = GetNodeAtMousePosition();
        if (secondStartNode == null)
        {
            Debug.Log("No second car start node selected. Defaulting to a random node.");
            // Assign second car to a random point if no node selected
            secondStartNode = nodes[UnityEngine.Random.Range(0, nodes.Count)];
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
        CallOnBChange(secondStartNode);
    }

    // Chọn điểm kết thúc
    private void SelectEndNode()
    {
        endNode = GetNodeAtMousePosition();
        Debug.Log("End Node: " + (endNode != null ? endNode.name : "None"));
        CallOnEndChange(endNode);
    }

    // Xe bắt đầu di chuyển
    private void StartCarsMovement()
    {
        if ((secondStartNode != null || firstStartNode!= null) && endNode != null)
        {
            // Tìm đường đi ngắn nhất sử dụng Dijkstra
            //DijkstraPathFinding dijkstra = new DijkstraPathFinding(nodes, paths);
            BreadthFirstSearch breadthFirstSearch = new BreadthFirstSearch(nodes,paths);
            if (carController != null)
            {
                List<Path> shortestPath = breadthFirstSearch.FindShortestPath(firstStartNode, endNode);
                StartCoroutine( MoveCar(shortestPath, carController, Color.yellow));
            }

            if (secondCarController != null)
            {
                List<Path> shortestPath = breadthFirstSearch.FindShortestPath(secondStartNode, endNode);

                StartCoroutine(MoveCar(shortestPath, secondCarController, Color.red));
            }
        }
    }

    // Di chuyển xe theo đường đi ngắn nhất
    private IEnumerator MoveCar(List<Path> shortestPath, CarController car, Color color)
    {
        if (shortestPath != null)
        {
            car.SetPath(shortestPath, targetTime);
            foreach (Path path in shortestPath)
            {
                path.HighlightPath(color); // Đổi màu các đường đi ngắn nhất
            }
        }
        yield return null;
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

    private void CallOnBChange(Point point)
    {
        onStartBChange?.Invoke(this, new OnStartBChange_Args
        {
            startBName = point.gameObject.name
        });
    }

    private void CallOnAChange(Point point)
    {
        onStartAChange?.Invoke(this, new OnStartAChange_Args
        {
            startAName = point.gameObject.name
        });
    }

    private void CallOnEndChange(Point point)
    {
        onEndChange?.Invoke(this, new OnEndChange_Args
        {
            endName = point.gameObject.name
        });
    }
}
