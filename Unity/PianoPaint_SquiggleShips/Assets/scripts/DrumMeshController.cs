using UnityEngine;
using UnityEngine.InputSystem;
using Minis;
using Unity.Mathematics;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using System.Linq;

public class DrumMeshController : MonoBehaviour
{
    public GameObject prefab;
    public int numNodes = 100;
    public float springDistance = 3f;
    public float springDistanceRandomness = 1f;
    private List<GameObject> DrumNodes;


    [Header("MIDI")]
    public int midiActiveChannel = 1;

    public float maxMagnitude = 1000f;

    private Mesh mesh;
    private MidiDevice midiDevice;

    void Awake()
    {
        mesh = new Mesh { name = "DynamicPolygonMesh2D" };
        GetComponent<MeshFilter>().mesh = mesh;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputSystem.onDeviceChange += MidiDeviceSettingUp;

        DrumNodes = new List<GameObject>();
        for (int i = 0; i < numNodes; i++)
        {
            var angle = i * Math.PI * 2f / numNodes;
            Vector2 position = (Vector2)transform.position + new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 5;
            var node = Instantiate(prefab, position, Quaternion.identity, transform);
            var joint2d = node.GetComponent<SpringJoint2D>();
            joint2d.connectedBody = GetComponent<Rigidbody2D>();
            joint2d.distance = springDistance + UnityEngine.Random.Range(-springDistanceRandomness, springDistanceRandomness);
            DrumNodes.Add(node);
        }

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
                DrumNodes = DrumNodes.OrderBy(t =>
                {
                    Vector2 d = (Vector2)(t.transform.position - transform.position);
                    return Mathf.Atan2(d.y, d.x);
                })
                .ToList();

                int index = UnityEngine.Random.Range(0, 10);
                int r = 2;
                for (int i = 0; i < 3; i++)
                {
                    var leftNode = DrumNodes[(index - i) % DrumNodes.Count];
                    Vector2 outDirectionVector = (leftNode.transform.position - transform.position).normalized;
                    Vector2 perpendicularVector = new Vector2(-outDirectionVector.y, outDirectionVector.x).normalized;
                    perpendicularVector *= math.remap(0, 1, -1, 1, UnityEngine.Random.value);
                    float magnitude = maxMagnitude * velocity;
                    Vector2 force = (outDirectionVector + perpendicularVector) * magnitude * (r + 1.0f/i+r);
                    leftNode.GetComponent<DrumNodeController>().ApplyForce(force);

                    var rightNode = DrumNodes[(index + i) % DrumNodes.Count];
                    outDirectionVector = (rightNode.transform.position - transform.position).normalized;
                    perpendicularVector = new Vector2(-outDirectionVector.y, outDirectionVector.x).normalized;
                    perpendicularVector *= math.remap(0, 1, -1, 1, UnityEngine.Random.value);
                    magnitude = maxMagnitude * velocity;
                    force = (outDirectionVector + perpendicularVector) * magnitude * (r + 1.0f/i+r);
                    rightNode.GetComponent<DrumNodeController>().ApplyForce(force);
                }
                // bool playedFoundNote = Enum.IsDefined(typeof(Drum), pitch);
                // if (playedFoundNote)
                // {
                //     // get the drum node
                //     GameObject node = DrumNodes.FirstOrDefault(x => x.GetComponent<DrumNodeController>().mappedDrums.Contains((Drum)pitch));
                //     DrumNodeController controller = node.GetComponent<DrumNodeController>();

                //     Vector2 outDirectionVector = (node.transform.position - transform.position).normalized;
                //     Vector2 perpendicularVector = new Vector2(-outDirectionVector.y, outDirectionVector.x).normalized;
                //     perpendicularVector *= math.remap(0, 1, -1, 1, UnityEngine.Random.value);

                //     float magnitude = maxMagnitude * velocity;
                //     Vector2 force = (outDirectionVector + perpendicularVector) * magnitude;
                //     controller.ApplyForce(force);
                // }
            }
            
        }
    }
    void onNoteOff (MidiNoteControl note) { }

    void LateUpdate()
    {
        // 1) Sort nodes by angle around THIS transform (world space order is fine)
        var ordered = DrumNodes
            .Select(o => o.transform)
            .Where(t => t != null)
            .OrderBy(t =>
            {
                Vector2 d = (Vector2)(t.position - transform.position);
                return Mathf.Atan2(d.y, d.x);
            })
            .ToArray();

        if (ordered.Length < 3) return;

        // 2) Build vertices
        Vector3[] verts = new Vector3[ordered.Length];
        Vector2[] uvs   = new Vector2[ordered.Length];

        // (Simple UVs: project into 0..1 using WORLD bounds)
        var positions2D = ordered.Select(t => (Vector2)t.position).ToArray();
        float minX = positions2D.Min(p => p.x);
        float maxX = positions2D.Max(p => p.x);
        float minY = positions2D.Min(p => p.y);
        float maxY = positions2D.Max(p => p.y);
        float invW = Mathf.Approximately(maxX - minX, 0f) ? 0f : 1f / (maxX - minX);
        float invH = Mathf.Approximately(maxY - minY, 0f) ? 0f : 1f / (maxY - minY);

        for (int i = 0; i < ordered.Length; i++)
        {
            // ✅ Convert WORLD -> LOCAL of the mesh object (this fixes parent movement shifting)
            Vector3 localP = transform.InverseTransformPoint(ordered[i].position);
            localP.z = 0f;
            verts[i] = localP;

            Vector2 p2 = positions2D[i];
            uvs[i] = new Vector2((p2.x - minX) * invW, (p2.y - minY) * invH);
        }

        // 3) Triangles for a convex polygon (triangle fan from vertex 0)
        int n = verts.Length;
        int[] tris = new int[(n - 2) * 3];
        int ti = 0;
        for (int i = 1; i < n - 1; i++)
        {
            tris[ti++] = 0;
            tris[ti++] = i;
            tris[ti++] = i + 1;
        }

        // 4) Apply mesh
        mesh.Clear();
        mesh.vertices = verts;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    // void FixedUpdate()
    // {
    //     transform.Rotate(0f, 0f, 90f * Time.deltaTime);
    // }


    void OnApplicationQuit()
    {
        InputSystem.onDeviceChange -= MidiDeviceSettingUp;
        if (midiDevice == null) return;
        midiDevice.onWillNoteOn -= onNoteOn;
        midiDevice.onWillNoteOff -= onNoteOff;
    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.5f);
    }
}
