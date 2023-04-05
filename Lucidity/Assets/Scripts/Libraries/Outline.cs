using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Outline : MonoBehaviour {
    private MapEditorManager _editor;
    private Vector3 _outlineScale;

    private void Awake() {
        _editor = GameObject.FindGameObjectWithTag("MapEditorManager")
            .GetComponent<MapEditorManager>();
        _outlineScale = _editor.OutlinePrefab.transform.localScale;
        CreateOutline();
    }

    /// <summary>
    /// Perform element-wise division of two vectors (i.e., dividing the x component of both
    /// vectors, dividing the y component of both vectors, and dividing the z component of both
    /// vectors).
    /// </summary>
    /// <param name="v1">
    /// <c>Vector3</c> with components as dividend
    /// </param>
    /// <param name="v2">
    /// <c>Vector3</c> with components as divisors
    /// </param>
    /// <returns>
    /// <c>Vector3</c> where each component is a quotient of the element-wise division
    /// </returns>
    private Vector3 ElementWiseDivision(Vector3 v1, Vector3 v2) {
        Vector3 result = new Vector3(v1.x / v2.x, v1.y / v2.y, v1.z / v2.z);
        return result;
    }

    /// <summary>
    /// Creates a rectangular outline around the current <c>GameObject</c>. Uses the selected
    /// outline prefab.
    /// </summary>
    private void CreateOutline() {
        GameObject left = CreateSideOutline(
            gameObject.transform,
            new Vector3(0, _outlineScale.x + gameObject.GetComponent<SpriteRenderer>().size.y, 0),
            new Vector3(-(gameObject.GetComponent<SpriteRenderer>().size.x + _outlineScale.x) / 2,
                        0, 0));

        GameObject right = CreateSideOutline(
            gameObject.transform,
            new Vector3(0, _outlineScale.x + gameObject.GetComponent<SpriteRenderer>().size.y, 0),
            new Vector3((gameObject.GetComponent<SpriteRenderer>().size.x + _outlineScale.x) / 2,
                        0, 0));

        GameObject top = CreateSideOutline(
            gameObject.transform,
            new Vector3(_outlineScale.y + gameObject.GetComponent<SpriteRenderer>().size.x, 0, 0),
            new Vector3(0,
                        (gameObject.GetComponent<SpriteRenderer>().size.y + _outlineScale.y) / 2,
                        0));

        GameObject bottom = CreateSideOutline(
            gameObject.transform,
            new Vector3(_outlineScale.y + gameObject.GetComponent<SpriteRenderer>().size.x, 0, 0),
            new Vector3(0,
                        -(gameObject.GetComponent<SpriteRenderer>().size.y + _outlineScale.y) / 2,
                        0));
        }

    /// <summary>
    /// Scales the outline prefab to be one of the sides for an outline.
    /// </summary>
    /// <param name="parentTransform">
    /// <c>Transform</c> corresponding to the parent <c>GameObject</c>
    /// </param>
    /// <param name="scaleOffset">
    /// <c>Vector3</c> corresponding to the desired scale offset of the outline
    /// </param>
    /// <param name="position">
    /// <c>Vector3</c> corresponding to the desired (local) position of the outline
    /// </param>
    /// <returns>
    /// <c>GameObject</c> corresponding to the side outline created
    /// </returns>
    private GameObject CreateSideOutline(Transform parentTransform, Vector3 scaleOffset,
                                         Vector3 position) {
        GameObject outline = Instantiate(_editor.OutlinePrefab, parentTransform, false);
        // outline.transform.localScale = ElementWiseDivision(outline.transform.localScale,
        //                                                    new Vector3(parentTransform.localScale.x, 1, 1));
        outline.transform.localScale += scaleOffset;
        outline.transform.SetLocalPositionAndRotation(position, Quaternion.identity);
        return outline;
    }
}
