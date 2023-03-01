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

    public override string ToString() {
        return $"Id: {Id}, Name: {Name}, PrefabIndex: {PrefabIndex}, MapPosition: {MapPosition}, "
            + $"MapOffset: {MapOffset}, Scale: {Scale}, Rotation: {Rotation}, IsActive: {IsActive}";
    }
}