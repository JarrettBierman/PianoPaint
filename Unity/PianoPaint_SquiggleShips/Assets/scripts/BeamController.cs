using UnityEngine;

public class BeamController : MonoBehaviour
{
    private ParticleSystem ps;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ps = GetComponent<ParticleSystem>();
    }

    public void DestroyAndKeepBeam()
    {
        if (ps != null)
        {
            Debug.Log("ABOUT TO DESTROY");
            ps.transform.SetParent(null, true);
            ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            Destroy(ps.gameObject, ps.main.duration + ps.main.startLifetime.constantMax);
        }
        Destroy(gameObject, 3);
    }
}
