using System.Collections.Generic;
using UnityEngine;

namespace Map
{
    [CreateAssetMenu(menuName = "MapConfig")]
    public class MapConfig : ScriptableObject
    {
        public List<NodeTemplate> NodeTemplates;
        
        public List<NodeType> randomNodes = new List<NodeType>
            {NodeType.Battle, NodeType.Shop, NodeType.Event};
        
        [Header("Node Position Setting")]
        public int xDist = 30;
        public int yGap = 25;
        public int placementRandomness = 5;
        
        [Header("Map Setting")]
        public int floors = 16;
        public int mapWidth = 7;
        public int paths = 6;

        
        [Header("Weights")]
        public float BattleNodeWeight = 10.0f;
        public float ShopNodeWeight = 2.5f;
        public float EventNodeWeight = 4.0f;

    }
}