using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FontApply : MonoBehaviour
{
    [SerializeField] public TMP_FontAsset fontAsset;
}

#if UNITY_EDITOR
[CustomEditor(typeof(FontApply))]
public class TMP_FontApplyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Font Apply"))
        {
            TMP_FontAsset fontAsset = ((FontApply)target).fontAsset;

            foreach(TextMeshPro textMeshPro3D in GameObject.FindObjectsOfType<TextMeshPro>(true))
            {
                textMeshPro3D.font = fontAsset;
            }
            foreach(TextMeshProUGUI textMeshProUi in GameObject.FindObjectsOfType<TextMeshProUGUI>(true))
            {
                textMeshProUi.font = fontAsset;
            }
        }
    }
}
#endif