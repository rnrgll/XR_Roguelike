using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    private List<MinorArcana> selectedCards = new List<MinorArcana>();

    public event Action<List<MinorArcana>> OnMinorArcanaAttack;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ShootCard(MinorArcana card)
    {
        if (selectedCards.Count >= 5) return;

        selectedCards.Add(card);

        if (selectedCards.Count == 5)
        {
            OnMinorArcanaAttack?.Invoke(new List<MinorArcana>(selectedCards));
            selectedCards.Clear();
        }
    }

    public void Reset() => selectedCards.Clear();
}