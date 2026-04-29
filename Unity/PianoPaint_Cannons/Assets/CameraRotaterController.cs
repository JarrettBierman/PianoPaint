using Unity.Cinemachine;
using UnityEngine;

public class CameraRotaterController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed = 10;
    CinemachineOrbitalFollow follow;
    void Start()
    {
        follow = GetComponent<CinemachineOrbitalFollow>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        follow.HorizontalAxis.Value += speed * Time.deltaTime % 360;
    }
}
