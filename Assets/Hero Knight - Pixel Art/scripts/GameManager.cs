using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Vector2 PlatformingRespawnPoint;
    public Vector2 respawnPoint;
    [SerializeField] SavePoint save;
    public static GameManager Instance {get; private set; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    public void RespawnPlayer()
    {
        save = FindAnyObjectByType<SavePoint>();
        if(save != null)
        {
            if(save.InterActed)
            {
                respawnPoint = save.transform.position;
            }
            else
            {
                respawnPoint = PlatformingRespawnPoint;
            }
        }
        else
            {
                respawnPoint = PlatformingRespawnPoint;
            }
        PlayerController.Instance.transform.position = respawnPoint;
        StartCoroutine(UIManager.Instance.DEactivateDeathScreen());
        PlayerController.Instance.Respawned();
    }
}