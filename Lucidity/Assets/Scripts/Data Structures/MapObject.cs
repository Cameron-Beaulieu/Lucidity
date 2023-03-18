using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapObject {
    public int Id;
    public string Name;
    public int PrefabIndex;
    public Vector2 MapPosition;
    public Vector2 MapOffset;
    public Vector3 Scale;
    public Quaternion Rotation;
    public bool IsActive;
    public string LayerName;

    /// <summary>
    /// MapObject constructor, initializing the id, asset, mapPosition, scale, and rotation
    /// </summary>
    /// <param name="id">
    /// ID used as a key within the MapObject dictionary within MapEditorManager
    /// </param>
    /// <param name="name">
    /// name represents the type of the asset (fortress, tree, etc.)
    /// </param>
    /// <param name="prefabIndex">
    /// The index of the prefab (in <c>MapEditorManager.AssetPrefabs</c>) associated with the <c>MapObject</c>
    /// </param>
    /// <param name="mapPosition">
    /// A 2D point that represents the (x,y) position of the asset on the 2D and 3D map
    /// </param>
    /// <param name="mapOffset">
    /// A 2D point that represents the (x,y) position of the parent of the asset that
    /// must be used to offset the asset's position on the map
    /// </param>
    /// <param name="scale">
    /// A value representing the <c>localScale</c> of the asset within the map
    /// </param>
    /// <param name="rotation">
    /// A <c>Quaternion</c> representing the rotation of the asset within the map
    /// </param>
    /// <param name="isActive">
    /// A <c>boolean</c> representing whether or not the asset is active in the map.
    /// </param>
    public MapObject (int id, string name, int prefabIndex, Vector2 mapPosition, Vector2 mapOffset,
                      Vector3 scale, Quaternion rotation, bool isActive) {
        Id = id;
        Name = name;
        PrefabIndex = prefabIndex;
        MapPosition = mapPosition;
        MapOffset = mapOffset;
        Scale = scale;
        Rotation = rotation;
        IsActive = isActive;
    }

    /// <summary>
    /// Alternate constructor that allows the layerName to be added.
    /// </summary>
    /// <param name="id">
    /// ID used as a key within the MapObject dictionary within MapEditorManager
    /// </param>
    /// <param name="name">
    /// name represents the type of the asset (fortress, tree, etc.)
    /// </param>
    /// <param name="prefabIndex">
    /// The index of the prefab (in <c>MapEditorManager.AssetPrefabs</c>) associated with the <c>MapObject</c>
    /// </param>
    /// <param name="mapPosition">
    /// A 2D point that represents the (x,y) position of the asset on the 2D and 3D map
    /// </param>
    /// <param name="mapOffset">
    /// A 2D point that represents the (x,y) position of the parent of the asset that
    /// must be used to offset the asset's position on the map
    /// </param>
    /// <param name="scale">
    /// A value representing the <c>localScale</c> of the asset within the map
    /// </param>
    /// <param name="rotation">
    /// A <c>Quaternion</c> representing the rotation of the asset within the map
    /// </param>
    /// <param name="isActive">
    /// A <c>boolean</c> representing whether or not the asset is active in the map.
    /// </param>
    /// <param name="layerName">
    /// A <c>string</c> representing the name of the layer the <c>MapObject</c> is placed on.
    /// </param>
    // public MapObject (int id, string name, int prefabIndex, Vector2 mapPosition, Vector2 mapOffset,
    //                   Vector3 scale, Quaternion rotation, bool isActive, string layerName) {
    //     Id = id;
    //     Name = name;
    //     PrefabIndex = prefabIndex;
    //     MapPosition = mapPosition;
    //     MapOffset = mapOffset;
    //     Scale = scale;
    //     Rotation = rotation;
    //     IsActive = isActive;
    //     LayerName = layerName;
    // }

    /// <summary>
    /// Alternate constructor that adds a layer name to a previously existing <c>MapObject</c>.
    /// </summary>
    /// <param name="obj">
    /// A previously existing <c>MapObject</c>.
    /// </param>
    /// <param name="layerName">
    /// A <c>string</c> representing the name of the layer the <c>MapObject</c> is placed on.
    /// </param>
    public MapObject (MapObject obj, string layerName) {
        Id = obj.Id;
        Name = obj.Name;
        PrefabIndex = obj.PrefabIndex;
        MapPosition = obj.MapPosition;
        MapOffset = obj.MapOffset;
        Scale = obj.Scale;
        Rotation = obj.Rotation;
        IsActive = obj.IsActive;
        LayerName = layerName;
    }
}