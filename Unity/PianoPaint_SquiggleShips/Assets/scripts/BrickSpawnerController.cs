using UnityEngine;
using UnityEngine.InputSystem;
using Minis;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.VisualScripting;

public class BrickSpawnerController : MonoBehaviour
{
    [Header("Brick data")]
    public GameObject brickPrefab;
    public float startingSpeed = 10f;

    [Header("MIDI")]
    public int midiActiveChannel = 1;
    public int minMidiNote = 23;
    public int maxMidiNote = 109;

    [Header("Color and Size")]
    public Gradient gradient;
    public AnimationCurve sizeCurve;

    [Header("Spawning")]
    public float spawningWidth = 15f;

    private MidiDevice midiDevice;
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
        if (Application.isPlaying)
        {
            var channel = ((MidiDevice)note.device).channel;
            var pitch = note.noteNumber;

            Debug.Log($"Note ON\tChannel: {channel}\tPitch: {pitch}\tVelocity: {velocity}");
            
            if (channel == midiActiveChannel)
            {
                float x = math.remap(minMidiNote, maxMidiNote, -spawningWidth, spawningWidth, pitch);
                if (x < -spawningWidth) x = -spawningWidth;
                if (x > spawningWidth) x = spawningWidth;

                Vector3 position = transform.position + transform.right * x;

                var brick = Instantiate(brickPrefab, position, transform.rotation * Quaternion.Euler(0, 0, 0));
                var brickRb = brick.GetComponent<Rigidbody2D>();
                brickRb.linearVelocityY = startingSpeed;

                Color c = gradient.Evaluate(math.remap(minMidiNote, maxMidiNote, 0, 1, pitch));
                var brickSr = brick.GetComponent<SpriteRenderer>();
                brickSr.color = c;
                var brickTrail = brick.GetComponent<TrailRenderer>();
                brickTrail.startColor = c;

                var scaleFactor = sizeCurve.Evaluate(velocity);
                brick.transform.localScale = Vector3.one * scaleFactor;
            }
            
        }
    }

    void onNoteOff (MidiNoteControl note) { }

    void OnApplicationQuit()
    {
        InputSystem.onDeviceChange -= MidiDeviceSettingUp;
        if (midiDevice == null) return;
        midiDevice.onWillNoteOn -= onNoteOn;
        midiDevice.onWillNoteOff -= onNoteOff;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        var origMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(spawningWidth * 2, 1, 1));
        Gizmos.matrix = origMatrix;
    }
}
