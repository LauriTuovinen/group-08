using UnityEngine;

public class Attack : Enemy
{

    public GameObject AttackPoint;
    public float attackRange = 1.0f;
    public float attackCooldown = 0.5f;
    private float lastAttackTime = 0;
    public override void Start()
    {
        anim = GetComponent<Animator>();
        Awake();
    }

    protected override void Update()
    {

        if (health <= 0)
        {
            Die();
        }
        if (isRecoiling)
        {
            if (recoilTimer < recoilLenght)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
        //Jos pelaaja on attackRangen sisällä, hyökkää
        if (Vector2.Distance(AttackPoint.transform.position, player.transform.position) < attackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                PerformAttack();
            }
        }
    }
    private void PerformAttack()
    {
        anim.SetTrigger("attackRoll");
        Attack();
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(AttackPoint.transform.position, attackRange);
    }
}

