using UnityEngine;
using UnityEngine.InputSystem;
using Minis;
using Unity.Mathematics;

public class SparkController : MonoBehaviour
{
    public GameObject sparkPrefab;
    public int activeChannel = 5;
    float minX, maxX;
    float minY, maxY;

    MidiDevice midiDevice;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputSystem.onDeviceChange += MidiDeviceSettingUp;

        // Get the main camera
        Camera cam = Camera.main;

        // Get screen bounds in world coordinates
        Vector3 bottomLeft = cam.ScreenToWorldPoint(new Vector3(0, 0, cam.nearClipPlane));
        Vector3 topRight = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, cam.nearClipPlane));

        // Individual edges
        minX = bottomLeft.x;
        maxX = topRight.x;
        minY = bottomLeft.y;
        maxY = topRight.y;
    }

    // Update is called once per frame
    void Update()
    {

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
        // Debug.Log($"Note ON\tChannel: {channel}\tPitch: {pitch}\tVelocity: {velocity}");
        if (channel == activeChannel)
        {
            var x = math.remap(25, 105, minX, maxX, pitch);
            var y = UnityEngine.Random.Range(minY, maxY);

            var spark = Instantiate(sparkPrefab, new Vector3(x, y), Quaternion.identity);
            // var sparkPs = spark.GetComponent<ParticleSystem>();

            // Gradient gradient = new Gradient();
            // var color1 = UnityEngine.Random.ColorHSV(0, 1, 0.5f, 0.5f, 1, 1);
            // Color.RGBToHSV(color1, out float h, out float s, out float v);

            // // Shift hue by 180 degrees (0.5 in Unity's 0-1 hue range)
            // float oppositeHue = (h + 0.5f) % 1f;

            // var color2 = Color.HSVToRGB(oppositeHue, s, v);

            // gradient.SetKeys(
            //     new GradientColorKey[] { new GradientColorKey(color1, 0.0f), new GradientColorKey(color2, 1.0f) },
            //     new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            // );
            // sparkPs.main.startColor = new ParticleSystem.MinMaxGradient(gradient);
        }
    }

    void onNoteOff (MidiNoteControl note) {

        // var channel = ((MidiDevice)note.device).channel;
        // var pitch = note.noteNumber;
        // Debug.Log($"Note OFF\tChannel: {channel}\tPitch: {pitch}");
    }

    void OnApplicationQuit() {
        InputSystem.onDeviceChange -= MidiDeviceSettingUp;
        if (midiDevice == null) return;
        midiDevice.onWillNoteOn -= onNoteOn;
        midiDevice.onWillNoteOff -= onNoteOff;
    }
}
