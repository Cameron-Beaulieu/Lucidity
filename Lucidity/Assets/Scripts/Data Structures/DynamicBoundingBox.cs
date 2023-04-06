using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicBoundingBox : MonoBehaviour {
    private MapEditorManager _editor;
    private static int _dynamicSideLength;  // Side length of the bounding box in number of assets
    private static List<List<Vector2>> _assetArrangements = new List<List<Vector2>>();

    public static int DynamicSideLength {
        get { return _dynamicSideLength; }
        set { _dynamicSideLength = value; }
    }

    public static List<List<Vector2>> AssetArrangements {
        get { return _assetArrangements; }
        set { _assetArrangements = value; }
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
        dynamicAssetImage.AddComponent<Outline>();

        if (AssetOptions.Random) {
            GenerateRandomCoordinates();
        } else {
            GenerateUniformCoordinates();
        }

        // Create a hovering asset image in each randomly assigned coordinate position
        foreach (Vector2 coordinate in _assetArrangements[AssetOptions.Variation]) {
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
        foreach (Vector2 coordinate in _assetArrangements[AssetOptions.Variation]) {
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
    /// Generate all coordinate pair variations (with no repeating pairs) up to the desired number
    /// of assets. These will be used in selecting the asset grouping.
    /// </summary>
    public static void GenerateVariations() {
        _assetArrangements.Clear();
        // Permute all possible variations
        List<List<Vector2>> placement = Permute((int)Mathf.Pow(_dynamicSideLength, 2) - 1,
                                                new List<Vector2>());
        foreach (List<Vector2> arrangement in placement) {
            // If there is not a correct number of assets selected, the current variation is not
            // valid and is not added to the list of possible arrangements
            if (arrangement.Count() == AssetOptions.AssetCount) {
                _assetArrangements.Add(arrangement);
            }
        }
        placement.Clear();
    }

    /// <summary>
    /// Permutes all possible arrangements using recursion. Specifies <c>index</c> as depth, and
    /// <c>currVariation</c> as the current asset arrangement. Recursion is done by assuming the
    /// next position in the dynamic bounding box is either included, or not included in the final
    /// arrangement.
    /// </summary>
    /// <param name="index">
    /// <c>int<c> representing the remaining recursion levels (i.e., positions)
    /// </param>
    /// <param name="currVariation">
    /// <c>List</c> of <c>Vector2</c> representing the current arrangement for the variation
    /// </param>
    /// <returns>
    /// <c>List</c> of arrangements for the current asset count
    /// </returns>
    private static List<List<Vector2>> Permute(int index, List<Vector2> currVariation) {
        // Base case: if there are no more positions to explore, simply return the variation
        if (index < 0) {
            List<List<Vector2>> list = new List<List<Vector2>>();
            list.Add(currVariation);
            return list;
        // Recursive case: add the variation so far along with the current position to the
        // arrangement and perform a recursive invocation. Perform a recursive invocation
        // on the original variation (assuming the current position is not added). Combine the
        // returned lists and return it
        } else {
            // Case where the current position is not included in the arrangement
            List<Vector2> permutation1 = new List<Vector2>(currVariation);
            List<List<Vector2>> list1 = Permute(index - 1, permutation1);

            // Case where the current position is included in the arrangement
            List<Vector2> permutation2 = new List<Vector2>(currVariation);
            permutation2.Add(new Vector2(index / _dynamicSideLength, index % _dynamicSideLength));
            List<List<Vector2>> list2 = Permute(index - 1, permutation2);
            
            return list1.Union(list2).ToList();
        }
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
