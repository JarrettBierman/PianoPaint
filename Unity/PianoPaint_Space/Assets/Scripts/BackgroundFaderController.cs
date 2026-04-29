using UnityEngine;
using UnityEngine.InputSystem;
using Minis;

public class BackgroundFaderController : MonoBehaviour
{
    public int channel = 1;
    public int noteNum = 40;
    public float scaleSpeed = 0.1f;
    public float scaleMultiplier = 1.5f;

    private SpriteRenderer spriteRenderer;
    private float ogHue, ogSat, ogBright, dBright;
    private float trueScaleSpeed => scaleSpeed * 0.001f;

    private float startingScale;
    MidiDevice midiDevice;
    // Start is called before the first frame update
    void Start()
    {
        InputSystem.onDeviceChange += MidiDeviceSettingUp;
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        // set what the original hsb is
        Color.RGBToHSV(spriteRenderer.color, out ogHue, out ogSat, out ogBright);
        dBright = ogBright * 0.5f;
        startingScale = transform.localScale.x;
    }

    void MidiDeviceSettingUp(InputDevice device, InputDeviceChange change)
    {
        if (change != InputDeviceChange.Added) return;
        midiDevice = device as MidiDevice;
        if (midiDevice == null) return;

        midiDevice.onWillNoteOn += onNoteOn;
        // midiDevice.onWillNoteOff += onNoteOff;
    }

    void onNoteOn(MidiNoteControl note, float velocity)
    {
        var _channel = ((MidiDevice)note.device).channel;
        var _pitch = note.noteNumber;
        Debug.Log($"Note ON\tChannel: {_channel}\tPitch: {_pitch}\tVelocity: {velocity}");
        if (channel == _channel && _pitch == noteNum)
        {
            float scaleTo = startingScale * scaleMultiplier;
            transform.localScale = new Vector3(scaleTo, scaleTo, scaleTo);
            dBright = ogBright;
        }
    }

    // void onNoteOff (MidiNoteControl note) {

    //     var channel = ((MidiDevice)note.device).channel;
    //     var pitch = note.noteNumber;
    //     Debug.Log($"Note OFF\tChannel: {channel}\tPitch: {pitch}");
    // }

    void OnApplicationQuit()
    {
        InputSystem.onDeviceChange -= MidiDeviceSettingUp;
        if (midiDevice == null) return;
        midiDevice.onWillNoteOn -= onNoteOn;
        // midiDevice.onWillNoteOff -= onNoteOff;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.localScale.x > startingScale)
        {
            transform.localScale -= new Vector3(trueScaleSpeed, trueScaleSpeed, trueScaleSpeed) * Time.deltaTime;
        }
        dBright = processingMap(transform.localScale.x, startingScale, startingScale * scaleMultiplier, ogBright * 0.5f, ogBright);
        spriteRenderer.color = Color.HSVToRGB(ogHue, ogSat, dBright);
    }

    public static float processingMap(float input, float oldRangeMin, float oldRangeMax, float newRangeMin, float newRangeMax)
    {
        float t = Mathf.InverseLerp(oldRangeMin, oldRangeMax, input);
        return Mathf.Lerp(newRangeMin, newRangeMax, t);
    }
}
