using UnityEngine;

public class UnlockingDoubleJump : MonoBehaviour

{
    [SerializeField] GameObject particles;
    bool used;
    void Start()
    {
        if(PlayerController.Instance.unlockedDoubleJump)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !used)
        {
            used = true;
            GameObject _particles = Instantiate(particles, transform.position, Quaternion.identity);
            Destroy(_particles, 0.5f);
            PlayerController.Instance.unlockedDoubleJump = true;

            Destroy(gameObject);
        }
    }
}
