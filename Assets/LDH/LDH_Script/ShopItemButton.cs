using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemButton : MonoBehaviour
{
    //어떤 버튼인지
    public Button itemButton;
    public TMP_Text itemNameText;
    public TMP_Text itemDescText;

    private ShopTest data;
    
    // Start is called before the first frame update
    void Start()
    {
        data = GetComponent<ShopTest>();
    }

    public void ShowData()
    {
        itemNameText.text = data.itemName;
        itemDescText.text = data.itemDescription;
    }

    public void ClearData()
    {
        itemNameText.text = "";
        itemDescText.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
