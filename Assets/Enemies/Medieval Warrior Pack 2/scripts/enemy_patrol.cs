using UnityEngine;

public class EnemyAI : Enemy
{
    public GameObject pointA;
    public GameObject pointB;
    public GameObject AttackPoint;
    public float speed;
    public float attackRange = 1.0f;

    private Transform currentPoint;
    public GameObject playerObject;
    public float attackCooldown = 0.5f;
    private float lastAttackTime = 0;

    public override void Start()
    {
        base.Start();
        currentPoint = pointB.transform;
        anim.SetBool("isWalking", true);
        playerObject = GameObject.FindWithTag("Player");
        Awake();
    }

    protected override void Update()
    {


        if (health <= 0)
        {
            anim.SetBool("isWalking", false);
            rb.linearVelocity = Vector2.zero;
            Die();
            return;
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
        //Jos pelaaja on attackRangen sisällä, hyökkää, muuten kävelee 
        if (Vector2.Distance(AttackPoint.transform.position, player.transform.position) < attackRange)
        {
            anim.SetBool("isWalking", false);
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                BasicAttack();
            }
        }
        else
        {
            Patrol();
        }

    }



    void Patrol()
    {
        anim.SetBool("isWalking", true);
        Vector2 point = currentPoint.position - transform.position;
        if (currentPoint == pointA.transform)
        {
            rb.linearVelocity = new Vector2(speed, 0);
        }
        else
        {
            rb.linearVelocity = new Vector2(-speed, 0);
        }

        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointB.transform)
        {
            flip();
            currentPoint = pointA.transform;
        }

        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointA.transform)
        {
            flip();
            currentPoint = pointB.transform;
        }
    }


    private void BasicAttack()
    {

        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("Attack");

        anim.SetTrigger("afterAttack");
        Attack();
    }

    private void flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(pointA.transform.position, 0.3f);
        Gizmos.DrawWireSphere(pointB.transform.position, 0.3f);
        Gizmos.DrawLine(pointA.transform.position, pointB.transform.position);

        Gizmos.DrawWireSphere(AttackPoint.transform.position, attackRange);
    }
}