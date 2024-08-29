using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : ProjectBehaviour
{
    public int lvlIndex = 0;
    [SerializeField] private ParticleSystem particles;

    private void Start()
    {
        GameManager.SpawnPoints.Add(this);
    }

    public bool PlayerNear()
    {
        float distance = Vector3.Distance(Player.Instance.transform.position, transform.position);

        return distance < 10f;
    }

    public void PlayParticles()
    {
        particles.Play();
    }
}
