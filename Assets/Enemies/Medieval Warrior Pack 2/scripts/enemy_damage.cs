using UnityEngine;

public class enemy_damage : MonoBehaviour
{
    public Animator anim;
    public int maxHealth = 10;
    int currentHealth;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        anim.SetTrigger("takeDamage");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        anim.SetTrigger("noHp");

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;

        Invoke("DestroyComponent", 0.5f);
    }


    void DestroyComponent()
    {
        Destroy(gameObject);
    }
}
