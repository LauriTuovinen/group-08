using UnityEditor.Callbacks;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float damage;
    [SerializeField] protected PlayerController player;
    [SerializeField] protected float recoilLenght;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;

    protected float recoilTimer;
    protected Rigidbody2D rb;
    public Animator anim;

    public virtual void Start()
    {
        anim = GetComponent<Animator>();
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = PlayerController.Instance;
    }

    protected virtual void Update()
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
    }

    public void EnemyHit(float damageDone, Vector2 hitDirection, float hitforce)
    {
        health -= damageDone;
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("takeDamage");
        if (!isRecoiling)
        {
            rb.AddForce(-hitforce * recoilFactor * hitDirection);
            isRecoiling = true;
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D Other)
    {
            if (Other.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
            {
                Attack();
            }
    }
    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }

    protected void Die()
    {
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("noHp");

        Invoke("DestroyComponent", 2f);
    }
    protected void DestroyComponent()
    {
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        Destroy(gameObject);
    }
}
