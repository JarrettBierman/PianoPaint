using Unity.VisualScripting;
using UnityEngine;

public class DestroyOnDistance : MonoBehaviour
{
    public float distanceX = 10f;
    public float distanceY = 10f;
    public Vector3 target;


    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > distanceX * 0.5f || 
            transform.position.x < -distanceX * 0.5f  || 
            transform.position.y > distanceY * 0.5f  || 
            transform.position.y < -distanceY * 0.5f)
        {
            // Destroy(gameObject);
            GetComponent<ShipController>().DestroyAndKeepTrail();
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(target, new Vector3(distanceX, distanceY, 0));
    }
}
