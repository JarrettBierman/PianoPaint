using Cinemachine;
using UnityEngine;
[RequireComponent(typeof(CinemachineFreeLook))]
public class CameraMoveScript : MonoBehaviour
{
    [Header("Swivel Settings")]
    public float maxAngle = 30f;   // + / - degrees
    public float speed = 0.5f;     // oscillations per second
    private CinemachineFreeLook controller;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        controller = GetComponent<CinemachineFreeLook>();
    }

    // Update is called once per frame
    void Update()
    {
        float t = Mathf.Sin(Time.time * speed * Mathf.PI * 2f);
        controller.m_XAxis.Value = t * maxAngle;
    }
}
