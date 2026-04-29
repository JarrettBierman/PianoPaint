using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Trail3D : MonoBehaviour
{
    [Header("Trail Settings")]
    public Transform targetObject; // Object to follow (should be child of same parent)
    public Transform parentContainer; // Parent object that contains both trail and target
    public float trailWidth = 0.5f;
    public float minDistance = 0.1f; // Min distance before adding new segment
    public int maxSegments = 50; // Max trail segments
    public bool isGrowing = true; // Condition to stop growing
    
    [Header("Taper Settings")]
    public AnimationCurve widthCurve = AnimationCurve.Linear(0, 1, 1, 0.1f);
    public int radialSegments = 8; // Smoothness of cylindrical trail
    
    [Header("Smoothing")]
    public bool smoothTrail = true;
    public int smoothingPasses = 2;
    
    private MeshFilter meshFilter;
    private Mesh mesh;
    private List<Vector3> positions = new List<Vector3>();
    private Vector3 lastUpVector = Vector3.up;
    
    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        mesh = new Mesh();
        mesh.name = "Trail3D";
        meshFilter.mesh = mesh;
        
        // Set parent if specified
        if (parentContainer != null)
        {
            transform.SetParent(parentContainer);
        }
        
        // Reset local transform
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        
        if (targetObject != null)
        {
            // Store positions in local space relative to parent
            Vector3 localPos = parentContainer != null ? 
                parentContainer.InverseTransformPoint(targetObject.position) : 
                targetObject.position;
            positions.Add(localPos);
        }
    }
    
    void LateUpdate()
    {
        if (targetObject == null) return;
        
        // Always update the first position to follow the target
        if (positions.Count == 0)
        {
            Vector3 localPos = parentContainer != null ? 
                parentContainer.InverseTransformPoint(targetObject.position) : 
                targetObject.position;
            positions.Add(localPos);
            return;
        }
        
        // Get current target position in local space
        Vector3 currentLocalPos = parentContainer != null ? 
            parentContainer.InverseTransformPoint(targetObject.position) : 
            targetObject.position;
        
        // Check if target has moved enough to add a new segment
        float distanceMoved = Vector3.Distance(currentLocalPos, positions[0]);
        
        if (distanceMoved > minDistance)
        {
            if (isGrowing)
            {
                // Insert new segment at the beginning (most recent position)
                positions.Insert(0, currentLocalPos);
                
                // Remove oldest segment if we exceed max
                if (positions.Count > maxSegments)
                {
                    positions.RemoveAt(positions.Count - 1);
                }
            }
            else if (positions.Count > 0)
            {
                // Not growing - shift all positions and update first one
                for (int i = positions.Count - 1; i > 0; i--)
                {
                    positions[i] = positions[i - 1];
                }
                positions[0] = currentLocalPos;
            }
        }
        
        UpdateMesh();
    }
    
    void UpdateMesh()
    {
        if (positions.Count < 2)
        {
            mesh.Clear();
            return;
        }
        
        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uvs = new List<Vector2>();
        
        // Smooth positions if enabled
        List<Vector3> renderPositions = new List<Vector3>(positions);
        
        if (smoothTrail && positions.Count > 2)
        {
            for (int pass = 0; pass < smoothingPasses; pass++)
            {
                List<Vector3> smoothed = new List<Vector3>(renderPositions);
                for (int i = 1; i < renderPositions.Count - 1; i++)
                {
                    smoothed[i] = (renderPositions[i - 1] + renderPositions[i] + renderPositions[i + 1]) / 3f;
                }
                renderPositions = smoothed;
            }
        }
        
        // Create cylindrical trail mesh with consistent orientation
        for (int i = 0; i < renderPositions.Count; i++)
        {
            // t goes from 0 (newest) to 1 (oldest)
            float t = (float)i / (renderPositions.Count - 1);
            float width = trailWidth * widthCurve.Evaluate(t);
            
            // Calculate direction along the trail
            Vector3 forward;
            if (i < renderPositions.Count - 1)
            {
                forward = (renderPositions[i + 1] - renderPositions[i]).normalized;
            }
            else
            {
                forward = (renderPositions[i] - renderPositions[i - 1]).normalized;
            }
            
            // Use consistent up vector to prevent twisting
            Vector3 right = Vector3.Cross(forward, lastUpVector).normalized;
            if (right.sqrMagnitude < 0.001f)
            {
                right = Vector3.Cross(forward, Vector3.forward).normalized;
            }
            
            Vector3 up = Vector3.Cross(right, forward).normalized;
            lastUpVector = up; // Store for next iteration to maintain consistency
            
            // Create ring of vertices around this position
            for (int j = 0; j <= radialSegments; j++)
            {
                float angle = (float)j / radialSegments * Mathf.PI * 2f;
                Vector3 offset = (right * Mathf.Cos(angle) + up * Mathf.Sin(angle)) * width;
                
                // Positions are stored in local space, so add them directly
                vertices.Add(renderPositions[i] + offset);
                uvs.Add(new Vector2((float)j / radialSegments, t));
            }
        }
        
        // Create triangles connecting the rings
        for (int i = 0; i < renderPositions.Count - 1; i++)
        {
            int ringStart = i * (radialSegments + 1);
            int nextRingStart = (i + 1) * (radialSegments + 1);
            
            for (int j = 0; j < radialSegments; j++)
            {
                int a = ringStart + j;
                int b = ringStart + j + 1;
                int c = nextRingStart + j;
                int d = nextRingStart + j + 1;
                
                triangles.Add(a);
                triangles.Add(c);
                triangles.Add(b);
                
                triangles.Add(b);
                triangles.Add(c);
                triangles.Add(d);
            }
        }
        
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
    
    // Call this to stop the trail from growing
    public void StopGrowing()
    {
        isGrowing = false;
    }
    
    // Call this to resume growing
    public void StartGrowing()
    {
        isGrowing = true;
    }
    
    // Clear the trail
    public void ClearTrail()
    {
        positions.Clear();
        if (targetObject != null)
        {
            Vector3 localPos = parentContainer != null ? 
                parentContainer.InverseTransformPoint(targetObject.position) : 
                targetObject.position;
            positions.Add(localPos);
        }
        mesh.Clear();
        lastUpVector = Vector3.up;
    }
}