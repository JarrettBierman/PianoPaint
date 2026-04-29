using System.Collections;
using UnityEngine;

public class DestroyScript : MonoBehaviour
{
    public float delay = 5f;
    void Start()
    {
        Destroy(gameObject, delay);
    }
}
