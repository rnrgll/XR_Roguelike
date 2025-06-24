using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    [CreateAssetMenu(menuName = "MapConfig")]
    public class MapConfig : ScriptableObject
    {
        public List<NodeTemplate> NodeTemplates;
    }
}