using UnityEditor.Callbacks;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float damage;
    [SerializeField] protected PlayerController player;
    [SerializeField] float recoilLenght;
    [SerializeField] float recoilFactor;
    [SerializeField] bool isRecoiling = false;
    float recoilTimer;
    protected Rigidbody2D rb;

    public virtual void Start()
    {
        
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
            Destroy(gameObject);
        }
        if (isRecoiling)
        {
            if(recoilTimer < recoilLenght)
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
        if(!isRecoiling)
        {
            rb.AddForce(-hitforce*recoilFactor * hitDirection);
            isRecoiling = true;
        }
    }

    protected void OnTriggerStay2D(Collider2D Other)
    {
        if(Other.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
        {
            Attack();
        }
    }
    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }

}
