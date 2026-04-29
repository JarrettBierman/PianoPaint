using UnityEngine;

public class BeamController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private LineRenderer lr;
    public float lifeTime = 10f;
    private float life;
    public bool doneShooting;
    void Start()
    {
        lr = GetComponent<LineRenderer>();
        SetRandomGradient();
        life = 0;
    }

    // Update is called once per frame
    void Update()
    {
        lr.positionCount = transform.childCount;
        for (int i = 0; i < transform.childCount; i++)
        {
            lr.SetPosition(i, transform.GetChild(i).position);
        }
        if (doneShooting)
        {
            life += Time.deltaTime;
        }
        if (life >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    void SetRandomGradient()
    {
        Gradient gradient = new();
        GradientColorKey[] colorKeys = new GradientColorKey[2];
        colorKeys[0] = new GradientColorKey(Random.ColorHSV(0, 1, 1, 1, 1, 1), 0);
        colorKeys[1] = new GradientColorKey(Color.white, 1);

        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
        alphaKeys[0] = new GradientAlphaKey(1, 0);
        alphaKeys[1] = new GradientAlphaKey(1, 1);

        gradient.SetKeys(colorKeys, alphaKeys);
        lr.colorGradient = gradient;
    }

    void OnApplicationQuit()
    {
        Destroy(gameObject);        
    }
}
