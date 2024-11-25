using UnityEngine;

public class UnlockingDoubleJump : MonoBehaviour

{
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
            PlayerController.Instance.unlockedDoubleJump = true;

            Destroy(gameObject);
        }
    }
}
