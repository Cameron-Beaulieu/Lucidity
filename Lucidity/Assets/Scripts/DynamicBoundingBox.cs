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
		dynamicAssetImage.transform.localScale *= _dynamicSideLength * AssetOptions.BrushSize;
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
		obj.GetComponent<Mouse>().enabled = false;
		obj.transform.localScale /= _dynamicSideLength * AssetOptions.BrushSize;

		if (_dynamicSideLength == 1) {
			obj.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), Quaternion.identity);
		} else if (_dynamicSideLength > 1) {
			float offset;
			if (_dynamicSideLength % 2 == 0) {
				offset = obj.transform.localScale.x * 0.5f;
			} else {
				offset = obj.transform.localScale.x * -0.5f * (AssetOptions.BrushSize - 1);
			}
			obj.transform.SetLocalPositionAndRotation(
				new Vector3(offset - obj.transform.localScale.x
								* Mathf.Ceil((_dynamicSideLength - 1f) / 2f)
								* AssetOptions.BrushSize + obj.transform.localScale.x
								* (((AssetOptions.BrushSize * _dynamicSideLength) - 1f)
								/ (_dynamicSideLength - 1f)) * xOffset,
							offset - obj.transform.localScale.y
								* Mathf.Ceil((_dynamicSideLength - 1f) / 2f)
								* AssetOptions.BrushSize + obj.transform.localScale.y
								* (((AssetOptions.BrushSize * _dynamicSideLength) - 1f)
								/ (_dynamicSideLength - 1f)) * yOffset,
							0),
				Quaternion.identity);
		}
		obj.transform.localScale = new Vector3(obj.transform.localScale.x + Zoom.zoomFactor,
											   obj.transform.localScale.y + Zoom.zoomFactor,
											   obj.transform.localScale.z + Zoom.zoomFactor);
		return obj;
	}
}
