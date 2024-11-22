using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private UIInventory inventoryUI;
    public int inventorysize = 6;
    
    private void Start()
    {
        inventoryUI.InitializeInventoryUI(inventorysize);
    }
        public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryUI.isActiveAndEnabled == false)
            {
                inventoryUI.Show();
            }
            else{
                inventoryUI.Hide();
            }
        }
    }
}
