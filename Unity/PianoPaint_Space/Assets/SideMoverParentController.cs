using UnityEngine;
using UnityEngine.InputSystem;
using Minis;

public class SideMoverParentController : MonoBehaviour
{
    public int activeChannel = 2;
    MidiDevice midiDevice;
    private SideMoverScript[] sideMovers;
    private int numSideMovers;
    void Start()
    {
        InputSystem.onDeviceChange += MidiDeviceSettingUp;
        sideMovers = GetComponentsInChildren<SideMoverScript>();
        numSideMovers = sideMovers.Length;
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
            var randomMoverIndex = Random.Range(0, numSideMovers);
            sideMovers[randomMoverIndex].Fling();
        }
    }
    void onNoteOff (MidiNoteControl note) { }

    void OnApplicationQuit() {
        InputSystem.onDeviceChange -= MidiDeviceSettingUp;
        if (midiDevice == null) return;
        midiDevice.onWillNoteOn -= onNoteOn;
        midiDevice.onWillNoteOff -= onNoteOff;
    }
}
