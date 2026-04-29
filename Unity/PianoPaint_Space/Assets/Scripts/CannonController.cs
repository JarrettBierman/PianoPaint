using System;
using UnityEngine;

public class CannonController : MonoBehaviour
{
    public GameObject beamPrefab;
    public GameObject beamPartPrefab;
    public float timeBetweenShots = 10f;
    bool shooting;
    public float time = 0;

    private GameObject currentBeam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shooting = false;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (shooting && time >= timeBetweenShots)
        {
            ShootBullet();
            time = 0;
        }
    }

    public void SetShooting(bool value)
    {
        shooting = value;

        if (shooting)
        {
            time = timeBetweenShots;
            currentBeam = Instantiate(beamPrefab);
        }
        else
        {
            ShootBullet();
            currentBeam.GetComponent<BeamController>().doneShooting = true;
        }

    }

    void ShootBullet()
    {
        var spawnedBullet = Instantiate(beamPartPrefab, currentBeam.transform);
        spawnedBullet.transform.position = transform.position;
        spawnedBullet.GetComponent<BeamPartController>().direction = transform.up;
    }

    void OnDrawGizmos()
    {
        if (shooting)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.white;
        }
        Gizmos.DrawRay(transform.position, transform.up);
    }
}
