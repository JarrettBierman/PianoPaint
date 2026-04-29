using UnityEngine;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using Minis;

public class PlatformFactoryController : MonoBehaviour
{
    public int activeChannel = 1;
    public int activePitch = 43;
    public float speed = 5f;
    public bool debugMode = false;
    public GameObject platformPrefab;
    MidiDevice midiDevice;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputSystem.onDeviceChange += MidiDeviceSettingUp;
    }

    // Update is called once per frame
    void Update()
    {

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
            var velocityYOffset = math.remap(0, 1, -0.5f, 0.5f, velocity);
            Vector3 updatedPosition = new Vector3(transform.position.x, transform.position.y + velocityYOffset);
            var platform = Instantiate(platformPrefab, updatedPosition, Quaternion.identity);
            platform.GetComponent<platformController>().speed = speed;
        }
        if(debugMode)
            Debug.Log($"Note ON\tChannel: {channel}\tPitch: {pitch}\tVelocity: {velocity}");
    }

    void onNoteOff (MidiNoteControl note) {

        var channel = ((MidiDevice)note.device).channel;
        var pitch = note.noteNumber;
        // Debug.Log($"Note OFF\tChannel: {channel}\tPitch: {pitch}");
    }

    void OnApplicationQuit() {
        InputSystem.onDeviceChange -= MidiDeviceSettingUp;
        if (midiDevice == null) return;
        midiDevice.onWillNoteOn -= onNoteOn;
        midiDevice.onWillNoteOff -= onNoteOff;
    }
}
