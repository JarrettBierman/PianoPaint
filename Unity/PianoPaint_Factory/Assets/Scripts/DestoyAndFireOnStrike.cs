using UnityEngine;
using UnityEngine.InputSystem;
using Minis;

public class DestoyAndFireOnStrike : MonoBehaviour
{
    public int activeChannel = 1;
    public int activePitch = 41;
    MidiDevice midiDevice;
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
        if (channel == activeChannel && pitch == activePitch)
        {
            var balls = GameObject.FindGameObjectsWithTag("Ball");
            var boxes = GameObject.FindGameObjectsWithTag("Box");
            var triangles = GameObject.FindGameObjectsWithTag("Triangle");

            foreach (GameObject ball in balls) Destroy(ball);
            foreach (GameObject box in boxes) Destroy(box);
            foreach (GameObject triangle in triangles) Destroy(triangle);
        }
        
    }

    void onNoteOff(MidiNoteControl note)
    {

        var channel = ((MidiDevice)note.device).channel;
        var pitch = note.noteNumber;
    }

    void OnApplicationQuit() {
        InputSystem.onDeviceChange -= MidiDeviceSettingUp;
        if (midiDevice == null) return;
        midiDevice.onWillNoteOn -= onNoteOn;
        midiDevice.onWillNoteOff -= onNoteOff;
    }
}
