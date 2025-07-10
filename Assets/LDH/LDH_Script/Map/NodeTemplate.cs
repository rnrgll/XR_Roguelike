using UnityEngine;

namespace Map
{
    [CreateAssetMenu(menuName = "Map/Node")]
    public class NodeTemplate : ScriptableObject
    {
        public Sprite sprite;
        public NodeType nodeType;
    }
}