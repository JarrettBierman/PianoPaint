using System.Collections.Generic;
using UnityEngine;

public class DrumNodeController : MonoBehaviour
{

    [Header("Repel Nodes")]
    public float minRepelDistance = 2f;
    public float repelStrength = 15f;
    public float maxRepelForce = 100f;
    public List<Drum> mappedDrums;
    
    private Rigidbody2D rb;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Rigidbody2D[] allBodies = Object.FindObjectsByType<Rigidbody2D>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        Vector2 totalForce = Vector2.zero;

        foreach (var other in allBodies)
        {
            if (other == rb) continue;

            Vector2 offset = rb.position - other.position;
            float distance = offset.magnitude;

            if (distance < minRepelDistance && distance > 0f)
            {
                Vector2 direction = offset.normalized;

                float strength = (minRepelDistance - distance) / minRepelDistance;
                totalForce += direction * strength;
            }
        }

        totalForce = Vector2.ClampMagnitude(totalForce * repelStrength, maxRepelForce);

        ApplyForce(totalForce);
    }

    public void ApplyForce(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Force);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, minRepelDistance);
    }
}
