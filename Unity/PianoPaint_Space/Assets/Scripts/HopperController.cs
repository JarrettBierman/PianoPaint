using UnityEngine;
using UnityEngine.InputSystem;
using Minis;
using System.Collections;
using UnityEngine.Rendering;

public class HopperController : MonoBehaviour
{
    public int activeChannel = 2;
    public float minDistance = 1;
    public float maxDistance = 10;
    public float angle = 45;
    public Vector2 direction = new Vector2(1, 1);
    public float minSpeed = 1;
    public float maxSpeed = 5;
    public float dSpeed = 0.1f;
    public float arcHeight = 3;
    public float minX = 1;
    public float minY = 1;
    public float maxX = 8;
    public float maxY = 8;

    public Color color;
    


    private enum HopState { LANDED, ASCEND, DESCEND }
    MidiDevice midiDevice;
    float speed = 1;
    int previousNote = -1;

    HopState state = HopState.LANDED;

    private Vector3 nextLocation;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextLocation = GetNextLocation();
        InputSystem.onDeviceChange += MidiDeviceSettingUp;
    }

    void HandleDirction(int pitch)
    {
        if      (transform.position.x > maxX)    direction = Vector3.left;
        else if (transform.position.x < minX)    direction = Vector3.right;
        else if (transform.position.y > maxY)    direction = Vector3.down;
        else if (transform.position.y < minY)    direction = Vector3.up;
        
        else if (pitch > previousNote)          direction = Vector3.right;
        else if (pitch < previousNote)          direction = Vector3.left;
        else if (Random.value < 0.5)            direction = Vector3.up;
        else                                    direction = Vector3.down;

        previousNote = pitch;
    }

    void HandleDistance(float velocity)
    {
        maxDistance = BackgroundFaderController.processingMap(velocity, 0, 1, 1, 3);
        arcHeight = BackgroundFaderController.processingMap(velocity, 0, 1, 0.1f, 3f);
        if (Random.value < 0.5f)
        {
            arcHeight *= -1;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    Vector2 GetNextLocation()
    {
        float randomAngle = Random.Range(-angle, angle);
        float randomDistance = Random.Range(minDistance, maxDistance);
        Vector3 randomDirection = Quaternion.AngleAxis(randomAngle, Vector3.forward) * direction.normalized * randomDistance;
        return transform.position + randomDirection;
    }
    void MidiDeviceSettingUp (InputDevice device, InputDeviceChange change) 
    {
        if (change != InputDeviceChange.Added) return;
        midiDevice = device as MidiDevice;
        if (midiDevice == null) return;

        midiDevice.onWillNoteOn += onNoteOn;
        midiDevice.onWillNoteOff += onNoteOff;
    }

    void onNoteOn(MidiNoteControl note, float velocity)
    {
        var channel = ((MidiDevice)note.device).channel;
        var pitch = note.noteNumber;
        if (channel == activeChannel)
        {
            // movement
            HandleDirction(pitch);
            HandleDistance(velocity);
            nextLocation = GetNextLocation();
            MoveToTarget(nextLocation);
            speed = maxSpeed;

            // state
            state = HopState.ASCEND;
            
        }
    }

    void onNoteOff(MidiNoteControl note)
    {

        var channel = ((MidiDevice)note.device).channel;
        var pitch = note.noteNumber;
        if (channel == activeChannel)
        {
            if (state == HopState.ASCEND)
            {
                state = HopState.DESCEND;
            }
        }
    }

    public void MoveToTarget(Vector3 targetPosition)
    {
        speed = maxSpeed;
        StartCoroutine(MoveOnBezierArc(transform.position, targetPosition));
    }
    IEnumerator MoveOnBezierArc(Vector3 startPos, Vector3 endPos)
    {
        Vector3 midPoint = (endPos + startPos) * 0.5f;
        Vector3 controlPoint = midPoint + Vector3.up * arcHeight;
        float elapsedTime = 0f;
        while (elapsedTime < 1)
        {
            float t = elapsedTime;
            Vector3 currentPos = CalculateBezierPoint(startPos, controlPoint, endPos, t);
            transform.position = currentPos;
            elapsedTime += Time.deltaTime * speed;
            yield return null;
        }
        transform.position = endPos;
        state = HopState.LANDED;
    }
    Vector3 CalculateBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        float u = 1 - t;
        return u * u * p0 + 2 * u * t * p1 + t * t * p2;
    }

    void OnApplicationQuit() {
        InputSystem.onDeviceChange -= MidiDeviceSettingUp;
        if (midiDevice == null) return;
        midiDevice.onWillNoteOn -= onNoteOn;
        midiDevice.onWillNoteOff -= onNoteOff;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(minX, minY), new Vector2(maxX, minY));
        Gizmos.DrawLine(new Vector2(maxX, minY), new Vector2(maxX, maxY));
        Gizmos.DrawLine(new Vector2(maxX, maxY), new Vector2(minX, maxY));
        Gizmos.DrawLine(new Vector2(minX, maxY), new Vector2(minX, minY));

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minDistance);

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(nextLocation, 0.1f);

        Gizmos.color = Color.yellow;
        var rotatedUp = Quaternion.AngleAxis(angle, Vector3.forward) * direction.normalized;
        Gizmos.DrawRay(transform.position, rotatedUp.normalized * maxDistance);
        var rotatedDown = Quaternion.AngleAxis(-angle, Vector3.forward) * direction.normalized;
        Gizmos.DrawRay(transform.position, rotatedDown.normalized * maxDistance);
        
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, direction.normalized * maxDistance);
    }
}
