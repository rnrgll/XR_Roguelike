using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemButton : MonoBehaviour
{
    public TMP_Text itemNameText;
    public TMP_Text itemInfoText;
    private Button itemButton;
    private ShopTest data;
    
    // Start is called before the first frame update
    void Start()
    {
        itemButton = GetComponent<Button>();
        
        data = GetComponent<ShopTest>();
    }

    public void ShowData()
    {
        itemNameText.text = data.itemName;
        itemInfoText.text = data.itemDescription;
    }

    public void ClearData()
    {
        itemNameText.text = "";
        itemInfoText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
