using UnityEngine;

public class DestroyOnTime : MonoBehaviour
{
    public float time = 5;
    void Start()
    {
        Destroy(gameObject, time);
    }

}
