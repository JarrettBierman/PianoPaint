using UnityEngine;
using UnityEngine.InputSystem;
using Minis;

public class MidiScriptBase : MonoBehaviour
{
    MidiDevice midiDevice;
    // Start is called before the first frame update
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
        Debug.Log($"Note ON\tChannel: {channel}\tPitch: {pitch}\tVelocity: {velocity}");
    }

    void onNoteOff (MidiNoteControl note) {

        var channel = ((MidiDevice)note.device).channel;
        var pitch = note.noteNumber;
        Debug.Log($"Note OFF\tChannel: {channel}\tPitch: {pitch}");
    }

    void OnApplicationQuit() {
        InputSystem.onDeviceChange -= MidiDeviceSettingUp;
        if (midiDevice == null) return;
        midiDevice.onWillNoteOn -= onNoteOn;
        midiDevice.onWillNoteOff -= onNoteOff;
    }
}