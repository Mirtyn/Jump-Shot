using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseEnemy : ProjectBehaviour
{
    protected float health;
    protected float attackDistance;
    protected Transform thisTransform;
    protected Rigidbody2D rb;
    protected bool yMovement;
    protected float moveSpeed;
    protected float attackCooldown;
    protected float lastAttackTime;
    protected float randomCooldown;
    protected float lastRandomTime;
    protected float leapOfFaithDistance;
    [SerializeField] protected ParticleSystem deathParticles;
    [SerializeField] protected AudioSource deathAudio;
    [SerializeField] protected AudioSource hitAudio;
    protected bool died;

    protected virtual void OnAwake()
    {
        thisTransform = transform;
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual bool CanAttack()
    {
        if (lastAttackTime + attackCooldown < Time.time)
        {
            lastAttackTime = Time.time;

            return true;
        }

        return false;
    }

    protected virtual bool CanRandomMove()
    {
        if (lastRandomTime + randomCooldown < Time.time)
        {
            lastRandomTime = Time.time;

            return true;
        }

        return false;
    }

    protected virtual void OnUpdate()
    {
        if (died) return;
        Vector3 playerPos = Player.Instance.transform.position;
        float distance = Vector3.Distance(playerPos, thisTransform.position);

        if (thisTransform.position.y < -70)
        {
            Die(true);
        }

        if (distance < attackDistance)
        {
            this.CanAttack();
        }
        else
        {
            this.TryMove(playerPos);
        }
    }

    protected virtual void TryMove(Vector3 playerPos)
    {
        Vector2 moveVector = new Vector2();

        if (playerPos.x < thisTransform.position.x)
        {
            // move left
            moveVector.x -= 1;
        }
        else
        {
            // move right
            moveVector.x += 1;
        }

        if (yMovement)
        {
            if (playerPos.y < thisTransform.position.y)
            {
                // move down
                moveVector.y -= 1;
            }
            else
            {
                // move up
                moveVector.y += 1;
            }
        }

        moveVector = moveVector.normalized;

        float distance = moveSpeed * Time.deltaTime;

        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position, transform.localScale, 0f, moveVector, distance);

        bool hitSomething = false;

        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];

            if (hit.transform != thisTransform && hit.transform != null && !hit.transform.CompareTag("Pickup"))
            {
                // hit
                hitSomething = true;
                break;
            }
        }

        if (hitSomething && yMovement)
        {
            bool hitOnX = false;
            bool hitOnY = false;
            Vector2 moveVectorX = new Vector2(moveVector.x, 0f).normalized;

            RaycastHit2D[] hitsX = Physics2D.BoxCastAll(transform.position, transform.localScale, 0f, moveVectorX, distance);

            for (int i = 0; i < hitsX.Length; i++)
            {
                RaycastHit2D hitX = hitsX[i];

                if (hitX.transform != thisTransform && hitX.transform != null && !hitX.transform.CompareTag("Pickup"))
                {
                    hitOnX = true;
                    break;
                }
            }

            Vector2 moveVectorY = new Vector2(0f, moveVector.y).normalized;

            RaycastHit2D[] hitsY = Physics2D.BoxCastAll(transform.position, transform.localScale, 0f, moveVectorY, distance);

            for (int i = 0; i < hitsY.Length; i++)
            {
                RaycastHit2D hitY = hitsY[i];

                if (hitY.transform != thisTransform && hitY.transform != null && !hitY.transform.CompareTag("Pickup"))
                {
                    hitOnY = true;
                    break;
                }
            }

            if (hitOnX && hitOnY)
            {
                return;
            }

            if (!hitOnX)
            {
                moveVector = moveVectorX;
            }
            else if(!hitOnY)
            {
                moveVector = moveVectorY;
            }
        }
        else if (hitSomething)
        {
            return;
        }

        bool floorSomewhereBelow = false;
        bool floorDirectleyBelow = false;

        if (!yMovement)
        {
            Vector3 moveVector3 = moveVector * distance;
            Vector2 checkFloor = -thisTransform.up;
            RaycastHit2D[] floorHits = Physics2D.BoxCastAll(transform.position + moveVector3, transform.localScale, 0f, checkFloor);

            for (int i = 0; i < floorHits.Length; i++)
            {
                RaycastHit2D floorHit = floorHits[i];

                if (floorHit.transform != thisTransform && floorHit.transform != null && !floorHit.transform.CompareTag("Bullet") && !floorHit.transform.CompareTag("Player") && !floorHit.transform.CompareTag("EnemyBullet") && !floorHit.transform.CompareTag("Enemy") && !floorHit.transform.CompareTag("Pickup"))
                {
                    // there is floor somewhere below
                    floorSomewhereBelow = true; 
                    break;
                }
            }

            if (floorSomewhereBelow)
            {
                Vector2 checkFloorDirect = -thisTransform.up;
                RaycastHit2D[] floorHitsDirect = Physics2D.BoxCastAll(transform.position + moveVector3, transform.localScale, 0f, checkFloorDirect, transform.localScale.y / 2 * 1.1f);

                for (int i = 0; i < floorHitsDirect.Length; i++)
                {
                    RaycastHit2D floorHitDirect = floorHitsDirect[i];

                    if (floorHitDirect.transform != thisTransform && floorHitDirect.transform != null && !floorHitDirect.transform.CompareTag("Bullet") && !floorHitDirect.transform.CompareTag("Player") && !floorHitDirect.transform.CompareTag("EnemyBullet") && !floorHitDirect.transform.CompareTag("Enemy") && !floorHitDirect.transform.CompareTag("Pickup"))
                    {
                        // there is floor somewhere below
                        floorDirectleyBelow = true;
                        break;
                    }
                }
            }
        }
        else
        {
            this.Move(moveVector, distance);
        }

        if (!floorDirectleyBelow)
        {
            if (!floorSomewhereBelow) return;

            // leap of faith

            float dist = Vector3.Distance(playerPos, thisTransform.position);

            if (dist < leapOfFaithDistance)
            {
                this.LeapOfFaith(moveVector, distance);
            }
        }
        else
        {
            this.Move(moveVector, distance);
        }
    }

    protected virtual void Move(Vector3 moveVector, float distance)
    {
        thisTransform.position += moveVector * distance;
    }

    protected virtual void LeapOfFaith(Vector3 moveVector, float distance)
    {
        thisTransform.position += moveVector * distance;
    }

    public virtual void Damage(float damage, Vector2 hitForce)
    {
        if (died) return;
        health -= damage;
        rb.AddForce(hitForce / 2.5f, ForceMode2D.Impulse);

        if (health <= 0)
        {
            Die(false);
        }
        else
        {
            int rnd = Random.Range(0, GameManager.HitClips.Count);
            hitAudio.clip = GameManager.HitClips[rnd];
            hitAudio.Play();
        }
    }

    public virtual void Die(bool byFall)
    {
        died = true;
        deathParticles.Play();
        if (!byFall)
        {
            deathAudio.Play();
        }

        Destroy(gameObject, 1f);

        this.thisTransform.GetChild(0).gameObject.SetActive(false);
        
        PlaceAmmo();
    }

    public virtual void PlaceAmmo()
    {
        Instantiate(GameManager.PickupPrefab, thisTransform.position, Quaternion.identity);
        Instantiate(GameManager.PickupPrefab, thisTransform.position, Quaternion.identity);
        Instantiate(GameManager.PickupPrefab, thisTransform.position, Quaternion.identity);
    }
}
