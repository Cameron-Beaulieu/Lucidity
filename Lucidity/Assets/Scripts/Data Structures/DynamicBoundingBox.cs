using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBoundingBox : MonoBehaviour {
    private MapEditorManager _editor;
    private static int _dynamicSideLength;  // Side length of the bounding box in number of assets
    private static HashSet<Vector2> _randomAssetArrangement;

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
    /// <param name="position">
    /// <c>Vector2</c> corresponding to the position of the cursor
    /// </param>
    /// <returns>
    /// <c>GameObject</c> parent, that is the dynamic bounding box
    /// </returns>
    public static GameObject CreateDynamicAssetImage(GameObject assetImage, Vector2 position) {
        GameObject dynamicAssetImage = Instantiate(assetImage,
            new Vector3(position.x, position.y, 90),
            Quaternion.identity);
        dynamicAssetImage.name = "HoverDynamicBoundingBoxObject";
        dynamicAssetImage.transform.localScale *= _dynamicSideLength * AssetOptions.Spread;
        // Remove old material 
        dynamicAssetImage.GetComponent<SpriteRenderer>().materials = new Material[0];

        GenerateRandomCoordinates();

        // Create a hovering asset image in each randomly assigned coordinate position
        foreach (Vector2 coordinate in _randomAssetArrangement) {
            CreateDynamicAssetImageChild(assetImage, coordinate, dynamicAssetImage.transform);
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
    public static void CreateDynamicAssetImageChild(GameObject assetImage,
                                                    Vector2 coordinate,
                                                    Transform parentTransform) {
        GameObject obj = Instantiate(assetImage, parentTransform, false);
        obj.GetComponent<Mouse>().enabled = false;
        obj.transform.localScale /= _dynamicSideLength * AssetOptions.Spread;
        Vector3 offsetPosition = GetOffsetPosition(coordinate, obj.transform.localScale,
                                                   obj.GetComponent<SpriteRenderer>().size);
        obj.transform.SetLocalPositionAndRotation(offsetPosition * (Zoom.zoomFactor + 1), Quaternion.identity);
        obj.transform.localScale = new Vector3(obj.transform.localScale.x * (Zoom.zoomFactor + 1),
                                               obj.transform.localScale.y * (Zoom.zoomFactor + 1),
                                               obj.transform.localScale.z * (Zoom.zoomFactor + 1));
    }

    /// <summary>
    /// Create a dynamic bounding box with appropriate position, tag, and scale. Moreover, remove
    /// its renderer components so that it is not visible. 
    /// </summary>
    /// <param name="assetPrefab">
    /// <c>GameObject<c> corresponding to the desired prefab that will be used
    /// </prefab>
    /// <param name="position">
    /// <c>Vector2</c> corresponding to the position of the cursor
    /// </param>
    /// <return>
    /// <c>GameObject</c> corresponding to the dynamic bounding box
    /// </return>
    public static GameObject CreateDynamicBoundingBox(GameObject assetPrefab, Vector2 position) {
        GameObject dynamicBoundingBox = Instantiate(assetPrefab,
                                                    new Vector3(position.x, position.y, 90),
                                                    Quaternion.identity);
        dynamicBoundingBox.name = "DynamicBoundingBox";
        dynamicBoundingBox.tag = "DynamicBoundingBox";
        dynamicBoundingBox.transform.localScale =
            new Vector3(dynamicBoundingBox.transform.localScale.x * (Zoom.zoomFactor + 1),
                        dynamicBoundingBox.transform.localScale.y * (Zoom.zoomFactor + 1),
                        dynamicBoundingBox.transform.localScale.z * (Zoom.zoomFactor + 1))
                * _dynamicSideLength * AssetOptions.Spread;
        Destroy(dynamicBoundingBox.GetComponent<SpriteRenderer>());

        // Change the collider of the dynamic bounding box to a consistent rectangle
        if (dynamicBoundingBox.GetComponent<PolygonCollider2D>()) {
            Vector2 size =
                new Vector2(dynamicBoundingBox.GetComponent<RectTransform>().rect.width,
                            dynamicBoundingBox.GetComponent<RectTransform>().rect.height);
            size = Vector2.Scale(size, new Vector2(0.5f, 0.5f));
            Vector2[] points = {
                Vector2.Scale(size, Vector2.one),
                Vector2.Scale(size, new Vector2(-1, 1)),
                Vector2.Scale(size, new Vector2(-1, -1)),
                Vector2.Scale(size, new Vector2(1, -1))
            };
            dynamicBoundingBox.GetComponent<PolygonCollider2D>().SetPath(0, points);
        }

        return dynamicBoundingBox;
    }

    /// <summary>
    /// Create the desired number of assets, placed in the space of the dynamic bounding box.
    /// </summary>
    /// <param name="assetPrefab">
    /// <c>GameObject<c> corresponding to the desired prefab that will be used
    /// </param>
    /// <param name="dynamicBoundingBox">
    /// <c>GameObject</c> corresponding to the dynamic bounding box that the asset resides in
    /// </param>
    /// <returns>
    /// <c>List</c> of <c>GameObject<c>, corresponding to all newly created objects
    /// </returns>
    public static List<GameObject> CreateAssets(GameObject assetPrefab,
                                                GameObject dynamicBoundingBox) {
        List<GameObject> assets = new List<GameObject>();
        foreach (Vector2 coordinate in _randomAssetArrangement) {
            GameObject newGameObject = CreateNewObject(assetPrefab,
                                                       coordinate,
                                                       dynamicBoundingBox);
            if (newGameObject != null) {
                assets.Add(newGameObject);
            }
        }
        return assets;
    }

    /// <summary>
    /// Create an instance of the desired <c>GameObject</c> in an appropriate position.
    /// </summary>
    /// <param name="assetPrefab">
    /// <c>GameObject<c> corresponding to the desired prefab that will be used
    /// </param>
    /// <param name="coordinate">
    /// <c>Vector2</c> corresponding to the x and y offsets of the current asset image if the
    /// dynamic bounding box were represented as a grid
    /// </param>
    /// <param name="dynamicBoundingBox">
    /// <c>GameObject</c> corresponding to the dynamic bounding box that the asset resides in
    /// </param>
    /// <returns>
    /// <c>GameObject</c> corresponding to the new <c>MapObject</c>. If a valid <c>MapObject</c>
    /// cannot be made, return <c>null</c>
    /// </returns>
    public static GameObject CreateNewObject(GameObject assetPrefab,
                                             Vector2 coordinate,
                                             GameObject dynamicBoundingBox) {
        GameObject parent = new GameObject();
        parent.name = assetPrefab.name + " Parent";
        parent.transform.SetParent(MapEditorManager.MapContainer.transform, true);
        parent.transform.localScale =
            new Vector3(Util.ParentAssetDefaultScale, Util.ParentAssetDefaultScale,
                        Util.ParentAssetDefaultScale);

        Vector3 scale = Vector3.one / (_dynamicSideLength * AssetOptions.Spread);
        Vector3 relativePosition =
            dynamicBoundingBox.transform.TransformPoint(GetOffsetPosition(
                coordinate,
                scale,
                new Vector2(dynamicBoundingBox.GetComponent<RectTransform>().rect.width,
                            dynamicBoundingBox.GetComponent<RectTransform>().rect.height)));
        parent.transform.position = relativePosition;

        GameObject newGameObject = Instantiate(assetPrefab, parent.transform);
        newGameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
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
        _randomAssetArrangement = new HashSet<Vector2>();
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
    /// <param name="coordinate">
    /// <c>Vector2</c> corresponding to the x and y offsets of the current asset image if the
    /// dynamic bounding box were represented as a grid
    /// </param>
    /// <param name="scale">
    /// <c>Vector2</c> corresponding to the relative scales of new <c>GameObject</c> inside the
    /// bounding box
    /// </param>
    /// <param name="size">
    /// <c>Vector2</c> corresponds to the size of the new <c>GameObject</c> inside the bounding box
    /// </param>
    /// <returns>
    /// <c>Vector2<c> corresponding to the appropriate local position of the <c>GameObject</c>
    /// </returns>
    public static Vector2 GetOffsetPosition(Vector2 coordinate, Vector2 scale, Vector2 size) {
        if (_dynamicSideLength > 1) {
            float offset;
            if (_dynamicSideLength % 2 == 0) {
                offset = scale.x * 0.5f;
            } else {
                offset = scale.x * -0.5f * (AssetOptions.Spread - 1);
            }
            return new Vector3(
                (offset - scale.x * Mathf.Ceil((_dynamicSideLength - 1f) / 2f)
                    * AssetOptions.Spread + (scale.x
                    * (((AssetOptions.Spread * _dynamicSideLength) - 1f)
                    / (_dynamicSideLength - 1f))) * coordinate.x) * size.x,
                (offset - scale.y * Mathf.Ceil((_dynamicSideLength - 1f) / 2f)
                    * AssetOptions.Spread + (scale.y
                    * (((AssetOptions.Spread * _dynamicSideLength) - 1f)
                    / (_dynamicSideLength - 1f))) * coordinate.y) * size.y);
        }
        return Vector2.zero;
    }

    /// <summary>
    /// Delete all existing dynamic bounding boxes.
    /// </summary>
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
