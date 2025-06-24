using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    public class MapData
    {
        public readonly List<List<Node>> map;
        public readonly List<Node> Nodes;
        public readonly List<Vector2Int> path; //플레이어가 선택한 경로
        public readonly Node bossNode;
        public readonly Node startNode;

        public MapData(List<List<Node>> map, Node startNode, Node bossNode)
        {
            this.map = map;
            this.startNode = startNode;
            this.bossNode = bossNode;

            Nodes = map.SelectMany(floor => floor).Where(node => node.HasConnections()).ToList();
        }

        public List<Node> GetNodeListInFloor(int floor)
        {
            if (floor < 0 || floor >= map.Count) return null;
            return map[floor];
        }

        public Node GetNodeByPoint(int row, int column)
        {
            if (row < 0 || column < 0 || row >= map.Count || column >= map[0].Count) return null;
            
            return map[row][column];
        }
        
        public float GetMapLength()
        {
            if (bossNode == null || startNode == null)
                return 0f;

            return bossNode.position.y - startNode.position.y;
        }

    }
}