using System.Collections;
using UnityEngine;

public class CrystalController : MonoBehaviour
{
    [Header("Speeds")]
    public float riseSpeed = 2f;
    public float fallSpeed = 2f;

    [Header("Timing")]
    public float waitTime = 1f;

    [Header("Easing distances (world units)")]
    public float slowDownDistance = 0.5f; // last X units before surface: slow to 0
    public float speedUpTime = 0.25f;     // NEW: time to ramp up the fall (seconds)

    [Header("Stop snap tolerance")]
    public float stopEpsilon = 0.002f;

    public Transform groundTransform;         
    private Transform iceburgPoint;                 

    private enum State { Rising, Waiting, Falling }
    private State state = State.Rising;

    private float fallStartTime;          // NEW
    private Coroutine waitRoutine;

    void Awake()
    {
        if (iceburgPoint == null) iceburgPoint = transform.GetChild(0);
    }

    void Update()
    {
        if (iceburgPoint == null || groundTransform == null) return;

        Vector3 axis = transform.up;
        float surfaceY = groundTransform.position.y;

        if (state == State.Rising)
        {
            float distToSurface = surfaceY - iceburgPoint.position.y;

            if (distToSurface <= stopEpsilon)
            {
                SnapTipToSurface(axis, surfaceY);

                state = State.Waiting;
                if (waitRoutine == null) waitRoutine = StartCoroutine(WaitThenFall());
                return;
            }

            float speedFactor = 1f;
            if (distToSurface < slowDownDistance)
            {
                speedFactor = Mathf.Clamp01(distToSurface / slowDownDistance);
                speedFactor = Mathf.SmoothStep(0f, 1f, speedFactor);
            }

            float speed = riseSpeed * speedFactor;

            // clamp to not overshoot
            float maxStep = MaxStepAlongAxisForDeltaY(axis, distToSurface);
            float step = Mathf.Min(speed * Time.deltaTime, maxStep);

            transform.position += axis * step;
        }
        else if (state == State.Falling)
        {
            // NEW: ease-in based on time since falling started
            float t = (Time.time - fallStartTime) / Mathf.Max(0.0001f, speedUpTime); // 0 -> 1
            float speedFactor = Mathf.Clamp01(t);
            speedFactor = Mathf.SmoothStep(0f, 1f, speedFactor);

            float speed = fallSpeed * speedFactor;
            transform.position -= axis * (speed * Time.deltaTime);

            if (transform.position.y < -10f)
                Destroy(gameObject);
        }
    }

    IEnumerator WaitThenFall()
    {
        Debug.Log("Coroutine started");
        yield return new WaitForSecondsRealtime(waitTime);
        Debug.Log("1");

        fallStartTime = Time.time;   // NEW
        state = State.Falling;

        waitRoutine = null;
        Debug.Log("2");
    }

    private void SnapTipToSurface(Vector3 axis, float surfaceY)
    {
        float axisY = axis.y;
        if (Mathf.Abs(axisY) < 0.0001f) return;

        float deltaY = surfaceY - iceburgPoint.position.y;
        float moveAlongAxis = deltaY / axisY;

        transform.position += axis * moveAlongAxis;
    }

    private float MaxStepAlongAxisForDeltaY(Vector3 axis, float deltaYToSurface)
    {
        float axisY = axis.y;
        if (Mathf.Abs(axisY) < 0.0001f) return 0f;

        float maxStep = deltaYToSurface / axisY;
        return Mathf.Max(0f, maxStep);
    }

    public void SetIceburgPoint(float val)
    {
        transform.GetChild(0).localPosition += Vector3.up * val;
    }

    public void SetGroundTranform(Transform t)
    {
        groundTransform = t;
    }
}
