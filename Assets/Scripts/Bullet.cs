using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float speed = 30f;
    private float damage = 30f;
    private Transform thisTransform;
    private bool hit = false;
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

    private void Update()
    {
        if (this.hit) return;
        Vector3 pos = thisTransform.position;
        pos += thisTransform.right * speed * Time.deltaTime;
        var hits = Physics2D.RaycastAll(thisTransform.position, thisTransform.right, speed * Time.deltaTime);

        RaycastHit hit = new RaycastHit();

        for (int j = 0; j < hits.Length; j++)
        {
            if (hits[j].transform != thisTransform && hits[j].transform != null && !hits[j].transform.CompareTag("Pickup"))
            {
                pos = hits[j].point;
                thisTransform.position = pos;
                Hit(hits[j]);
                return;
            }
        }


        thisTransform.position = pos;

        //if (hits.collider == null || hits.transform == thisTransform)
        //{
        //    thisTransform.position = pos;
        //}
        //else
        //{
        //    pos = hits.point;
        //    thisTransform.position = pos;
        //    Hit(hits);
        //}
    }

    private void Hit(RaycastHit2D hit)
    {
        this.hit = true;

        if (hit.transform.CompareTag("Enemy"))
        {
            hit.transform.GetComponent<BaseEnemy>().Damage(damage, thisTransform.right * speed);
        }
        GetComponent<BoxCollider2D>().enabled = false;

        normalParticle.Stop();
        specialParticle.Stop();
        Destroy(gameObject, 1f);
    }
}
