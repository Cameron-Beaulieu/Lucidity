using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// TODO: Add other necessary fields for saving/loading files
// These include Layers, Terrain, and possibly more
[Serializable]
public class MapData {
    public Biome Biome;  
    [NonSerialized] public static string FileName;
    public List<MapObject> MapObjects;
    public Vector2 SpawnPoint;
    public List<string> LayerNames;

    /// <summary>
    /// Constructor for creating the initial file.
    /// </summary/>
    /// <param name="fileName">
    /// The absolute path/filename of the map.
    /// </param>
    /// <param name="biome">
    /// The biome of the newly created map.
    /// </param>
    public MapData(string fileName, Biome biome) {
        FileName = fileName;
        Biome = biome;
        SpawnPoint = Vector2.zero;
        MapObjects = new List<MapObject>();
        LayerNames = new List<string>{"Layer0"};
    }

    /// <summary>
    /// Constructor for a non-empty map that is ready to be saved.
    /// </summary/>
    /// <param name="fileName">
    /// The absolute path/filename of the map.
    /// </param>
    /// <param name="biome">
    /// The biome of the map.
    /// </param>
    /// <param name="mapObjects">
    /// A dictionary containing all of the <c>MapObject</c> instances currently on the screen.
    /// </param>
    /// <param name="spawnPoint">
    /// The x,z position of the player when the map is converted to 3D.
    /// </param>
    /// <param name="layers">
    /// A list of dictionaries where each dictionary represents a layer and stores the
    /// <c>MapObjects</c> along with their IDs that belong to that layer.
    /// </param>
    /// <param name="layerIndex">
    /// A dictionary containing the name of each layer with their associated indices.
    /// </param>
    public MapData(Biome biome, Vector2 spawnPoint, List<Dictionary<int, MapObject>> layers, 
                   Dictionary<string, int> layerIndex) {
        Biome = biome;
        SpawnPoint = spawnPoint;
        MapObjects = new List<MapObject>();
        LayerNames = new List<string>(layerIndex.Count);
        // Layer names may be added out of order in the next loop, this is to allocate the space
        for (int i = 0; i < layerIndex.Count; i++) {
            LayerNames.Add("");
        }

        // Make sure that the layer names are in order based on index
        foreach (KeyValuePair<string, int> kvp in layerIndex) {
            LayerNames[kvp.Value] = kvp.Key;
        }
        // Add each MapObject to the list of MapObjects with their associated layer names
        int index = 0;
        foreach (Dictionary<int, MapObject> dict in layers) {
            foreach (KeyValuePair<int, MapObject> kvp in dict) {
                MapObjects.Add(new MapObject(kvp.Value, LayerNames[index]));
            }
            index++;
        }

        // Remove deleted MapObjects
        MapObjects.RemoveAll(obj => obj.IsActive == false);
    }

    /// <summary>
    /// Serializes an entire instance of the class into a json string.
    /// </summary>
    public string Serialize() {
        // Give a second parameter of true to format the json nicely
        return JsonUtility.ToJson(this);
    }

    /// <summary>
    /// Deserializes a MapData object that was saved as json.
    /// </summary>
    /// <param name="filePath">
    /// The path to the json file being deserialized.
    /// </param>
    public static MapData Deserialize(string filePath) {
        string jsonContent = File.ReadAllText(filePath);
        return JsonUtility.FromJson<MapData>(jsonContent);
    }
}