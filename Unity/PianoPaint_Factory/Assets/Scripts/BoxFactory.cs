using UnityEngine;


public class BoxFactory : MonoBehaviour
{

    public GameObject boxPrefab;
    public Vector3 initalSpeed;
    public float rotationSpeed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void Createbox()
    {
        var box = Instantiate(boxPrefab, transform.position, Quaternion.identity);

        var rb = box.GetComponent<Rigidbody2D>();
        rb.linearVelocity = initalSpeed;
        rb.angularVelocity = rotationSpeed;

        var randomHue = Random.Range(0.5f, 0.75f);
        var randomSat = Random.Range(0.5f, 1f);

        var sr = box.GetComponent<SpriteRenderer>();
        sr.color = Color.HSVToRGB(randomHue, randomSat, 1);

        var tr = box.GetComponent<TrailRenderer>();
        tr.colorGradient = GetGradient(randomHue, randomSat);

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
