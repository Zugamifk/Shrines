using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldManager : MonoBehaviour
{

    public TileData[] tiles;

    public static WorldManager Instance;

    World world;
    Dictionary<string, TileData> tileLookup = new Dictionary<string, TileData>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        foreach (var data in tiles)
        {
            tileLookup.Add(data.name, data);
        }


    }

    public TileData GetTileData(string name)
    {
        TileData result = null;
        tileLookup.TryGetValue(name, out result);
        return result;
    }

    public void GenerateWorld()
    {
        world = new World("Aebeltoft");
        world.Generate();
    }
}
