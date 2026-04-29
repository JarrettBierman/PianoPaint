using UnityEngine;
using UnityEngine.InputSystem;
using Minis;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.VisualScripting;

public class CrystalSpawnerController : MonoBehaviour
{
    [Header("Objects")]
    public GameObject crystalPrefab;
    public Transform groundTranform;

    [Header("MIDI data")]
    public int activeChannel = 1;


    [Header("Location")]
    public float spawningWidth = 15f;
    public float spawningLength = 15f;
    public int spawningNoteInterval = 24;
    public int spawningNoteOffset = 0;
    public float downOffset = 4f;
    public float xRandOffset = 0f;
    public float zRandOffset = 0f;

    public float maxSize = 1f;
    public AnimationCurve sizeCurve;
    public float speed = 5f;
    public float zNoiseScale = 0.1f;
    private float zNoiseOffset = 0;

    public Gradient colorGradient;    

    private MidiDevice midiDevice;
    // Start is called before the first frame update
    void Start()
    {
        InputSystem.onDeviceChange += MidiDeviceSettingUp;
        zNoiseOffset = UnityEngine.Random.Range(0, 1000);
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
                // Debug.Log($"Note ON\tChannel: {channel}\tPitch: {pitch}\tVelocity: {velocity}");
                // apply offset
                pitch += spawningNoteOffset;
                // set pos and rotation and instantiate
                float x = math.remap(0, spawningNoteInterval, -spawningWidth, spawningWidth, pitch % spawningNoteInterval) + UnityEngine.Random.Range(-xRandOffset, xRandOffset);

                float zNoise = Mathf.PerlinNoise1D(zNoiseOffset);
                float z = math.remap(0, 1, -spawningLength, spawningLength, zNoise) + UnityEngine.Random.Range(-zRandOffset, zRandOffset);;
                zNoiseOffset += zNoiseScale;

                Vector3 position = transform.position + (transform.right * x) + (transform.forward * z) + (transform.up * -downOffset);
                Quaternion rotation = Quaternion.Euler(UnityEngine.Random.Range(-30, 30), UnityEngine.Random.Range(0, 90), UnityEngine.Random.Range(-30, 30));
                var crystal = Instantiate(crystalPrefab, position, rotation);

                // set the iceburg point and the ground transform
                var controller = crystal.GetComponent<CrystalController>();
                controller.SetGroundTranform(groundTranform);
                float iceburgPoint = math.remap(0, 1, 0.4f, -0.4f, velocity);
                // float iceburgPoint = UnityEngine.Random.Range(-0.4f, 0.3f);
                controller.SetIceburgPoint(iceburgPoint);

                // set color
                var color = colorGradient.Evaluate(math.remap(0, spawningNoteInterval, 0, 1, pitch % spawningNoteInterval));
                SetCrystalColor(crystal, color);
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

    void SetCrystalColor(GameObject crystal, Color color)
    {
        // change the color of the squiggle
        var renderer = crystal.GetComponent<Renderer>();
        renderer.material.color = color;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        var origMatrix = Gizmos.matrix;

        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.DrawWireCube(Vector3.zero, new Vector3(spawningWidth * 2, -downOffset, spawningLength * 2));
        Gizmos.DrawWireSphere(new Vector3(0, -downOffset, 0), 1f);

        Gizmos.matrix = origMatrix;
    }
}
