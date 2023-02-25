using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// TODO: Add other necessary fields for saving/loading files
// These include Layers, Terrain, and possibly more
[Serializable]
public class MapData {
    public CreateNewMap.SizeType MapSize;
    public Biome Biome;  
    [NonSerialized] public static string FileName;
    public List<MapObject> MapObjects;
    public Vector2 SpawnPoint;

    /// <summary>
    /// Constructor for creating the initial file.
    /// </summary/>
    /// <param name="fileName">
    /// The absolute path/filename of the map.
    /// </param>
    /// <param name="mapSize">
    /// The <c>CreateNewMap.SizeType</c> enumeration of the map size.
    /// </param>
    /// <param name="biome">
    /// The biome of the newly created map.
    /// </param>
    public MapData(string fileName, CreateNewMap.SizeType mapSize, Biome biome) {
        FileName = fileName;
        MapSize = mapSize;
        Biome = biome;
    }

    /// <summary>
    /// Constructor for a non-empty map that is ready to be saved.
    /// </summary/>
    /// <param name="fileName">
    /// The absolute path/filename of the map.
    /// </param>
    /// <param name="mapSize">
    /// The <c>CreateNewMap.SizeType</c> enumeration of the map size.
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
    public MapData(CreateNewMap.SizeType mapSize, Biome biome, 
                   Dictionary<int, MapObject> mapObjects, Vector2 spawnPoint) {
        MapSize = mapSize;
        Biome = biome;
        // Unity can't serialize a dictionary, so the values are converted to a list
        // Each MapObject in the list contains its ID, which will be used as the key
        // when converted back to a dictionary
        MapObjects = new List<MapObject>(mapObjects.Values);
        // Remove deleted MapObjects
        MapObjects.RemoveAll(obj => obj.IsActive == false);
        SpawnPoint = spawnPoint;
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

    public override string ToString() {
        string mapObjectsString = "[";
        foreach (MapObject mapObject in MapObjects) {
            mapObjectsString += "{" + mapObject.ToString() + "}, ";
        }
        mapObjectsString += "]";

        return $"Biome: {Biome.Name}, MapObjects: {mapObjectsString}, SpawnPoint: {SpawnPoint}";
    }
}