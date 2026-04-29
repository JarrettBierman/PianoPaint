using UnityEngine;
using Minis;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Linq;
using System;
using Unity.VisualScripting;
public class ConeDrumController : MonoBehaviour
{
    [Header("MIDI Information and Mappings")]
    public int activeChannel;
    public Drum[] ClockwiseSpinDrums;
    public Drum[] CounterClockwiseSpinDrums;
    public Drum[] VerticalMovementDrums;
    public Drum[] ColorChangeDrums;
    public Drum[] SpikeAnimationDrums;
    public Drum[] SizeIncreaseDrums;

    [Header("Rotation Spin Settings")]
    public float RotationDeltaDegrees = 15f;
    public float RotationForce = 10f;

    [Header("Vertical Movement Settings")]
    public float VerticalMovementSpeed = 1f;
    public Vector3 VerticalPositionOffset;

    [Header("Size Increase Settings")]
    public float SizeChangeSpeed = 5f;
    public float MaxSizeScale = 1.7f;



    private MidiDevice midiDevice;


    // public Vector3 positionOffset;
    private Vector3 verticalFrom;
    private Vector3 verticalTo;
    private float verticalLerpValue;

    private float sizeFrom;
    private float sizeTo;
    private float sizeIncreaseLerpValue;
    private Rigidbody rb;

    private Animator animator;

    void Start()
    {
        InputSystem.onDeviceChange += MidiDeviceSettingUp;

        verticalFrom = transform.position;
        verticalTo = transform.position + VerticalPositionOffset;
        verticalLerpValue = 0;

        sizeFrom = transform.localScale.x;
        sizeTo = sizeFrom * MaxSizeScale;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if(verticalLerpValue > 0)
        {
            verticalLerpValue -= VerticalMovementSpeed * Time.deltaTime;
        }
        else if (verticalLerpValue < 0)
        {
            verticalLerpValue = 0;
        }

        if (sizeIncreaseLerpValue > 0)
        {
            sizeIncreaseLerpValue -= SizeChangeSpeed * Time.deltaTime;
        }
        else if (sizeIncreaseLerpValue < 0)
        {
            sizeIncreaseLerpValue = 0;
        }
    }

    void LateUpdate()
    {
        transform.position = Vector3.Lerp(verticalFrom, verticalTo, verticalLerpValue);

        var scaleVal = Mathf.Lerp(sizeFrom, sizeTo, sizeIncreaseLerpValue);
        transform.localScale = new Vector3(scaleVal, transform.localScale.y, scaleVal);
    }

    void MidiDeviceSettingUp(InputDevice device, InputDeviceChange change)
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
                if (ClockwiseSpinDrums.Contains((Drum)pitch))
                {
                    // Vector3 currentRotation = transform.localEulerAngles;
                    // transform.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y + 15, currentRotation.z);
                    rb.AddTorque(Vector3.up * RotationForce, ForceMode.Impulse);
                }
                else if (CounterClockwiseSpinDrums.Contains((Drum)pitch))
                {
                    // Vector3 currentRotation = transform.localEulerAngles;
                    // transform.localEulerAngles = new Vector3(currentRotation.x, currentRotation.y - 15, currentRotation.z);
                    rb.AddTorque(Vector3.up * -RotationForce, ForceMode.Impulse);
                }
                else if (VerticalMovementDrums.Contains((Drum)pitch))
                {
                    verticalLerpValue = velocity;
                }
                else if (ColorChangeDrums.Contains((Drum)pitch))
                {
                    var newColor = UnityEngine.Random.ColorHSV(0, 1, 0.7f, 1f, 0.5f, 1f);
                    var material = gameObject.GetComponent<Renderer>().material;
                    material.color = newColor;
                    sizeIncreaseLerpValue = velocity * 0.3f;
                }
                else if (SpikeAnimationDrums.Contains((Drum)pitch))
                {
                    animator.Play("Left", 0, 0);
                    animator.Play("Right", 1, 0);
                }
                else if (SizeIncreaseDrums.Contains((Drum)pitch))
                {
                    sizeIncreaseLerpValue = velocity;
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
            if (channel == activeChannel) { }
        }
    }

    void OnApplicationQuit()
    {
        InputSystem.onDeviceChange -= MidiDeviceSettingUp;
        if (midiDevice == null) return;
        midiDevice.onWillNoteOn -= onNoteOn;
        midiDevice.onWillNoteOff -= onNoteOff;
    }
}
