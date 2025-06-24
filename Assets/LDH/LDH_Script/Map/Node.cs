using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Map
{


    
    public class Node
    {
        public NodeType nodeType;
        public int row, column;
        public Vector2 position;
        public List<Node> nextNodes;
        public bool selected;
        

        public Node(int row, int column, Vector2 position)
        {
            //this.type = type;
            this.row = row;
            this.column = column;
            this.position = position;
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