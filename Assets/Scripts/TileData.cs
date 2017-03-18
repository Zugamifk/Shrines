using UnityEngine;
using System.Collections;

public class TileData : ScriptableObject {

    public enum CollisionType
    {
        None,
        Solid,
        Platform
    }

    public CollisionType collisions;

}
