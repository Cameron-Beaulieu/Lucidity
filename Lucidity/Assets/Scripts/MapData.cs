using System;
using UnityEngine;

// TODO: Add other necessary fields for saving/loading files
[Serializable]
public class MapData {
    public CreateNewMap.SizeType MapSize;
    public Biome Biome;

    public MapData(CreateNewMap.SizeType mapSize, Biome biome) {
        MapSize = mapSize;
        Biome = biome;
    }

    public string Serialize() {
        return JsonUtility.ToJson(this);
    }
}