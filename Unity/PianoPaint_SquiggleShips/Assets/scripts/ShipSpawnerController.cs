using UnityEngine;
using UnityEngine.InputSystem;
using Minis;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.VisualScripting;

public class ShipSpawnerController : MonoBehaviour
{
    [Header("Ship data")]
    public GameObject shipPrefab;
    public float shipMaxSpeed = 10f;
    public float shipTurnForce = 10f;
    public float shipNoiseScale = 0.01f;

    [Header("MIDI")]
    public int midiActiveChannel = 1;

    [Header("Color and Size")]
    public Gradient gradient;
    public AnimationCurve sizeCurve;

    [Header("Spawning")]
    public float spawningWidth = 15f;
    public int minMidiNote = 23;
    public int maxMidiNote = 109;
    public bool disableSpawnOutsideOfMidiZone;

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
                // check if we are in the spawning bounds
                if (disableSpawnOutsideOfMidiZone && (pitch < minMidiNote || pitch > maxMidiNote))
                {
                    return;
                }
                
                float x = math.remap(minMidiNote, maxMidiNote, -spawningWidth, spawningWidth, pitch);
                if (x < -spawningWidth) x = -spawningWidth;
                if (x > spawningWidth) x = spawningWidth;

                Vector3 position = transform.position + transform.right * x;

                var ship = Instantiate(shipPrefab, position, transform.rotation);
                var shipSettings = ship.GetComponent<ShipController>();
                shipSettings.maxSpeed = shipMaxSpeed;
                shipSettings.turnForce = shipTurnForce;
                shipSettings.noiseScale = shipNoiseScale;
                shipSettings.color = gradient.Evaluate(math.remap(minMidiNote, maxMidiNote, 0, 1, pitch));

                var scaleFactor = sizeCurve.Evaluate(velocity);
                ship.transform.localScale = new Vector3(0.7f * scaleFactor, scaleFactor, scaleFactor);
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
