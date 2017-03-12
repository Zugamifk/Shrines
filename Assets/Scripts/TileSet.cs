using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileSet : ScriptableObject {

    [System.Serializable]
    public class Tile
    {
        public string name;
        public TileData tile;
    }
    public List<Tile> tiles;
}
