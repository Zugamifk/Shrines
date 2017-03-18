using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TilePaintingPalette : ScriptableObject {


    [System.Serializable]
    public class GraphicLookup
    {
        public string name;
        public TileGraphicData tile;
    }

    [SerializeField]
    List<GraphicLookup> m_tiles;

    Dictionary<string, TileGraphicData> m_lookup = new Dictionary<string, TileGraphicData>();

    void OnEnable()
    {
        foreach (var t in m_tiles)
        {
            if (!string.IsNullOrEmpty(t.name))
                m_lookup[t.name] = t.tile;
        }
    }

    public TileGraphicData this[string name]
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
