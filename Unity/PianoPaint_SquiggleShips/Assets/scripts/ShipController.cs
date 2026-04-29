using System;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class ShipController : MonoBehaviour
{
    public float maxSpeed = 10f;
    public float turnForce = 5f;
    // public float maxSteerForce = 20f;
    public float noiseScale = 0.01f;

    public Color color = Color.white;

    public Vector3 test = Vector3.zero;

    public Vector3 _acceleration;
    public Vector3 _velocity;
    private float seed;
    private DestroyOnCollision doc;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        doc = GetComponent<DestroyOnCollision>();
        doc.color = color;
        seed = UnityEngine.Random.Range(-9999, 9999);

        SetColor();

        _velocity = transform.up * maxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        _acceleration = Vector3.zero;

        float steerMagnitude = noise.snoise(new float2(seed));
        Vector3 steer = Vector2.Perpendicular(_velocity).normalized * steerMagnitude * turnForce;
        _acceleration = (_velocity + steer) * maxSpeed;
        _velocity += _acceleration * Time.deltaTime;
        _velocity = Vector3.ClampMagnitude(_velocity, maxSpeed);
        transform.position += _velocity * Time.deltaTime;

        float angle = Mathf.Atan2(_velocity.y, _velocity.x) * Mathf.Rad2Deg;
        // Change this depending on your triangle’s “forward” direction:
        float offsetDegrees = -90f; // triangle points UP
        // float offsetDegrees = 0f; // triangle points RIGHT
        transform.rotation = Quaternion.Euler(0f, 0f, angle + offsetDegrees);

        doc.velocity = _velocity;

        seed += noiseScale * Time.deltaTime;
    }

    void SetColor()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = color;

        var boosterParticles = GetComponentInChildren<ParticleSystem>();
        var main = boosterParticles.main;
        main.startColor = new ParticleSystem.MinMaxGradient(color);


        var gradient = new Gradient();
        gradient.alphaKeys = new GradientAlphaKey[2] {new GradientAlphaKey(1, 0), new GradientAlphaKey(0, 1)};
        gradient.colorKeys = new GradientColorKey[1] { new GradientColorKey(color, 0) };

        var colorOverTime = boosterParticles.colorOverLifetime;
        colorOverTime.color = new ParticleSystem.MinMaxGradient(gradient);
    }

    public void DestroyAndKeepTrail()
    {
        var childPs = GetComponentInChildren<ParticleSystem>();
        if (childPs != null)
        {
            childPs.transform.SetParent(null, true);
            //childPs.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            Destroy(childPs.gameObject, 3);
        }
        Destroy(gameObject);
    }
}
