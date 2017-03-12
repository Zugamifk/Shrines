using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//todo: 'gradient' structure for doing graded textures, like for depth of ground
public class TileGraphicData : ScriptableObject
{

    public enum Edge
    {
        None, TopRight, Right, BottomRight, Bottom, BottmLeft,Left, TopLeft, Top, All
    }
    [System.Serializable]
    public class SpriteShape
    {
        [Tooltip("the AABB for this shape")]
        public Vector2i dimensions = Vector2i.one;
        [Tooltip("depth from surface")]
        public int depth;
        [Tooltip("what increments in the grid this tile can appear on")]
        public Vector2i step;
        public Sprite[] sprites;
        public Edge edge;
    }

    public List<SpriteShape> shapes;
}
