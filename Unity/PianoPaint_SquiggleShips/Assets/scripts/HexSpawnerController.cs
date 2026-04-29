using UnityEngine;
using UnityEngine.InputSystem;
using Minis;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;

public class HexSpawnerController : MonoBehaviour
{
    public GameObject hexPrefab;

    [Header("MIDI")]
    public int midiActiveChannel = 1;

    [Range(0f, 10f)]
    public float kickMaxSize = 1f;
    [Range(0f, 10f)]
    public float snareMaxSize = 1f;
    [Range(0f, 10f)]
    public float hithatMaxSize = 1f;
    [Range(0f, 10f)]
    public float rideMaxSize = 1f;
    [Range(0f, 10f)]
    public float crashMaxSize = 1f;
    [Range(0f, 10f)]
    public float tomMaxSize = 1f;
    
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
                if (Enum.IsDefined(typeof(Drum), pitch))
                {
                    Drum hit = (Drum)pitch;

                    Color c = Color.white;
                    float posX = 0;
                    float posY = 0;
                    float scaleX = 1;
                    float scaleY = 1;
                    float duration = 1;

                    if (hit == Drum.KICK)
                    {
                        c = Color.red;
                        posY = UnityEngine.Random.Range(-9f, 0f);
                        scaleX = kickMaxSize * velocity;
                        scaleY = scaleX * UnityEngine.Random.Range(0.5f, 2f);
                        duration = 2.0f;
                    }
                    else if (hit == Drum.SNARE || hit == Drum.SNARE_RIM)
                    {
                        c = Color.pink;
                        posY = UnityEngine.Random.Range(-4f, 4f);
                        scaleX = snareMaxSize * velocity;
                        scaleY = scaleX * UnityEngine.Random.Range(0.5f, 2f);
                        duration = 1.5f;
                    }
                    else if (hit == Drum.HIHAT_CLOSED || hit == Drum.HIHAT_OPEN || hit == Drum.HIHAT_FOOT)
                    {
                        c = Color.orange;
                        posY = UnityEngine.Random.Range(2f, 9f);
                        scaleX = hithatMaxSize * velocity;
                        scaleY = scaleX * UnityEngine.Random.Range(0.5f, 2f);
                        duration = 0.5f;
                    }
                    else if (hit == Drum.RIDE)
                    {
                        c = Color.lightGreen;
                        posY = UnityEngine.Random.Range(0f, 6f);
                        scaleX = rideMaxSize * velocity;
                        scaleY = scaleX * UnityEngine.Random.Range(0.5f, 2f);
                        duration = 1.0f;
                    }
                    else if (hit == Drum.CRASH)
                    {
                        c = Color.coral;
                        posY = UnityEngine.Random.Range(-8f, 8f);
                        scaleX = crashMaxSize * velocity;
                        scaleY = scaleX * UnityEngine.Random.Range(0.5f, 2f);
                        duration = 3f;
                    }
                    else if (hit == Drum.TOM_HIGH || 
                             hit == Drum.TOM_HIGH_RIM || 
                             hit == Drum.TOM_MID || 
                             hit == Drum.TOM_MID_RIM || 
                             hit == Drum.TOM_LOW || 
                             hit == Drum.TOM_LOW_RIM)
                    {
                        c = UnityEngine.Random.ColorHSV(0f, 1f, 0.5f, 1f, 1f, 1f);
                        posY = UnityEngine.Random.Range(-8f, 8f);
                        scaleX = tomMaxSize * velocity;
                        scaleY = scaleX * UnityEngine.Random.Range(0.5f, 2f);
                        duration = 2f;
                    }

                    posX = UnityEngine.Random.Range(-17f, 17f);
                    var rotationSpeed = UnityEngine.Random.Range(-200, 200);

                    var hex = Instantiate(hexPrefab, new Vector3(posX, posY, 0), Quaternion.Euler(0, 0, UnityEngine.Random.Range(0f, 360f)));
                    hex.transform.localScale = new Vector3(scaleX, scaleY, 1);
                    hex.GetComponent<SpriteRenderer>().color = c;
                    hex.GetComponent<HexagonController>().rotationSpeed = rotationSpeed;
                    hex.GetComponent<HexagonController>().fadeDuration = duration;
                    hex.GetComponent<HexagonController>().growRate = UnityEngine.Random.Range(0.01f, 0.5f);
                }
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
        Gizmos.color = Color.purple;
    }
}
