using UnityEngine;

public class ConveryerBeltController : MonoBehaviour
{
    public float speed = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnCollisionStay2D(Collision2D other)
    {
        if (other.rigidbody != null)
        {
            Vector3 movement = transform.right * speed * Time.deltaTime;
            other.gameObject.GetComponent<Rigidbody2D>().MovePosition(other.gameObject.transform.position + movement);
        }
    }
}
