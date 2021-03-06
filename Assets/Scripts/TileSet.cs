﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileSet : ScriptableObject {

    [System.Serializable]
    public class Tile
    {
        public string name;
        public TileData tile;
    }

    [SerializeField]
    List<Tile> m_tiles;

    Dictionary<string, TileData> m_lookup = new Dictionary<string, TileData>();

    void OnEnable()
    {
        foreach (var t in m_tiles)
        {
            if(!string.IsNullOrEmpty(t.name))
                m_lookup[t.name] = t.tile;
        }
    }

    public TileData this[string name]
    {
        get
        {
            return m_lookup[name];
        }
#if UNITY_EDITOR
        set
        {
            m_lookup[name] = value;
        }
#endif
    }
}
