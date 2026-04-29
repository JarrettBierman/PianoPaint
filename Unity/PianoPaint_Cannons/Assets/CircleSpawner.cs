using UnityEngine;

public class CircleSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject prefabToSpawn;
    public int numberOfObjects = 8;
    public float diameter = 10f;
    
    [Header("Optional Settings")]
    public bool spawnOnStart = true;
    public Vector3 rotationOffset = Vector3.zero;
    
    void Awake()
    {
        if (spawnOnStart)
        {
            SpawnObjects();
        }
    }
    
    [ContextMenu("Spawn Objects")]
    public void SpawnObjects()
    {
        if (prefabToSpawn == null)
        {
            Debug.LogError("No prefab assigned to spawn!");
            return;
        }
        
        float radius = diameter / 2f;
        float angleStep = 360f / numberOfObjects;
        
        for (int i = 0; i < numberOfObjects; i++)
        {
            // Calculate angle in radians
            float angle = i * angleStep * Mathf.Deg2Rad;
            
            // Calculate position on circle
            Vector3 spawnPosition = new Vector3(
                transform.position.x + radius * Mathf.Cos(angle),
                transform.position.y,
                transform.position.z + radius * Mathf.Sin(angle)
            );
            
            // Spawn the object
            GameObject spawnedObject = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity, transform);
            
            // Make objects face the center
            Vector3 directionToCenter = (transform.position - spawnPosition).normalized;
            spawnedObject.transform.LookAt(transform.position);
            
            // Apply rotation offset if needed
            if (rotationOffset != Vector3.zero)
            {
                spawnedObject.transform.Rotate(rotationOffset);
            }
            
            // Parent to this object for organization
            spawnedObject.transform.SetParent(transform);
        }
    }
    
    [ContextMenu("Clear Spawned Objects")]
    public void ClearSpawnedObjects()
    {
        // Remove all child objects
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            else
            {
                DestroyImmediate(transform.GetChild(i).gameObject);
            }
        }
    }
    
    // Visualize the circle in Scene view
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, diameter / 2f);
        
        // Draw spawn points
        if (numberOfObjects > 0)
        {
            Gizmos.color = Color.red;
            float radius = diameter / 2f;
            float angleStep = 360f / numberOfObjects;
            
            for (int i = 0; i < numberOfObjects; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                Vector3 position = new Vector3(
                    transform.position.x + radius * Mathf.Cos(angle),
                    transform.position.y,
                    transform.position.z + radius * Mathf.Sin(angle)
                );
                Gizmos.DrawWireSphere(position, 0.2f);
            }
        }
    }
}