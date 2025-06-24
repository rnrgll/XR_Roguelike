using UnityEngine;

namespace Map
{
    public class MapLayer : MonoBehaviour
    {
        public NodeType nodeType;
        public float minDistanceFromPrevisousLayer;
        public float maxDistanceFromPreviousLayer;
        public float nodesApartDistance;
        [Range(0f, 1f)] public float randomizePosition;
        [Range(0f, 1f)] public float randomizesNodes;
    }
}