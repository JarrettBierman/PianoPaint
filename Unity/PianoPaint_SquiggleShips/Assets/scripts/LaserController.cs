using UnityEngine;
using UnityEngine.InputSystem;
using Minis;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.VisualScripting;

public class LaserController : MonoBehaviour
{
    [Header("MIDI")]
    public int midiActiveChannel = 1;

    [Header("Prefab")]
    public GameObject BeamPrefab;

    [Header("Spawining")]
    public float xOffset = 9f;
    public float spawingHeight = 5f;
    public int minMidiNote = 25;
    public int maxMidiNote = 105;

    private Dictionary<int, Transform> beamMap;
    private MidiDevice midiDevice;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputSystem.onDeviceChange += MidiDeviceSettingUp;
        beamMap = new();
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
                // only add one if nothing exists
                if(!beamMap.ContainsKey(pitch))
                {
                    var y = math.remap(minMidiNote, maxMidiNote, -spawingHeight, spawingHeight, pitch);
                    Vector3 position = transform.position + transform.up * y;
                    var newBeam = Instantiate(BeamPrefab, position, transform.rotation * Quaternion.Euler(0, 0, 90));
                    beamMap.Add(pitch, newBeam.transform);
                }
            }
            
        }
    }

    void onNoteOff (MidiNoteControl note)
    {
        if (Application.isPlaying)
        {
            var channel = ((MidiDevice)note.device).channel;
            var pitch = note.noteNumber;
            if (channel == midiActiveChannel)
            {
                if (beamMap.TryGetValue(pitch, out Transform beam))
                {
                    beam.gameObject.GetComponent<BeamController>().DestroyAndKeepBeam();
                    beamMap.Remove(pitch);
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        InputSystem.onDeviceChange -= MidiDeviceSettingUp;
        if (midiDevice == null) return;
        midiDevice.onWillNoteOn -= onNoteOn;
        midiDevice.onWillNoteOff -= onNoteOff;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        var origMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(1, spawingHeight * 2, 1));
        Gizmos.matrix = origMatrix;
    }
}
