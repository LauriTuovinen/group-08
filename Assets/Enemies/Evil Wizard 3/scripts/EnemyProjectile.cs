using UnityEngine;

public class EnemyProjectile : Enemy
{

    [SerializeField] private AudioSource audioSource1;
    [SerializeField] private AudioSource audioSource2;
    public float force;

    private float timer;
    public override void Start()
    {
        Awake();
        audioSource1.Play();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        Vector3 direction = player.transform.position - transform.position;
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;

        float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot);
    }

    protected override void Update()
    {
        timer += Time.deltaTime;

        if (timer > 5)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Attack();
            anim.SetTrigger("hit");
            rb.linearVelocity = Vector2.zero;
            Invoke("DestroyComponent", 0.583f);
        }
        else if (other.gameObject.CompareTag("Ground"))
        {
            anim.SetTrigger("hit");
            audioSource1.Stop();
            audioSource2.Play();
            rb.linearVelocity = Vector2.zero;
            Invoke("DestroyComponent", 0.583f);
        }
    }

}
