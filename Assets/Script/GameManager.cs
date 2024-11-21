using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject nodePrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject curvedLinePrefab;
    [SerializeField] private GameObject carPrefab;  // First car prefab
    [SerializeField] private GameObject secondCarPrefab;  // Second car prefab
    [SerializeField] private float targetTime = 5f;
    [SerializeField] private MeshGenerator meshGenerator;

    private HashSet<Point> curvedPoints = new HashSet<Point>();
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

    private void Update()
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
        Point overlapPoint = new Point();
        line.Initialize(startNode, endNode, meshGenerator);

        foreach (var path in paths)
        {
            if (line.pointA.transform.position == path.pointA.transform.position || line.pointB.transform.position == path.pointA.transform.position)
            {
                overlapPoint = path.pointA; // Gán path trùng vào overlapPoint
                var pathPoint = path.pointB;
                var linePoint = (line.pointA == overlapPoint) ? line.pointA : line.pointB;
                Vector3 targetPoint = Vector3.Lerp(
               overlapPoint.transform.position,
               (line.pointA == overlapPoint ? line.pointB.transform.position : line.pointA.transform.position),
                .4f); // Lấy 1/10 chiều dài của đường line

                Vector3 targetPointPrevious = Vector3.Lerp(
                    overlapPoint.transform.position,
                    (path.pointA == overlapPoint ? path.pointB.transform.position : path.pointA.transform.position),
                    .4f); // Lấy 1/10 chiều dài của đường path

                GameObject curvedObject = Instantiate(curvedLinePrefab);
                CurvedLine curvedLine = curvedObject.AddComponent<CurvedLine>();
                curvedLine.Initialize(targetPoint, overlapPoint.transform.position, targetPointPrevious,line,path, meshGenerator);
            }
            else if (line.pointA.transform.position == path.pointB.transform.position || line.pointB.transform.position == path.pointB.transform.position)
            {
                overlapPoint = path.pointB; // Gán path trùng vào overlapPoint
                var pathPoint = path.pointB;
                var linePoint = (line.pointA == overlapPoint) ? line.pointA : line.pointB;
                Vector3 targetPoint = Vector3.Lerp(
                overlapPoint.transform.position,
                (line.pointA == overlapPoint ? line.pointB.transform.position : line.pointA.transform.position),
                .4f); // Lấy 1/10 chiều dài của đường line

                Vector3 targetPointPrevious = Vector3.Lerp(
                    overlapPoint.transform.position,
                    (path.pointA == overlapPoint ? path.pointB.transform.position : path.pointA.transform.position),
                    .4f); // Lấy 1/10 chiều dài của đường path

                GameObject curvedObject = Instantiate(curvedLinePrefab);
                CurvedLine curvedLine = curvedObject.AddComponent<CurvedLine>();
                curvedLine.Initialize(targetPoint, overlapPoint.transform.position, targetPointPrevious, line, path,meshGenerator);
            }
        }

        Debug.Log("Over Lap at Point: " + overlapPoint);
        paths.Add(line); // Thêm Path vào danh sách
    }
    // Chọn điểm bắt đầu cho xe đầu tiên
    private void SelectFirstCarStartNode()
    {
        firstStartNode = GetNodeAtMousePosition();
        Debug.Log("Start Node: " + (firstStartNode != null ? firstStartNode.name : "None"));
        Vector3 instanceCar = firstStartNode.transform.position;
        instanceCar.y += 1f;
        if (firstStartNode != null)
        {
            if (carInstance == null)
            {
                
                carInstance = Instantiate(carPrefab, instanceCar, Quaternion.identity);
                carController = carInstance.GetComponent<CarController>();
            }
            else
            {
                carInstance.transform.position = instanceCar;
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
        Vector3 instanceCar = secondStartNode.transform.position;
        instanceCar.y += 1f;
        if (secondStartNode != null)
        {
            if (secondCarInstance == null)
            {
                
                secondCarInstance = Instantiate(secondCarPrefab, instanceCar, Quaternion.identity);
                secondCarController = secondCarInstance.GetComponent<CarController>();
            }
            else
            {
                secondCarInstance.transform.position = instanceCar;
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
        if ((secondStartNode != null || firstStartNode != null) && endNode != null)
        {
            // Tìm đường đi ngắn nhất sử dụng Dijkstra
            //DijkstraPathFinding dijkstra = new DijkstraPathFinding(nodes, paths);
            BreadthFirstSearch breadthFirstSearch = new BreadthFirstSearch(nodes, paths);
            if (carController != null)
            {
                List<Path> shortestPath = breadthFirstSearch.FindShortestPath(firstStartNode, endNode);
                StartCoroutine(MoveCar(shortestPath, carController, Color.yellow));
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
            car.SetPath(shortestPath, targetTime,endNode.transform.position);
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

        // Raycast trên đối tượng có MeshCollider
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Trả về vị trí va chạm trên đối tượng với MeshCollider
            return hit.point;
        }

        return Vector3.zero; // Giá trị mặc định nếu không có va chạm
    }


    // Hàm lấy node tại vị trí con trỏ chuột
    private Point GetNodeAtMousePosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // Raycast để kiểm tra va chạm
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Kiểm tra xem collider có chứa thành phần Point không
            return hit.collider.GetComponent<Point>();
        }

        return null; // Trả về null nếu không có thành phần Point
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
