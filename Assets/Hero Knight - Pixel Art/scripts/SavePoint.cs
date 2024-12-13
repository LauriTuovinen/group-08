using Unity.VisualScripting;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public bool InterActed;
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && Input.GetButtonDown("Interact"))
        {
            InterActed = true;
        }
    }
}
