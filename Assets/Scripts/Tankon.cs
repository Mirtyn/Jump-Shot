using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tankon : BaseEnemy
{
    [SerializeField] private AudioSource shot;
    private void Awake()
    {
        this.OnAwake();
    }

    protected override void OnAwake()
    {
        health = 150f;
        attackDistance = 14f;
        yMovement = false;
        moveSpeed = 3.5f;
        leapOfFaithDistance = 24f;
        attackCooldown = 1.5f;
        lastAttackTime = Time.time + attackCooldown;
        randomCooldown = 3f;
        lastRandomTime = Time.time + randomCooldown;
        base.OnAwake();
    }

    private void Update()
    {
        base.OnUpdate();
        CanRandomMove();
    }

    protected override bool CanAttack()
    {
        bool canAttack = base.CanAttack();

        if (canAttack)
        {
            Att();
            Invoke(nameof(Att), 0.5f);
            Invoke(nameof(Att), 1f);
        }

        return false;
    }
    private void Att()
    {
        if (died) return;

        Vector3 playerDir = (Player.Instance.transform.position - thisTransform.position).normalized;
        Vector2 pos = thisTransform.position + playerDir * 4f;
        shot.Play();
        var obj = Instantiate(GameManager.EnemyBulletPrefab, pos, Quaternion.identity);
        obj.GetComponent<EnemyBullet>().SetStats(10f, 2, new Vector3(0.8f, 0.5f, 1f), playerDir);
    }

    public override void PlaceAmmo()
    {
        base.PlaceAmmo();
        base.PlaceAmmo();
        base.PlaceAmmo();
        base.PlaceAmmo();
    }

    protected override bool CanRandomMove()
    {
        bool canAttack = base.CanRandomMove();

        if (canAttack)
        {
            Vector3 playerDir = (Player.Instance.transform.position - thisTransform.position).normalized;
            rb.AddForce(-playerDir * 50f, ForceMode2D.Impulse);
            //Vector2 pos = thisTransform.position + playerDir * 1.3f;
            //gunShot.Play();
            //var obj = Instantiate(GameManager.EnemyBulletPrefab, pos, Quaternion.identity);
            //obj.GetComponent<EnemyBullet>().SetStats(10f, 2, new Vector3(0.5f, 0.3f, 1f), playerDir);
        }

        return false;
    }
}
