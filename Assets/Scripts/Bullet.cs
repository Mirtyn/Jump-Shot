using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 30f;
    private float damage = 30f;
    private Transform thisTransform;
    [SerializeField] private Transform visual;
    [SerializeField] private ParticleSystem normalParticle;
    [SerializeField] private ParticleSystem specialParticle;

    private void Awake()
    {
        thisTransform = transform;
    }

    public void SetStats(float speed, float damage, bool special, Vector3 size)
    {
        this.speed = speed;
        this.damage = damage;
        visual.localScale = size;
        if (special)
        {
            specialParticle.Play();
        }
        else
        {
            normalParticle.Play();
        }
    }

    private void FixedUpdate()
    {
        Vector3 pos = thisTransform.position;
        pos += thisTransform.right * speed * Time.fixedDeltaTime;
        var hit = Physics2D.Raycast(thisTransform.position, thisTransform.right, speed * Time.fixedDeltaTime);

        if (hit.collider == null || hit.transform == thisTransform)
        {
            thisTransform.position = pos;
        }
        else
        {
            pos = hit.point;
            thisTransform.position = pos;
            Hit(hit);
        }
    }

    private void Hit(RaycastHit2D hit)
    {
        normalParticle.Stop();
        specialParticle.Stop();
        Destroy(gameObject, 1f);
    }
}
