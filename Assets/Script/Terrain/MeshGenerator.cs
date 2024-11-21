using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertical;
    int[] triangles;
    Vector2[] uvs;
    public int xSize = 20;
    public int zSize = 20;
    public float maxHeight = 1.0f;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();
        UpdateMesh();

        // Gán MeshCollider
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            meshCollider = gameObject.AddComponent<MeshCollider>();
        }
        meshCollider.sharedMesh = mesh; // Đảm bảo collider khớp với mesh
    }

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertical;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();

        // Cập nhật MeshCollider
        MeshCollider meshCollider = gameObject.GetComponent<MeshCollider>();
        if (meshCollider != null)
        {
            meshCollider.sharedMesh = null; // Xóa collider cũ trước
            meshCollider.sharedMesh = mesh;
            meshCollider.isTrigger = true;
        }
    }


    private void CreateShape()
    {
        vertical = new Vector3[(xSize + 1) * (zSize + 1)];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * maxHeight;
                vertical[i] = new Vector3(x, y, z);
                i++;
            }
        }

        triangles = new int[zSize * zSize * 6];
        int ver = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = ver + 0;
                triangles[tris + 1] = ver + xSize + 1;
                triangles[tris + 2] = ver + 1;
                triangles[tris + 3] = ver + 1;
                triangles[tris + 4] = ver + xSize + 1;
                triangles[tris + 5] = ver + xSize + 2;

                ver++;
                tris += 6;
            }
        }

        uvs = new Vector2[vertical.Length];
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                uvs[i] = new Vector2((float)x / xSize, (float)z / zSize);
                i++;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (vertical == null) return;
        for (int i = 0; i < vertical.Length; i++)
        {
            Gizmos.DrawSphere(vertical[i], .1f);
        }
    }

    // New method to project points onto the mesh surface
    public List<Vector3> ProjectPointsOntoMesh(List<Vector3> points)
    {
        List<Vector3> projectedPoints = new List<Vector3>();

        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Transform meshTransform = transform;

        foreach (Vector3 point in points)
        {
            // Thay đổi giá trị y của point dựa trên chiều cao của mesh
            float newY = GetHeightAtPosition(transform.InverseTransformPoint(point));
            Vector3 projectedPoint = new Vector3(point.x, newY, point.z);

            projectedPoints.Add(projectedPoint);
        }

        return projectedPoints;
    }

    public float GetHeightAtPosition(Vector3 position)
    {
        // Chuyển đổi vị trí x và z vào phạm vi của mesh
        if (position.x < 0 || position.x > xSize || position.z < 0 || position.z > zSize)
        {
            Debug.LogWarning($"Position {position} is outside the mesh bounds!");
            return transform.position.y; // Trả về offset của game object (vị trí y)
        }

        int x0 = Mathf.FloorToInt(position.x);
        int x1 = Mathf.Clamp(x0 + 1, 0, xSize);
        int z0 = Mathf.FloorToInt(position.z);
        int z1 = Mathf.Clamp(z0 + 1, 0, zSize);

        // Xác định chỉ số các đỉnh
        int index00 = z0 * (xSize + 1) + x0;
        int index01 = z1 * (xSize + 1) + x0;
        int index10 = z0 * (xSize + 1) + x1;
        int index11 = z1 * (xSize + 1) + x1;

        // Lấy chiều cao của các đỉnh
        float y00 = vertical[index00].y;
        float y01 = vertical[index01].y;
        float y10 = vertical[index10].y;
        float y11 = vertical[index11].y;

        // Tỷ lệ nội suy
        float tx = (x0 == x1) ? 0 : Mathf.InverseLerp(x0, x1, position.x);
        float tz = (z0 == z1) ? 0 : Mathf.InverseLerp(z0, z1, position.z);

        // Nội suy bilinear
        float y0 = Mathf.Lerp(y00, y10, tx);
        float y1 = Mathf.Lerp(y01, y11, tx);
        float y = Mathf.Lerp(y0, y1, tz);

        // Cộng thêm offset y của game object
        return y + transform.position.y;
    }



}
