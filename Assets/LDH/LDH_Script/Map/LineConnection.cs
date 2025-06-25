using UnityEngine;
using UnityEngine.UI.Extensions;
using Gradient = UnityEngine.Gradient;

namespace Map
{
    public class LineConnection
    {
        public UILineRenderer  lr;
        public MapNode from;
        public MapNode to;
        
        public LineConnection(UILineRenderer lr, MapNode from, MapNode to)
        {
            this.lr = lr;
            this.from = from;
            this.to = to;
        }
        
        public void SetColor(Color color)
        {
            lr.color = color;
        }
    }
}