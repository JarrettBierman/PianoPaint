using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DestroyOnCollision : MonoBehaviour
{
    public GameObject flarePrefab;
    public Vector3 velocity;
    public float velocityMultiplier = 1f;
    public Color color;

    void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
        SpawnFlare();
    }

    void OnParticleCollision(GameObject other)
    {
        Destroy(gameObject);
        SpawnFlare();
    }

    void SpawnFlare()
    {
        var flare = Instantiate(flarePrefab, transform.position, transform.rotation);
        var ps = flare.GetComponent<ParticleSystem>();
        var psMain = ps.main;
        psMain.startColor = color;

        var gradient = new Gradient();
        gradient.alphaKeys = new GradientAlphaKey[2] {new GradientAlphaKey(1, 0), new GradientAlphaKey(0, 1)};
        gradient.colorKeys = new GradientColorKey[1] { new GradientColorKey(color, 0) };

        var colorOverTime = ps.colorOverLifetime;
        colorOverTime.color = new ParticleSystem.MinMaxGradient(gradient);

        var velocityOverTime = ps.velocityOverLifetime;
        velocityOverTime.x = velocity.x * velocityMultiplier;
        velocityOverTime.y = velocity.y * velocityMultiplier;
        velocityOverTime.z = velocity.z * velocityMultiplier;
    }
}
