using System;
using NUnit.Framework;
using UnityEditor.Callbacks;
using UnityEngine;
using Random = UnityEngine.Random;
public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float damage;
    [SerializeField] protected PlayerController player;
    [SerializeField] protected float recoilLenght;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;

    [SerializeField] public GameObject StrenghtPotion;
    [SerializeField] public GameObject HealthPotion;
    [SerializeField] public GameObject Apple;

    private bool isDead = false;

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

        if (health <= 0 && !isDead)
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
        if (Other.CompareTag("Player") && !PlayerController.Instance.pState.invincible && !isDead)
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
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("noHp");

        Invoke("SpawnItem", 2f);
        Invoke("DestroyComponent", 2f);
        Debug.Log("vihu kuoli");
    }

    private void SpawnItem()
    {   
        int itemSpawn = Random.Range(0, 10);

        if (itemSpawn == 1 || itemSpawn == 2)
        {
            Instantiate(HealthPotion, transform.position, Quaternion.identity);
        }
        else if (itemSpawn == 3 || itemSpawn == 4)
        {
            Instantiate(StrenghtPotion, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(Apple, transform.position, Quaternion.identity);
        }
    }

    protected void DestroyComponent()
    {
        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
        Destroy(gameObject);
    }
}
