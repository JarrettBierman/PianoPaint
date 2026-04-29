using UnityEngine;
using UnityEngine.InputSystem;
using Minis;
using System.Linq;
using UnityEngine.U2D;

public class BackgroundColorController : MonoBehaviour
{
    public int activeChannel;
    public int[] activeNotes;
    public Camera CameraObject;
    public GameObject[] BackgroundObjects;
    public Color color;

    MidiDevice midiDevice;


    void Start()
    {
        InputSystem.onDeviceChange += MidiDeviceSettingUp;
    }

    void UpdateColors()
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        Color darkerVersion = Color.HSVToRGB(h, s, v * 0.5f);

        CameraObject.backgroundColor = darkerVersion;
        foreach (var obj in BackgroundObjects)
        {
            var foundRenderer = obj.TryGetComponent<SpriteRenderer>(out var renderer);
            if (foundRenderer)
            {
                renderer.color = color;
            }
            else
            {
                var renderer2 = obj.GetComponent<SpriteShapeRenderer>();
                renderer2.color = color;
            }
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
        if (channel == activeChannel && activeNotes.Contains(pitch))
        {
            color = Random.ColorHSV(0, 1, 0, 1, 0.5f, 1);
            UpdateColors();
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
