using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionMenu : MonoBehaviour
{
    [SerializeField] private RectTransform backButton;
    public void OnClickTabs(RectTransform PanelRectTransform)
    {
        PanelRectTransform.SetAsLastSibling();
        backButton.SetAsLastSibling();
    }

    public void OnClickBack()
    {
        gameObject.SetActive(false);
    }
}
