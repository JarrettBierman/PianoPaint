using UnityEngine;
using UnityEngine.InputSystem;
using Minis;
using Unity.Mathematics;

public class BallFactory : MonoBehaviour
{

    MidiDevice midiDevice;
    public int activeChannel;
    public GameObject ballPrefab;
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
        var channel = ((MidiDevice)note.device).channel;
        var pitch = note.noteNumber;
        if (channel == activeChannel)
        {
            Debug.Log($"Note OFF\tChannel: {channel}\tPitch: {pitch}");
            CreateBall(pitch, velocity);
        }
        
    }

    public void CreateBall(int pitch, float velocity)
    {
        int fixedPitch = pitch % 44;
        float x = math.remap(0, 44, -9, 9, fixedPitch);
        float y = 5.1f;
        float vx = UnityEngine.Random.Range(-1, 1);
        float vy = math.remap(0, 1, 0f, 2f, velocity);

        var ball = Instantiate(ballPrefab, new Vector3(x, y), Quaternion.identity);
        ball.transform.localScale = Vector3.one * velocity * 0.75f;

        var rb = ball.GetComponent<Rigidbody2D>();
        rb.linearVelocity = new Vector2(vx, -vy);
        rb.angularVelocity = UnityEngine.Random.Range(-50, 50);

        var randomHue = UnityEngine.Random.Range(0.875f, 1.125f) % 1f;
        var randomSat = UnityEngine.Random.Range(0.5f, 1f);

        var sr = ball.GetComponent<SpriteRenderer>();
        sr.color = Color.HSVToRGB(randomHue, randomSat, 1);

        var tr = ball.GetComponent<TrailRenderer>();
        tr.colorGradient = GetGradient(randomHue, randomSat);
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

    Gradient GetGradient(float h, float s)
    {
        Gradient gradient = new();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(Color.HSVToRGB(h, s, 0.75f), 0);
        colorKeys[1] = new GradientColorKey(Color.HSVToRGB(h, s, 0.25f), 0);

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1, 0);
        alphaKeys[1] = new GradientAlphaKey(1, 1);

        gradient.SetKeys(colorKeys, alphaKeys);
        return gradient;
    }
}
