using System;
using UnityEngine;
using Unity.Mathematics;


public class HeadController : MonoBehaviour
{
    public float speed = 1f;
    public float stoppingSpeed = 0.05f;
    public float noiseScale = 0.1f;
    public bool moving = true;

    private float xOffset, yOffset, zOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        xOffset = UnityEngine.Random.Range(0, 1000);
        yOffset = UnityEngine.Random.Range(0, 1000);
        zOffset = UnityEngine.Random.Range(0, 1000);
    }

    // Update is called once per frame
    void Update()
    {
        var xNoise = Mathf.PerlinNoise1D(xOffset);
        var yNoise = Mathf.PerlinNoise1D(yOffset);
        var zNoise = Mathf.PerlinNoise1D(zOffset);

        var mappedX = math.remap(0, 1, -1, 1, xNoise);
        var mappedY = math.remap(0, 1, 0, 0.75f, yNoise);
        var mappedZ = math.remap(0, 1, -1, 1, zNoise);

        Vector3 movement = new Vector3(mappedX, mappedY, mappedZ);
        transform.position += movement * speed * Time.deltaTime;

        xOffset += noiseScale;
        yOffset += noiseScale;

        if (!moving && speed > 0)
        {
            speed -= stoppingSpeed * Time.deltaTime;

            if (speed <= 0)
            {
                speed = 0;
            }
        }
    }
}
