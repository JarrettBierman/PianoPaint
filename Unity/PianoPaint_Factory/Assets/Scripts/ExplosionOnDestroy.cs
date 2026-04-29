using UnityEngine;

public class ExplosionOnDestroy : MonoBehaviour
{
    public GameObject explosionPrefab;
    private Color color;

    void Start()
    {
        color = gameObject.GetComponent<SpriteRenderer>().color;
    }

    void OnDestroy()
    {
        var explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        var ps = explosion.GetComponent<ParticleSystem>();
        var main = ps.main;
        main.startColor = color;
    }
    
}
