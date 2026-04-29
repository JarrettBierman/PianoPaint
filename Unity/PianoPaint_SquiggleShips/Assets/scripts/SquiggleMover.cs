using UnityEngine;

public class SquiggleMover : MonoBehaviour
{
    public float speed = 5f;
    public float destroyThreshhold = 50f;

    private Vector3 origPosition;

    void Start()
    {
        origPosition = transform.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if(Vector3.Distance(transform.position, origPosition) > destroyThreshhold)
        {
            Destroy(gameObject);
        }
    }
}
