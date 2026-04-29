using UnityEngine;

public class DynamicBlobController : MonoBehaviour
{
[SerializeField] private int resolution = 64;
    [SerializeField] private float radius = 1f;
    [SerializeField] private float noiseScale = 0.5f;
    [SerializeField] private float animationSpeed = 1f;
    [SerializeField] private float deformationAmount = 0.3f;
    
    private Mesh mesh;
    private Vector3[] originalVertices;
    private Vector3[] vertices;
    
    void Start()
    {
        CreateCircleMesh();
    }
    
    void CreateCircleMesh()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        
        vertices = new Vector3[resolution + 1];
        originalVertices = new Vector3[resolution + 1];
        Vector2[] uvs = new Vector2[resolution + 1]; // Add UV array
        int[] triangles = new int[resolution * 3];
        
        // Center vertex
        vertices[0] = Vector3.zero;
        originalVertices[0] = Vector3.zero;
        uvs[0] = new Vector2(0.5f, 0.5f); // Center UV
        
        // Circle vertices
        for (int i = 0; i < resolution; i++)
        {
            float angle = i * 2f * Mathf.PI / resolution;
            Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
            vertices[i + 1] = pos;
            originalVertices[i + 1] = pos;
            
            // Calculate UV coordinates (map from -1,1 to 0,1)
            uvs[i + 1] = new Vector2(
                (pos.x / radius + 1f) * 0.5f,
                (pos.y / radius + 1f) * 0.5f
            );
        }
        
        // Triangles
        for (int i = 0; i < resolution; i++)
        {
            triangles[i * 3] = 0; // Center
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = (i + 1) % resolution + 1;
        }
        
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs; // Assign UVs to the mesh
        mesh.RecalculateNormals();
    }
    
    void Update()
    {
        AnimateVertices();
    }
    
    void AnimateVertices()
    {
        for (int i = 1; i < vertices.Length; i++)
        {
            Vector3 originalPos = originalVertices[i];
            
            // Use Perlin noise for smooth animation
            float noise = Mathf.PerlinNoise(
                originalPos.x * noiseScale + Time.time * animationSpeed,
                originalPos.y * noiseScale + Time.time * animationSpeed
            );
            
            // Apply deformation
            float deformation = (noise - 0.5f) * deformationAmount;
            vertices[i] = originalPos * (1f + deformation);
        }
        
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.orange;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
