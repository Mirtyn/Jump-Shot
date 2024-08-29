using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : BaseEnemy
{
    private void Awake()
    {
        this.OnAwake();
    }

    protected override void OnAwake()
    {
        health = 55f;
        attackDistance = 0.2f;
        yMovement = false;
        moveSpeed = 5f;
        leapOfFaithDistance = 20f;
        attackCooldown = 2f;
        lastAttackTime = Time.time + attackCooldown;
        base.OnAwake();
    }

    private void Update()
    {
        base.OnUpdate();
    }

    protected override bool CanAttack()
    {
        bool canAttack = base.CanAttack();

        //if (canAttack)
        //{
        //    Vector3 playerDir = (Player.Instance.transform.position - thisTransform.position).normalized;
        //    Vector2 pos = thisTransform.position + playerDir * 1.3f;
        //    //gunShot.Play();
        //    var obj = Instantiate(GameManager.EnemyBulletPrefab, pos, Quaternion.identity);
        //    obj.GetComponent<EnemyBullet>().SetStats(10f, 2, new Vector3(0.5f, 0.3f, 1f), playerDir);
        //}

        return false;
    }
}
