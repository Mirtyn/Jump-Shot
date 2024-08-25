using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bar : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;

    public void Activate()
    {
        particle.gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        particle.gameObject.SetActive(false);
    }
}
