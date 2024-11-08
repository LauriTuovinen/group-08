using UnityEngine;

public class Attack : MonoBehaviour
{

    public GameObject AttackPoint;
    public float attackRange = 1.0f;
    public float attackCooldown = 0.5f;
    private float lastAttackTime = 0;
    public GameObject player;
    private Animator anim;
    private Rigidbody2D rb;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
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
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(AttackPoint.transform.position, attackRange);
    }
}

