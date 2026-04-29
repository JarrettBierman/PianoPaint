using UnityEngine;

public class CannonShootingController : MonoBehaviour
{
    public GameObject ballPrefab;
    public GameObject explosionPrefab;
    public Transform barrelTransform;
    public Transform barrelLipTransform;

    public void Fire(float velocity)
    {
        var ball = Instantiate(ballPrefab, barrelTransform.position, Quaternion.identity);

        var rb = ball.GetComponent<Rigidbody>();
        rb.linearVelocity = -barrelTransform.right * velocity;

        var tr = ball.GetComponent<TrailRenderer>();
        tr.colorGradient = GetRandomGradient();

        Instantiate(explosionPrefab, barrelLipTransform.position, Quaternion.identity);
    }
    
    Gradient GetRandomGradient()
    {
        Gradient gradient = new();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(Random.ColorHSV(0, 1, 1, 1, 1, 1), 0);
        colorKeys[1] = new GradientColorKey(Color.white, 1);

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1, 0);
        alphaKeys[1] = new GradientAlphaKey(1, 1);

        gradient.SetKeys(colorKeys, alphaKeys);
        return gradient;
    }
}
