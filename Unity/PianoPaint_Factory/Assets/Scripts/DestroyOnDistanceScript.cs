using UnityEngine;

public class DestroyOnDistanceScript : MonoBehaviour
{
    public float yMix = -20;
    
    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < yMix)
        {
            Destroy(gameObject);
        }
    }
}
