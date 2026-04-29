using UnityEngine;
using UnityEngine.InputSystem;
using Minis;
using UnityEngine.U2D;
using Unity.Mathematics;
using System.Collections;

public class BackgroundFaderController : MonoBehaviour
{
    public int channel = 1;
    public int noteNum = 40;
    public float scaleSpeed = 0.1f;
    public float scaleMultiplier = 1.5f;

    private SpriteShapeRenderer spriteRenderer;
    private float ogHue, ogSat, ogBright, dBright;
    private Vector3 ogScale;
    private float trueScaleSpeed => scaleSpeed * 0.001f;

    private float scale;
    MidiDevice midiDevice;
    // Start is called before the first frame update
    void Start()
    {
        InputSystem.onDeviceChange += MidiDeviceSettingUp;
        spriteRenderer = gameObject.GetComponent<SpriteShapeRenderer>();

        // set what the original hsb is
        Color.RGBToHSV(spriteRenderer.color, out ogHue, out ogSat, out ogBright);
        dBright = ogBright * 0.5f;
        scale = 1;

        ogScale = transform.localScale;
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
        var _channel = ((MidiDevice)note.device).channel;
        var _pitch = note.noteNumber;
        Debug.Log($"Note ON\tChannel: {_channel}\tPitch: {_pitch}\tVelocity: {velocity}");
        if (channel == _channel && _pitch == noteNum)
        {
            StartCoroutine(DelayedMakeBig());
        }
    }
    IEnumerator DelayedMakeBig()
    {
        yield return new WaitForSeconds(0.03f);
        scale = scaleMultiplier;
        dBright = ogBright; 
    }

    void onNoteOff (MidiNoteControl note) {

        var channel = ((MidiDevice)note.device).channel;
        var pitch = note.noteNumber;
    }

    void OnApplicationQuit()
    {
        InputSystem.onDeviceChange -= MidiDeviceSettingUp;
        if (midiDevice == null) return;
        midiDevice.onWillNoteOn -= onNoteOn;
        midiDevice.onWillNoteOff -= onNoteOff;
    }

    // Update is called once per frame
    void Update()
    {

        if (scale > 1)
        {
            scale -= scaleSpeed * Time.deltaTime;
        }
        else
        {
            scale = 1;
        }

        transform.localScale = new Vector3(ogScale.x * scale, ogScale.y * scale, ogScale.z * scale);
        // dBright = math.remap(1, scaleMultiplier, ogBright * 0.5f, ogBright, scale);
        // if (transform.localScale.x > scale)
        // {
        //     transform.localScale -= new Vector3(trueScaleSpeed, trueScaleSpeed, trueScaleSpeed) * Time.deltaTime;
        // }
        // dBright = processingMap(transform.localScale.x, scale, scale * scaleMultiplier, ogBright * 0.5f, ogBright);
        // spriteRenderer.color = Color.HSVToRGB(ogHue, ogSat, dBright);
    }
}
