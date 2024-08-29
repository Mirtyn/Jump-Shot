using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    private float speed = 10f;
    private float damage = 5f;
    private Transform thisTransform;
    private bool hit = false;
    [SerializeField] private Transform visual;
    [SerializeField] private ParticleSystem normalParticle;

    private void Awake()
    {
        thisTransform = transform;
    }

    public void SetStats(float speed, float damage, Vector3 size, Vector3 dir)
    {
        this.speed = speed;
        this.damage = damage;
        visual.localScale = size;
        thisTransform.right = dir;
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

        Destroy(gameObject, 1f);
    }
}
