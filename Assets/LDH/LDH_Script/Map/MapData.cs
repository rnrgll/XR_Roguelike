using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    public class MapData
    {
        public readonly List<List<Node>> map;
        public readonly List<Node> Nodes;
        public readonly List<Vector2Int> path;
        public readonly Node bossNode;
        public readonly Node startNode;

        public MapData(List<List<Node>> map, Node startNode, Node bossNode)
        {
            this.map = map;
            this.startNode = startNode;
            this.bossNode = bossNode;

            Nodes = map.SelectMany(floor => floor).Where(node => node.HasConnections()).ToList();
        }

        public float GetMapLength()
        {
            if (bossNode == null || startNode == null)
                return 0f;

            return bossNode.position.y - startNode.position.y;
        }

    }
}