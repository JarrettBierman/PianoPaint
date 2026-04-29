using UnityEngine;
using UnityEngine.InputSystem;
using Minis;

public class LauncherController : MonoBehaviour
{
    public int activeChannel = 1;
    public int activePitch = 36;
    public float speed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    MidiDevice midiDevice;
    public bool pressed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputSystem.onDeviceChange += MidiDeviceSettingUp;
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
        if (channel == activeChannel && pitch == activePitch)
        {
            pressed = true;
        }
        
    }

    void onNoteOff(MidiNoteControl note)
    {

        var channel = ((MidiDevice)note.device).channel;
        var pitch = note.noteNumber;
        if (channel == activeChannel && pitch == activePitch)
        {
            pressed = false;
        }
        // Debug.Log($"Note OFF\tChannel: {channel}\tPitch: {pitch}");
    }

    void OnApplicationQuit() {
        InputSystem.onDeviceChange -= MidiDeviceSettingUp;
        if (midiDevice == null) return;
        midiDevice.onWillNoteOn -= onNoteOn;
        midiDevice.onWillNoteOff -= onNoteOff;
    }

    public void OnCollisionEnter2D(Collision2D other)
    {
        HandleCollision(other);
    }

    public void OnCollisionStay2D(Collision2D other)
    {
        HandleCollision(other);
    }

    void HandleCollision(Collision2D other)
    {
        if (other.rigidbody != null)
        {
            other.rigidbody.WakeUp();
            if (pressed)
            {
                Debug.Log("pressing");
                var rb = other.gameObject.GetComponent<Rigidbody2D>();
                rb.angularVelocity = Random.Range(-100, 100);
                rb.AddForce(Vector2.up * speed);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
