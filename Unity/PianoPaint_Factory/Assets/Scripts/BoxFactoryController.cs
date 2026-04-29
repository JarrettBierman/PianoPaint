using UnityEngine;
using UnityEngine.InputSystem;
using Minis;
public class BoxFactoryController : MonoBehaviour
{
    MidiDevice midiDevice;
    public int activeChannel;
    public BoxFactory[] factories;
    public int index = 0;
    void Start()
    {
        InputSystem.onDeviceChange += MidiDeviceSettingUp;
        factories = GetComponentsInChildren<BoxFactory>();
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
            int trueIndex = index % factories.Length;
            factories[trueIndex].Createbox();
            index++;
        }
        
    }

    void onNoteOff(MidiNoteControl note)
    {

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
