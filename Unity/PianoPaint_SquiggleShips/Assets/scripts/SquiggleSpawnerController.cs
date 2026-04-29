using UnityEngine;
using UnityEngine.InputSystem;
using Minis;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SquiggleSpawnerController : MonoBehaviour
{
    [Header("General")]
    public GameObject squigglePrefab;
    public int activeChannel = 1;
    public int minMidiNote = 23;
    public int maxMidiNote = 109;

    [Header("Location & Size")]
    public float spawningWidth = 15f;
    public float maxSize = 1f;
    public AnimationCurve sizeCurve;
    public float verticalSpeed = 15f;
    public float horizontalSpeed = 5f;
    public float destroyDistance = 60f;

    [Header("Color")]
    public Gradient colorGradient;
    public int colorGradientLength = 36;
    



    private Dictionary<int, HeadController> squiggleData;
    private MidiDevice midiDevice;
    // Start is called before the first frame update
    void Start()
    {
        InputSystem.onDeviceChange += MidiDeviceSettingUp;
        squiggleData = new();
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
            if (channel == activeChannel)
            {
                Debug.Log($"Note ON\tChannel: {channel}\tPitch: {pitch}\tVelocity: {velocity}");

                float x = math.remap(minMidiNote, maxMidiNote, -spawningWidth, spawningWidth, pitch);
                if (x < -spawningWidth) x = -spawningWidth;
                if (x > spawningWidth) x = spawningWidth;

                Vector3 position = transform.position + transform.right * x;

                var squiggle = Instantiate(squigglePrefab, position, transform.rotation);
                var trailSettings = squiggle.GetComponentInChildren<Trail3D>();

                trailSettings.parentContainer = squiggle.transform;
                trailSettings.targetObject = squiggle.GetComponentInChildren<HeadController>().transform;

                // var color = colorGradient.Evaluate(math.remap(minMidiNote, maxMidiNote, 0, 1, pitch));
                var color = colorGradient.Evaluate(math.remap(0, colorGradientLength, 0, 1, pitch % colorGradientLength));
                
                SetSquiggleColor(squiggle, color);

                squiggle.transform.localScale = Vector3.one * sizeCurve.Evaluate(velocity) * maxSize;

                squiggle.GetComponentInChildren<HeadController>().speed = verticalSpeed;
                squiggle.GetComponent<SquiggleMover>().speed = horizontalSpeed;
                squiggle.GetComponent<SquiggleMover>().destroyThreshhold = destroyDistance;

                squiggleData[pitch] = squiggle.GetComponentInChildren<HeadController>();
            }
            
        }
    }

    void onNoteOff (MidiNoteControl note)
    {
        if (Application.isPlaying)
        {
            var channel = ((MidiDevice)note.device).channel;
            var pitch = note.noteNumber;
            if (channel == activeChannel)
            {
                Debug.Log($"Note OFF\tChannel: {channel}\tPitch: {pitch}");
                HeadController headController = squiggleData[pitch];
                headController.moving = false;
                
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

    void SetSquiggleColor(GameObject squiggle, Color color)
    {
        // change the color of the squiggle
        var renderers = squiggle.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            if (renderer != null)
            {
                Material material = renderer.material;
                material.color = color;
                // Set emission to a darker version of the color
                Color emissionColor = color * 0.85f;
                material.SetColor("_EmissionColor", emissionColor);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;

        var origMatrix = Gizmos.matrix;

        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawWireCube(Vector3.zero, new Vector3(spawningWidth * 2, 2, 2));

        Gizmos.matrix = origMatrix;
    }
}
