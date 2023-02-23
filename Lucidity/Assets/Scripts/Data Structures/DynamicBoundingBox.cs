using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBoundingBox : MonoBehaviour {
    private MapEditorManager _editor;
    private static int _dynamicSideLength;  // Side length of the bounding box in number of assets
    private static GameObject[,] _images;
    private static HashSet<Vector2> _randomAssetArrangement = new HashSet<Vector2>();

    public static int DynamicSideLength {
        get { return _dynamicSideLength; }
        set { _dynamicSideLength = value; }
    }

    private void Start() {
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();
    }

    /// <summary>
    /// Create a dynamic bounding box parent, with an appropriate n x n arrangement of asset images
    /// placed within it. To be used for creating an appropriate asset image for mouse hovering.
    /// </summary>
    /// <param name="assetImage">
    /// <c>GameObject</c> corresponding to the desired asset to be shown on hover
    /// </param>
    /// <returns>
    /// <c>GameObject</c> parent, that is the dynamic bounding box
    /// </returns>
    public static GameObject CreateDynamicAssetImage(GameObject assetImage) {
        _images = new GameObject[_dynamicSideLength, _dynamicSideLength];
        Vector2 worldPosition = Mouse.GetMousePosition();
        GameObject dynamicAssetImage = Instantiate(assetImage,
            new Vector3(worldPosition.x, worldPosition.y, 90),
            Quaternion.identity);
        dynamicAssetImage.name = "HoverDynamicBoundingBoxObject";
        dynamicAssetImage.transform.localScale *= _dynamicSideLength * AssetOptions.BrushSize;
        Destroy(dynamicAssetImage.GetComponent<MeshRenderer>());
        Destroy(dynamicAssetImage.GetComponent<MeshFilter>());

        // Create a hovering asset image to make an n x n shaped dynamic bounding box that will
        // follow the cursor
        for (int i = 0; i < _dynamicSideLength; i++) {
            for (int j = 0; j < _dynamicSideLength; j++) {
                Vector2 offset = new Vector2(i, j);
                _images[i,j] = CreateDynamicAssetImageChild(assetImage,
                                                            offset,
                                                            dynamicAssetImage.transform);
            }
        }

        return dynamicAssetImage;
    }

    /// <summary>
    /// Creation of a single asset image, that will populate the n x n shaped dynamic bounding box.
    /// </summary>
    /// <param name="assetImage">
    /// <c>GameObject</c> corresponding to the desired asset to be shown on hover
    /// </param>
    /// <param name="coordinate">
    /// <c>Vector2</c> corresponding to the x and y offsets of the current asset image if the
    /// dynamic bounding box were represented as a grid
    /// </param>
    /// <param name="parentTransform">
    /// <c>Transform</c> corresponding to the parent <c>GameObject</c>
    /// </param>
    /// <returns>
    /// <c>GameObject</c> corresponding to a single, current asset image of the
    /// dynamic bounding box to be displayed
    /// </returns>
    public static GameObject CreateDynamicAssetImageChild(GameObject assetImage,
                                                          Vector2 coordinate,
                                                          Transform parentTransform) {
        GameObject obj = Instantiate(assetImage, parentTransform, false);
        obj.GetComponent<Mouse>().enabled = false;
        obj.transform.localScale /= _dynamicSideLength * AssetOptions.BrushSize;
        Vector3 position = GetOffsetPosition(obj, coordinate);
        obj.transform.SetLocalPositionAndRotation(position, Quaternion.identity);
        obj.transform.localScale = new Vector3(obj.transform.localScale.x + Zoom.zoomFactor,
                                               obj.transform.localScale.y + Zoom.zoomFactor,
                                               obj.transform.localScale.z + Zoom.zoomFactor);
        return obj;
    }

    /// <summary>
    /// Create a dynamic bounding box with appropriate position, tag, and scale. Moreover, remove
    /// its renderer components so that it is not visible. 
    /// </summary>
    /// <param name="assetPrefab">
    /// <c>GameObject<c> corresponding to the desired prefab that will be used
    /// </prefab>
    /// <return>
    /// <c>GameObject</c> corresponding to the dynamic bounding box
    /// </return>
    public static GameObject CreateDynamicBoundingBox(GameObject assetPrefab) {
        Vector2 worldPosition = Mouse.GetMousePosition();
        GameObject dynamicBoundingBox = Instantiate(
            assetPrefab,
            new Vector3(worldPosition.x, worldPosition.y, 90),
            Quaternion.identity);
        dynamicBoundingBox.name = "DynamicBoundingBox";
        dynamicBoundingBox.tag = "DynamicBoundingBox";
        dynamicBoundingBox.transform.localScale = 
            new Vector3(dynamicBoundingBox.transform.localScale.x + Zoom.zoomFactor,
                        dynamicBoundingBox.transform.localScale.y + Zoom.zoomFactor,
                        dynamicBoundingBox.transform.localScale.z + Zoom.zoomFactor)
            * _dynamicSideLength * AssetOptions.BrushSize;
        Destroy(dynamicBoundingBox.GetComponent<MeshRenderer>());
        Destroy(dynamicBoundingBox.GetComponent<MeshFilter>());
        return dynamicBoundingBox;
    }

    /// <summary>
    /// Create the desired number of assets, placed in the space of the dynamic bounding box.
    /// </summary>
    /// <param name="assetPrefab">
    /// <c>GameObject<c> corresponding to the desired prefab that will be used
    /// </param>
    /// <returns>
    /// <c>List</c> of <c>GameObject<c>, corresponding to all newly created objects
    /// </returns>
    public static List<GameObject> CreateAssets(GameObject assetPrefab) {
        List<GameObject> assets = new List<GameObject>();
        foreach (Vector2 coordinate in _randomAssetArrangement) {
            // Retrieve position based on the appropriate coordinate of the hovering asset image
            GameObject newGameObject = CreateNewObject(assetPrefab, new Vector3(
                (_images[(int)coordinate.x, (int)coordinate.y]).transform.position.x,
                (_images[(int)coordinate.x, (int)coordinate.y]).transform.position.y,
                (_images[(int)coordinate.x, (int)coordinate.y]).transform.position.z));
            if (newGameObject != null) {
                assets.Add(newGameObject);
            }
        }
        return assets;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assetPrefab">
    /// <c>GameObject<c> corresponding to the desired prefab that will be used
    /// </param>
    /// <param name="position">
    /// <c>Vector3<c> corresponding to the desired position of the <c>MapObject</c>
    /// </param>
    /// <returns>
    /// <c>GameObject</c> corresponding to the new <c>MapObject</c>. If a valid <c>MapObject</c>
    /// cannot be made, return <c>null</c>
    /// </returns>
    public static GameObject CreateNewObject(GameObject assetPrefab, Vector3 position) {
        GameObject parent = new GameObject();
        parent.name = assetPrefab.name + " Parent";
        parent.transform.SetParent(MapEditorManager.MapContainer.transform, true);
        parent.transform.position = new Vector3(position.x, position.y, position.z);
        parent.transform.localPosition = new Vector3(
            parent.transform.localPosition.x,
            parent.transform.localPosition.y, 0);

        GameObject newGameObject = Instantiate(assetPrefab, position, Quaternion.identity,
                                               parent.transform);
        if (newGameObject != null && !newGameObject.GetComponent<AssetCollision>()
                .IsInvalidPlacement()) {
            return newGameObject;
        } else {
            Destroy(newGameObject);
            Destroy(parent);
            return null;
        }
    }

    /// <summary>
    ///	Generate random coordinate pairs (with no repeating pair) up to the desired number of
    /// assets. These will be used in selecting the random arrangement of grouped assets.
    /// </summary>
    public static void GenerateRandomCoordinates() {
        do {
            _randomAssetArrangement.Add(new Vector2(
                Random.Range(0, _dynamicSideLength),
                Random.Range(0, _dynamicSideLength)));
        } while (_randomAssetArrangement.Count < AssetOptions.AssetCount);
    }

    /// <summary>
    /// Given coordinate position of a specific child object, retrieve its position relative to
    /// its parent's dynamic size.
    /// </summary>
    /// <param name="obj">
    /// Instantiated <c>GameObject</c> parent
    /// </param>
    /// <param name="coordinate">
    /// <c>Vector2</c> corresponding to the x and y offsets of the current asset image if the
    /// dynamic bounding box were represented as a grid
    /// </param>
    /// <returns>
    /// <c>Vector3<c> corresponding to the appropriate local position of the <c>GameObject</c>
    /// </returns>
    public static Vector3 GetOffsetPosition(GameObject obj, Vector2 coordinate) {
        if (_dynamicSideLength > 1) {
            float offset;
            if (_dynamicSideLength % 2 == 0) {
                offset = obj.transform.localScale.x * 0.5f;
            } else {
                offset = obj.transform.localScale.x * -0.5f * (AssetOptions.BrushSize - 1);
            }
            return new Vector3(
                offset - obj.transform.localScale.x * Mathf.Ceil((_dynamicSideLength - 1f) / 2f)
                    * AssetOptions.BrushSize + (obj.transform.localScale.x
                    * (((AssetOptions.BrushSize * _dynamicSideLength) - 1f)
                    / (_dynamicSideLength - 1f)) + 1e-6f) * coordinate.x,
                offset - obj.transform.localScale.y * Mathf.Ceil((_dynamicSideLength - 1f) / 2f)
                    * AssetOptions.BrushSize + (obj.transform.localScale.y
                    * (((AssetOptions.BrushSize * _dynamicSideLength) - 1f)
                    / (_dynamicSideLength - 1f)) + 1e-6f) * coordinate.y,
                0);
        }
        return new Vector3(0, 0, 0);
    }

    public static void DeleteDynamicBoundingBoxes() {
        GameObject[] dynamicBoundingBoxes
            = GameObject.FindGameObjectsWithTag("DynamicBoundingBox");
        if (dynamicBoundingBoxes.Length > 0) {
            foreach (GameObject dynamicBoundingBox in dynamicBoundingBoxes) {
                Destroy(dynamicBoundingBox);
            }
        }
    }
}
