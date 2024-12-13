using System.Collections;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public SceneFader sceneFader;
    [SerializeField] GameObject deathScreen;

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

    private void Start()
    {
        sceneFader = GetComponentInChildren<SceneFader>();
    }

    public IEnumerator ActiveDeathScreen()
    {
        yield return new WaitForSeconds(0.8f);
        if (sceneFader == null)
    {
        Debug.LogError("sceneFader is null!");
        yield break;
    }
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.In));

        yield return new WaitForSeconds(0.8f);
        if (deathScreen == null)
    {
        Debug.LogError("deathScreen is null!");
        yield break;
    }
        deathScreen.SetActive(true);
    }

    public IEnumerator DEactivateDeathScreen()
    {
        yield return new WaitForSeconds(0.5f);
        deathScreen.SetActive(false);
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.Out));
    }
}
