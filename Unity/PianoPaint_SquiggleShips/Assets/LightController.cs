using UnityEngine;

public class LightController : MonoBehaviour
{
    [Header("X Axis (Simple Harmonic Motion)")]
    public float minX = -30f;
    public float maxX = 30f;
    public float xFrequency = 0.2f; // cycles per second

    [Header("Y Axis (Constant Rotation)")]
    public float yDegreesPerSecond = 5f;

    private float xMid;
    private float xAmplitude;
    private float time;
    private float yAngle; // stored Y angle (KEY FIX)

    void Start()
    {
        xMid = (minX + maxX) * 0.5f;
        xAmplitude = (maxX - minX) * 0.5f;

        yAngle = transform.eulerAngles.y;
    }

    void Update()
    {
        time += Time.deltaTime;

        // Smooth SHM on X
        float xRotation =
            xMid + xAmplitude * Mathf.Sin(2f * Mathf.PI * xFrequency * time);

        // Stable constant rotation on Y
        yAngle += yDegreesPerSecond * Time.deltaTime;

        transform.rotation = Quaternion.Euler(xRotation, yAngle, 0f);
    }
}
