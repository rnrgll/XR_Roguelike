using System.Collections.Generic;
using UnityEngine;
using CardEnum;

[CreateAssetMenu(menuName = "Database/DebuffDatabase")]
public class DebuffDatabase : ScriptableObject
{
    public static DebuffDatabase Instance { get; private set; }

    [System.Serializable]
    public class Entry
    {
        public CardDebuff type;
        public CardDebuffSO debuffSO;
    }

    [SerializeField] private List<Entry> entries;
    private Dictionary<CardDebuff, CardDebuffSO> lookup = new();

    private void OnEnable()
    {
        Instance = this;
        lookup.Clear();
        foreach (var entry in entries)
        {
            if (!lookup.ContainsKey(entry.type))
                lookup[entry.type] = entry.debuffSO;
        }
    }

    public CardDebuffSO GetDebuffSO(CardDebuff type)
    {
        lookup.TryGetValue(type, out var so);
        return so;
    }
}