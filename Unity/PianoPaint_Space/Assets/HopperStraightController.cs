using UnityEngine;
using UnityEngine.InputSystem;
using Minis;
using System.Collections;
using Unity.Collections;

public class HopperStraightController : MonoBehaviour
{
    public int activeChannel = 2;
    public float minX = 1;
    public float minY = 1;
    public float maxX = 8;
    public float maxY = 8;
    public float angle = 45;
    public float startingSpeed = 10f;
    public float cruisingSpeed = 5f;
    public float decel = 0.1f;

    private int previousNote = -1;
    private float speed;
    private Vector2 direction;
    private int numNotesPlaying = 0;
    private enum MoveState { REST, START, CRUISE, SLOW }
    private MoveState state;


    MidiDevice midiDevice;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputSystem.onDeviceChange += MidiDeviceSettingUp;
        direction = new Vector2(1, 1);
        state = MoveState.REST;
    }

    // Update is called once per frame
    void Update()
    {
        if (state == MoveState.START)
        {
            speed -= decel * Time.deltaTime;
            if (speed <= cruisingSpeed)
            {
                state = MoveState.CRUISE;
            }
        }
        if (state == MoveState.CRUISE)
        {
            speed = cruisingSpeed;
        }
        if (state == MoveState.SLOW)
        {
            if (speed > 0)
            {
                speed -= decel * Time.deltaTime;
            }
            else
            {
                speed = 0;
                state = MoveState.REST;
            }
        }

        if (transform.position.x < minX || transform.position.x > maxX ||
            transform.position.y < minY || transform.position.y > maxY)
        {
            if (state != MoveState.REST && state != MoveState.START)
            {
                state = MoveState.SLOW;
            }
        }

        transform.Translate(direction * speed * Time.deltaTime);
    }
    void HandleDirction(int pitch)
    {
        if (transform.position.x > maxX) direction = Vector3.left;
        else if (transform.position.x < minX) direction = Vector3.right;
        else if (transform.position.y > maxY) direction = Vector3.down;
        else if (transform.position.y < minY) direction = Vector3.up;

        else if (pitch > previousNote) direction = Vector3.right;
        else if (pitch < previousNote) direction = Vector3.left;
        else if (Random.value < 0.5) direction = Vector3.up;
        else direction = Vector3.down;

        float randomAngle = Random.Range(-angle, angle);
        direction = (Quaternion.AngleAxis(randomAngle, Vector3.forward) * direction).normalized;

        previousNote = pitch;
    }
    void MidiDeviceSettingUp(InputDevice device, InputDeviceChange change)
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
            numNotesPlaying++;
            HandleDirction(pitch);
            speed = startingSpeed;
            state = MoveState.START;
        }
    }

    void onNoteOff(MidiNoteControl note)
    {

        var channel = ((MidiDevice)note.device).channel;
        var pitch = note.noteNumber;
        if (channel == activeChannel)
        {
            numNotesPlaying--;
            if (numNotesPlaying <= 0)
            {
                state = MoveState.SLOW;
            }
        }
    }
    void OnApplicationQuit()
    {
        InputSystem.onDeviceChange -= MidiDeviceSettingUp;
        if (midiDevice == null) return;
        midiDevice.onWillNoteOn -= onNoteOn;
        midiDevice.onWillNoteOff -= onNoteOff;
    }
    Vector2 GetNextDirection()
    {
        float randomAngle = Random.Range(-angle, angle);
        Vector3 randomDirection = Quaternion.AngleAxis(randomAngle, Vector3.forward) * direction;
        return randomDirection.normalized;
        // return (transform.position + randomDirection).normalized;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(minX, minY), new Vector2(maxX, minY));
        Gizmos.DrawLine(new Vector2(maxX, minY), new Vector2(maxX, maxY));
        Gizmos.DrawLine(new Vector2(maxX, maxY), new Vector2(minX, maxY));
        Gizmos.DrawLine(new Vector2(minX, maxY), new Vector2(minX, minY));

        Gizmos.color = Color.yellow;
        var rotatedUp = Quaternion.AngleAxis(angle, Vector3.forward) * direction.normalized;
        Gizmos.DrawRay(transform.position, rotatedUp.normalized);
        var rotatedDown = Quaternion.AngleAxis(-angle, Vector3.forward) * direction.normalized;
        Gizmos.DrawRay(transform.position, rotatedDown.normalized);
        
        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, direction.normalized);
    }
}
