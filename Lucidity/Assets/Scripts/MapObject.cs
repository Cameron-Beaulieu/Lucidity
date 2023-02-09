using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MapObject {

	public int Id;
    public GameObject Asset;
    public Vector2 MapPosition;
    public Vector3 Scale;
    public Quaternion Rotation;
    public bool IsActive;

	/// <summary>
	/// MapObject constructor, initializing the id, asset, mapPosition, scale, and rotation
	/// </summary>
	/// <param name="id">
	/// ID used as a key within the MapObject dictionary within MapEditorManager
	/// </param>
    /// <param name="asset">
	/// The asset within the scene that will be attached to the MapObject
	/// </param>
    /// <param name="mapPosition">
	/// A 2D point that represents the (x,y) position of the asset on the 2D and 3D map
	/// </param>
    /// <param name="scale">
	/// A value representing the localScale of the asset within the map
	/// </param>
    /// <param name="rotation">
	/// A Quaternion representing the rotation of the asset within the map
	/// </param>
	public MapObject (int id, GameObject asset, Vector2 mapPosition, Vector3 scale, 
        Quaternion rotation, bool isActive) {
		Id = id;
        Asset = asset;
        MapPosition = mapPosition;
        Scale = scale;
        Rotation = rotation;
	}
}