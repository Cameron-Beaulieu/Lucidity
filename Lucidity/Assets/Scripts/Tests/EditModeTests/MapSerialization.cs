using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class MapSerialization {

    private MapData _mockMapData = new MapData(0, new Biome(0, "5d875c"), 
                                               new Dictionary<int, MapObject> {
                                                    {0, new MapObject(0, "Tree", 0, 
                                                                      new Vector2(0, 0), 
                                                                      new Vector2(100,100), 
                                                                      new Vector3(1, 1, 1), 
                                                                      Quaternion.identity, true)}},
                                                new Vector2(0, 0));
    private string _mockSerializedData = "{\"MapSize\":0,\"Biome\":{\"ID\":0,\"Name\":\"5d875c\"},"
        + "\"MapObjects\":[{\"ID\":0,\"Name\":\"Tree\",\"PrefabIndex\":0,\"MapPosition\":"
            + "{\"x\":0.0,\"y\":0.0},\"MapOffset\":{\"x\":100.0,\"y\":100.0},\"Scale\":{\"x\":1.0,"
                + "\"y\":1.0,\"z\":1.0},\"Rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0},"
                    + "\"IsActive\":true}],\"SpawnPoint\":{\"x\":0.0,\"y\":0.0}}";

    // A Test behaves as an ordinary method
    [Test]
    public void SerializesDataCorrectly() {
        string serializedData = _mockMapData.Serialize();
        Asset.AreEqual(serializedData, _mockSerializedData);
                       
    }

    [Test]
    public void DeserializesDataCorrectly() {
        File.WriteAllText("DeserializesDataCorrectly.json", _mockSerializedData);
        MapData deserializedData = MapData.Deserialize("DeserializesDataCorrectly.json");
        Asset.AreEqual(deserializedData, _mockMapData);
    }
}
