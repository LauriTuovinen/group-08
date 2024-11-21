using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HeartsController : MonoBehaviour
{
    PlayerController player;

    private GameObject[] heartsContainer;
    private Image[] heartsFill;
    public Transform heartsParent;
    public GameObject HeartsContainerPrefab;
    void Start()
    {
        player = PlayerController.Instance;
        heartsContainer = new GameObject[PlayerController.Instance.maxHealth];
        heartsFill = new Image[PlayerController.Instance.maxHealth];

        PlayerController.Instance.onHealthChangedCallback += UpdateHeartHUD;
        InstantiateHeartContainer();
        UpdateHeartHUD();
    }

    void Update()
    {
        
    }

    void SetHeartContainers()
    {
        for(int i = 0; i < heartsContainer.Length; i++)
        {
            if(i < PlayerController.Instance.maxHealth)
            {
                heartsContainer[i].SetActive(true);
            }
            else
            {
                heartsContainer[i].SetActive(false);
            }
        }
    }

    void SetFilledHearts()
    {
        for(int i = 0; i < heartsFill.Length; i++)
        {
            if(i < PlayerController.Instance.Health)
            {
                heartsFill[i].fillAmount = 1;
            }
            else
            {
                heartsFill[i].fillAmount = 0;
            }
        }
    }

    void InstantiateHeartContainer()
    {
        for(int i = 0; i < PlayerController.Instance.maxHealth; i++)
        {
            GameObject temp = Instantiate(HeartsContainerPrefab);
            temp.transform.SetParent(heartsParent, false);
            heartsContainer[i] = temp;
            heartsFill[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
        }
    }

    void UpdateHeartHUD()
    {
        SetHeartContainers();
        SetFilledHearts();
    }

}
