using UnityEngine;
using UnityEngine.InputSystem;
using Minis;

public class PianoBeamerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    MidiDevice midiDevice;
    public float rotateSpeed = 1;
    public float speed = 1;

    public float minX = 1;
    public float minY = 1;
    public float maxX = 8;
    public float maxY = 8;
    public int activeChannel = 5;

    private Vector3 direction;
    private CannonController[] cannons;
    void Start()
    {
        InputSystem.onDeviceChange += MidiDeviceSettingUp;
        direction = new Vector2(Random.value, Random.value).normalized * speed;
        cannons = gameObject.GetComponentsInChildren<CannonController>();
    }

    // Update is called once per frame
    void Update()
    {
        // rotate it
        transform.Rotate(new Vector3(0, 0, rotateSpeed) * Time.deltaTime);

        // move it
        transform.position += direction * Time.deltaTime;
        if (transform.position.x < minX || transform.position.x > maxX)
        {
            direction.x = -direction.x;
        }
        if (transform.position.y < minY || transform.position.y > maxY)
        {
            direction.y = -direction.y;
        }

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
            var cannonIndex = pitch % cannons.Length;
            cannons[cannonIndex].SetShooting(true);
            // Debug.Log($"Note ON\tChannel: {channel}\tPitch: {pitch}\tVelocity: {velocity}");
        }
    }

    void onNoteOff (MidiNoteControl note) {

        var channel = ((MidiDevice)note.device).channel;
        var pitch = note.noteNumber;
        if (channel == activeChannel)
        {
            var cannonIndex = pitch % cannons.Length;
            cannons[cannonIndex].SetShooting(false);
            // Debug.Log($"Note OFF\tChannel: {channel}\tPitch: {pitch}");
        }
    }

    void OnApplicationQuit() {
        InputSystem.onDeviceChange -= MidiDeviceSettingUp;
        if (midiDevice == null) return;
        midiDevice.onWillNoteOn -= onNoteOn;
        midiDevice.onWillNoteOff -= onNoteOff;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.purple;
        Gizmos.DrawLine(new Vector2(minX, minY), new Vector2(maxX, minY));
        Gizmos.DrawLine(new Vector2(maxX, minY), new Vector2(maxX, maxY));
        Gizmos.DrawLine(new Vector2(maxX, maxY), new Vector2(minX, maxY));
        Gizmos.DrawLine(new Vector2(minX, maxY), new Vector2(minX, minY));
    }
}
