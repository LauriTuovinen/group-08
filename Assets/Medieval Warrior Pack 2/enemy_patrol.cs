using UnityEngine;

public class EnemyAI : MonoBehaviour
{    
    public GameObject pointA;
    public GameObject pointB;
    public GameObject AttackPoint;
    public float speed;

    public float attackRange = 1.0f;

    private Transform currentPoint;
    private Animator anim;
    private Rigidbody2D rb;
    public GameObject player;
    void Start()
    {
     rb = GetComponent<Rigidbody2D>();
     anim = GetComponent<Animator>();
     currentPoint = pointB.transform;
     anim.SetBool("isWalking", true);
     player = GameObject.FindWithTag("Player");
    }

    void Update()
    {   
        //Jos pelaaja on attackRangen sisällä, hyökkää, muuten kävelee 
        if (Vector2.Distance(AttackPoint.transform.position, player.transform.position) < attackRange)
        {

            Attack();
        
        }
        else
        {
            anim.SetBool("isWalking", true);
            Patrol();
        }
    }

        void Patrol()
        {
        Vector2 point = currentPoint.position - transform.position;
        if(currentPoint == pointA.transform)
        {
            rb.linearVelocity = new Vector2(speed, 0);
        }
        else
        {
            rb.linearVelocity = new Vector2(-speed, 0);
        }

        if(Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointB.transform)
        {
            flip();
            currentPoint = pointA.transform;
        }

        if(Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == pointA.transform)
        {
            flip();
            currentPoint = pointB.transform;
        }
        }
    
    private void Attack()
    {
    anim.SetBool("isWalking", false);
    anim.SetTrigger("Attack");
    rb.linearVelocity = Vector2.zero;
    print("player in attack range");
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