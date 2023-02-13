using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBoundingBox : MonoBehaviour {
	private MapEditorManager _editor;
	private static int _dynamicSideLength;  // Side length of the bounding box in number of assets
	public static GameObject[,] Images;

	public static int DynamicSideLength {
		get { return _dynamicSideLength; }
		set { _dynamicSideLength = value; }
	}

	void Start() {
		_editor = GameObject.FindGameObjectWithTag("MapEditorManager")
			.GetComponent<MapEditorManager>();
	}

	/// <summary>
	/// Create a dynamic bounding box parent, with an appropriate n x n arrangement of asset images
	/// placed within it. To be used for creating an appropriate asset image for mouse hovering.
	/// </summary>
	/// <param name="baseAssetImage">
	/// <c>GameObject</c> corresponding to the desired asset to be shown on hover
	/// </param>
	/// <returns>
	/// <c>GameObject</c> parent, that is the dynamic bounding box
	/// </returns>
	public static GameObject CreateDynamicAssetImage(GameObject baseAssetImage) {
		Images = new GameObject[_dynamicSideLength, _dynamicSideLength];
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
				Images[i,j] = CreateDynamicAssetImageChild(dynamicAssetImage.transform,
														   baseAssetImage,
														   i,
														   j);
			}
		}
		return dynamicAssetImage;
	}

	/// <summary>
	/// Creation of a single asset image, that will populate the n x n shaped dynamic bounding box.
	/// </summary>
	/// <param name="parentTransform">
	/// <c>Transform</c> corresponding to the parent <c>GameObject</c>
	/// </param>
	/// <param name="baseAssetImage">
	/// <c>GameObject</c> corresponding to the desired asset to be shown on hover
	/// </param>
	/// <param name="xOffset">
	/// <c>int</c> corresponding to the x-position of the current asset image if the dynamic
	/// bounding box were represented as a grid
	/// </param>
	/// <param name="yOffset">
	/// <c>int</c> corresponding to the y-position of the current asset image if the dynamic
	/// bounding box were represented as a grid
	/// /// </param>
	/// <returns>
	/// <c>GameObject</c> corresponding to a single, current asset image of the
	/// dynamic bounding box to be displayed
	/// </returns>
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
								* AssetOptions.BrushSize + (obj.transform.localScale.x
								* (((AssetOptions.BrushSize * _dynamicSideLength) - 1f)
								/ (_dynamicSideLength - 1f)) + 1e-6f) * xOffset,
							offset - obj.transform.localScale.y
								* Mathf.Ceil((_dynamicSideLength - 1f) / 2f)
								* AssetOptions.BrushSize + (obj.transform.localScale.y
								* (((AssetOptions.BrushSize * _dynamicSideLength) - 1f)
								/ (_dynamicSideLength - 1f)) + 1e-6f) * yOffset,
							0),
				Quaternion.identity);
		}
		obj.transform.localScale = new Vector3(obj.transform.localScale.x + Zoom.zoomFactor,
											   obj.transform.localScale.y + Zoom.zoomFactor,
											   obj.transform.localScale.z + Zoom.zoomFactor);
		return obj;
	}
}
