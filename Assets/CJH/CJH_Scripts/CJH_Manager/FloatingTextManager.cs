using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingTextManager : MonoBehaviour
{
    public static FloatingTextManager Instance;

    public GameObject damageTextPrefab; // TextMeshPro UI 프리팹

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Spawn(int damage, Vector3 worldPosition)
    {
        var obj = Instantiate(damageTextPrefab, transform);
        obj.transform.position = worldPosition;

        var tmp = obj.GetComponent<TextMeshProUGUI>();
        tmp.text = $"-{damage}";
        tmp.color = Color.red;

        Destroy(obj, 1f);
    }
}