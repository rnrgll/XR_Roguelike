using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{


    
    public class Node
    {
        public NodeType nodeType;
        public Vector2Int point;
        public int row, column;
        public List<Node> nextNodes;
        public bool selected;
        

        public Node(int row, int column)
        {
            //this.type = type;
            this.row = row;
            this.column = column;
            point = new Vector2Int(row, column);
            nextNodes = new();
            selected = false;
        }

        public bool HasConnections()
        {
            return nextNodes.Count != 0;
        }


        //debug----------
        public override string ToString()
        {
            return $"{column} ({nodeType.ToString()[0]})";
        }
    }
}