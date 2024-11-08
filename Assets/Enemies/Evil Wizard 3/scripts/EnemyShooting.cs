using UnityEngine;

public class EnemyShooting : MonoBehaviour
{
    public GameObject projectile;
    private float timer;
    public Transform projectilePos;
    public float attackCooldown = 2.0f;
    private GameObject player;
    public float attackRange = 10.0f;
    private Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance < attackRange)
        {
            timer += Time.deltaTime;
            if (timer > attackCooldown)
            {
                anim.SetTrigger("attack");
                timer = 0;
                Invoke("shoot", .85f);
            }
        }
    }

    void shoot()
    {
        Instantiate(projectile, projectilePos.position, Quaternion.identity);
    }

        private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
