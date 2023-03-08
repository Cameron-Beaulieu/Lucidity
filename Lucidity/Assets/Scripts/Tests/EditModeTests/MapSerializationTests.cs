using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.TestTools;

[TestFixture]
public class MapSerializationTests {
    private MapData _mockMapData = new MapData(
        new Biome(0),
        new Vector2(0, 0),
        new List<Dictionary<int, MapObject>> {new Dictionary<int, MapObject> {
            {0, new MapObject(new MapObject(0, "Tree", 3, new Vector2(0, 0), 
            new Vector2(-100,200), new Vector3(1, 1, 1), Quaternion.identity, true), 
            "Layer0")}},
            new Dictionary<int, MapObject> {
            {1, new MapObject(new MapObject(1, "House", 1, new Vector2(0, 0), 
            new Vector2(250,0), new Vector3(1, 1, 1), Quaternion.identity, true), 
            "Layer0")}}},
        new Dictionary<string, int> {{"Layer0", 0}, {"a different name", 1}});

    private string _mockSerializedData = "{\"Biome\":{\"_name\":0,\"_groundColour\":\"5d875c\"}"
        + ",\"MapObjects\":[{\"Id\":0,\"Name\":\"Tree\",\"PrefabIndex\":3,\"MapPosition\":"
        + "{\"x\":0.0,\"y\":0.0},\"MapOffset\":{\"x\":-100.0,\"y\":200.0},\"Scale\":{"
        + "\"x\":1.0,\"y\":1.0,\"z\":1.0},"
        + "\"Rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0},\"IsActive\":true,"
        + "\"LayerName\":\"Layer0\"},{\"Id\":1,\"Name\":\"House\",\"PrefabIndex\":1,"
        + "\"MapPosition\":{\"x\":0.0,\"y\":0.0},\"MapOffset\":{\"x\":250.0,\"y\":0.0},"
        + "\"Scale\":{\"x\":1.0,\"y\":1.0,\"z\":1.0},"
        + "\"Rotation\":{\"x\":0.0,\"y\":0.0,\"z\":0.0,\"w\":1.0},\"IsActive\":true,"
        + "\"LayerName\":\"a different name\"}],\"SpawnPoint\":{\"x\":0.0,\"y\":0.0},"
        + "\"LayerNames\":[\"Layer0\",\"a different name\"]}";

    [Test]
    public void SerializesDataCorrectly() {
        string serializedData = _mockMapData.Serialize();
        Assert.AreEqual(serializedData, _mockSerializedData);      
    }

    [Test]
    public void DeserializesDataCorrectly() {
        File.WriteAllText("DeserializesDataCorrectly.json", _mockSerializedData);
        MapData deserializedData = MapData.Deserialize("DeserializesDataCorrectly.json");
        Assert.AreEqual(deserializedData.Biome.Name, _mockMapData.Biome.Name);
        int i = 0;
        foreach(MapObject mapObject in _mockMapData.MapObjects) {
            Assert.True(deserializedData.MapObjects[i].Id == mapObject.Id);
            Assert.AreEqual(mapObject.Name, deserializedData.MapObjects[i].Name);
            Assert.AreEqual(deserializedData.MapObjects[i].PrefabIndex, mapObject.PrefabIndex);
            Assert.True(deserializedData.MapObjects[i].MapPosition.x == mapObject.MapPosition.x);
            Assert.True(deserializedData.MapObjects[i].MapPosition.y == mapObject.MapPosition.y);
            Assert.True(deserializedData.MapObjects[i].MapOffset.x == mapObject.MapOffset.x);
            Assert.True(deserializedData.MapObjects[i].MapOffset.y == mapObject.MapOffset.y);
            Assert.True(deserializedData.MapObjects[i].Scale.x == mapObject.Scale.x);
            Assert.True(deserializedData.MapObjects[i].Scale.y == mapObject.Scale.y);
            Assert.True(deserializedData.MapObjects[i].Scale.z == mapObject.Scale.z);
            Assert.True(deserializedData.MapObjects[i].Rotation.x == mapObject.Rotation.x);
            Assert.True(deserializedData.MapObjects[i].Rotation.y == mapObject.Rotation.y);
            Assert.True(deserializedData.MapObjects[i].Rotation.z == mapObject.Rotation.z);
            Assert.True(deserializedData.MapObjects[i].Rotation.w == mapObject.Rotation.w);
            Assert.AreEqual(deserializedData.MapObjects[i].IsActive, mapObject.IsActive);
            i++;
        }
        Assert.True(deserializedData.SpawnPoint.x == _mockMapData.SpawnPoint.x);
        Assert.True(deserializedData.SpawnPoint.y == _mockMapData.SpawnPoint.y);
        int j = 0;
        foreach(string layer in _mockMapData.LayerNames) {
            Assert.AreEqual(deserializedData.LayerNames[j], layer);
            j++;
        }
        File.Delete("DeserializesDataCorrectly.json");
        Assert.IsFalse(File.Exists("DeserializesDataCorrectly.json"));
    }
}
