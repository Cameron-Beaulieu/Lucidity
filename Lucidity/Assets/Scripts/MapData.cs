using System;
using UnityEngine;

// TODO: Add other necessary fields for saving/loading files
[Serializable]
public class MapData {
    public string MapSize;
    public Biome Biome;

    public MapData(string mapSize, Biome biome) {
        MapSize = mapSize;
        Biome = biome;
    }

    public string Serialize() {
        return JsonUtility.ToJson(this);
    }
}