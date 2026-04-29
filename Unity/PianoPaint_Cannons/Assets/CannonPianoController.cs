using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using Minis;

public class CannonPianoController : MonoBehaviour
{
    CannonShootingController[] cannons;
    MidiDevice midiDevice;
    public float minVelocity = 10;
    public float maxVelocity = 100;
    void Start()
    {
        InputSystem.onDeviceChange += MidiDeviceSettingUp;
        cannons = GetComponentsInChildren<CannonShootingController>();
    }

    void FireRandom()
    {
        var index = UnityEngine.Random.Range(0, cannons.Length - 1);
        cannons[index].Fire(50f);
    }
    void FireFromNote(int pitch, float velocity)
    {
        var index = pitch % cannons.Length;
        cannons[index].Fire(velocity);
    }
    void onNoteOn(MidiNoteControl note, float velocity)
    {
        var channel = ((MidiDevice)note.device).channel;
        var pitch = note.noteNumber;
        if (channel != 0)
        {
            // Debug.Log($"Note ON\tChannel: {channel}\tPitch: {pitch}\tVelocity: {velocity}");
            float cannonVelocity = math.remap(0, 1, minVelocity, maxVelocity, velocity);
            FireFromNote(pitch, cannonVelocity);
        }
    }

    void onNoteOff (MidiNoteControl note) {

        // var channel = ((MidiDevice)note.device).channel;
        // var pitch = note.noteNumber;
        // Debug.Log($"Note OFF\tChannel: {channel}\tPitch: {pitch}");
    }


    void MidiDeviceSettingUp(InputDevice device, InputDeviceChange change)
    {
        if (change != InputDeviceChange.Added) return;
        midiDevice = device as MidiDevice;
        if (midiDevice == null) return;

        midiDevice.onWillNoteOn += onNoteOn;
        midiDevice.onWillNoteOff += onNoteOff;
    }
    void OnApplicationQuit() {
        InputSystem.onDeviceChange -= MidiDeviceSettingUp;
        if (midiDevice == null) return;
        midiDevice.onWillNoteOn -= onNoteOn;
        midiDevice.onWillNoteOff -= onNoteOff;
    }
}
