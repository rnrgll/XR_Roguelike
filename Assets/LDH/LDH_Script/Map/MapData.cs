using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    public class MapData
    {
        public readonly List<List<Node>> Map;
        public readonly List<Node> Nodes;
        public readonly List<Vector2Int> Path; //플레이어가 선택한 경로
        public readonly Node BossNode;
        public readonly Node StartNode;

        public MapData(List<List<Node>> map, Node startNode, Node bossNode)
        {
            this.Map = map;
            this.StartNode = startNode;
            this.BossNode = bossNode;

            Nodes = map.SelectMany(floor => floor).Where(node => node.nodeType!=NodeType.NotAssgined).ToList();

            Path = new();
        }

        public List<Node> GetNodeListInFloor(int floor)
        {
            if (floor < 0 || floor >= Map.Count) return null;
            return Map[floor];
        }

        public Node GetNodeByPoint(int row, int column)
        {
            if (row < 0 || column < 0 || row >= Map.Count || column >= Map[0].Count) return null;
            
            return Map[row][column];
        }
        
    }
}