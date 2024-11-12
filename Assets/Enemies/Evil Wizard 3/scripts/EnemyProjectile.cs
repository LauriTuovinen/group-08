using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{   
    private GameObject player;
    private Rigidbody2D rb;
    public float force;
    private float timer;
    private Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();

        Vector3 direction = player.transform.position - transform.position;
        rb.linearVelocity = new Vector2(direction.x, direction.y).normalized * force;

        float rot = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rot);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > 5)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Ground"))
        {
            anim.SetTrigger("hit");
            rb.linearVelocity = Vector2.zero;
            Invoke("DestroyComponent", 0.583f);
        }
    }

    void DestroyComponent()
    {
           Destroy(gameObject);
    }
}
