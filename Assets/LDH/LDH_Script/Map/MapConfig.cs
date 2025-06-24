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
        public int yDist = 25;
        public int placementRandomness = 5;
        
        [Header("Map Setting")]
        public int floors = 16;
        public int mapWidth = 7;
        public int paths = 6;

        
        [Header("Weights")]
        public float BattleRoomWeight = 10.0f;
        public float ShopRoomWeight = 2.5f;
        public float EventRoomWeight = 4.0f;

    }
}