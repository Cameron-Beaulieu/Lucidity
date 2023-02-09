using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBoundingBox : MonoBehaviour {
	private MapEditorManager _editor;
	private static int _dynamicSideLength;  // Side length of the bounding box in number of assets

	public static int DynamicSideLength {
		get { return _dynamicSideLength; }
		set { _dynamicSideLength = value; }
	}

	void Start() {
		_editor = GameObject.FindGameObjectWithTag("MapEditorManager")
			.GetComponent<MapEditorManager>();
	}

	public static GameObject CreateDynamicAssetImage(GameObject baseAssetImage) {
		Vector2 worldPosition = Mouse.getMousePosition();
		GameObject dynamicAssetImage = Instantiate(baseAssetImage,
			new Vector3(worldPosition.x, worldPosition.y, 90),
			Quaternion.identity);
		dynamicAssetImage.name = "HoverDynamicBoundingBoxObject";
		dynamicAssetImage.transform.localScale *= _dynamicSideLength;
		Destroy(dynamicAssetImage.GetComponent<MeshRenderer>());
		Destroy(dynamicAssetImage.GetComponent<MeshFilter>());
		for (int i = 0; i < _dynamicSideLength; i++) {
			for (int j = 0; j < _dynamicSideLength; j++) {
				CreateDynamicAssetImageChild(dynamicAssetImage.transform, baseAssetImage, i, j);
			}
		}
		return dynamicAssetImage;
	}

	public static GameObject CreateDynamicAssetImageChild(Transform parentTransform,
			GameObject baseAssetImage,
			int xOffset,
			int yOffset) {
		GameObject obj = Instantiate(baseAssetImage, parentTransform, false);
		obj.name = "HoverDynamicBoundingBoxChild";
		obj.GetComponent<Mouse>().enabled = false;
		obj.transform.localScale /= _dynamicSideLength;
		float offset = obj.transform.localScale.x;

		obj.transform.SetLocalPositionAndRotation(
			new Vector3((-offset * (_dynamicSideLength - 1) / 2) + (offset * xOffset),
						(-offset * (_dynamicSideLength - 1) / 2) + (offset * yOffset),
						 0),
			Quaternion.identity);
		obj.transform.localScale = new Vector3(obj.transform.localScale.x + Zoom.zoomFactor,
											   obj.transform.localScale.y + Zoom.zoomFactor,
											   obj.transform.localScale.z + Zoom.zoomFactor);
		return obj;
	}
}
