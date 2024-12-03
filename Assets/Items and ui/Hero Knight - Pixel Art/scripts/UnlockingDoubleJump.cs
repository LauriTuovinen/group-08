using System.Collections;
using UnityEngine;

public class UnlockingDoubleJump : MonoBehaviour

{
    [SerializeField] GameObject particles;
    [SerializeField] GameObject canvasUI;

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
            StartCoroutine(ShowUI());
        }
    }
    IEnumerator ShowUI()
    {
        GameObject _particles = Instantiate(particles, transform.position, Quaternion.identity);
        Destroy(_particles, 0.5f);
        yield return new WaitForSeconds(0.5f);

        canvasUI.SetActive(true);

        yield return new WaitForSeconds(4f);
        PlayerController.Instance.unlockedDoubleJump = true;
        canvasUI.SetActive(false);
        Destroy(gameObject);
    }
}