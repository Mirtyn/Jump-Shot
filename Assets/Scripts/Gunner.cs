using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunner : BaseEnemy
{
    [SerializeField] private AudioSource shot;
    private void Awake()
    {
        this.OnAwake();
    }

    protected override void OnAwake()
    {
        health = 30f;
        attackDistance = 12f;
        yMovement = false;
        moveSpeed = 3f;
        leapOfFaithDistance = 16f;
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

        if (canAttack)
        {
            Vector3 playerDir = (Player.Instance.transform.position - thisTransform.position).normalized;
            Vector2 pos = thisTransform.position + playerDir * 1.3f;
            //gunShot.Play();
            shot.Play();
            var obj = Instantiate(GameManager.EnemyBulletPrefab, pos, Quaternion.identity);
            obj.GetComponent<EnemyBullet>().SetStats(10f, 2, new Vector3(0.5f, 0.3f, 1f), playerDir);
        }

        return false;
    }
}
