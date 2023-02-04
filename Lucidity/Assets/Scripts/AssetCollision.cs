using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AssetCollision : MonoBehaviour {
	[SerializeField] private Material _errorMaterial;
	// Use this to ensure that the Gizmos are being drawn when in Play Mode
	private bool _detectionStarted = true;
	private LayerMask _filterMask;
	private int _uiLayer = 5;
	private int _assetLayer = 6;
	private Material _originalMaterial;

	void Start() {
		_filterMask = LayerMask.GetMask("Asset");
		CheckAssetOnUI();
		CheckAssetCollisions();
	}

	void OnDrawGizmos() {
		Gizmos.color = Color.red;
		if (_detectionStarted) {
			// Draw a cube where the OverlapBox is (positioned where the GameObject and with
			// identical size).
			Gizmos.DrawWireCube(transform.position, transform.localScale);
		}
	}

	/// <summary>
	/// Checks for all collisions during placement of a new map object and handles them.
	/// Handling involves turning the map object's material to an error material and
	/// calling a corountine to revert the materials and destroy the map object after an
	/// appropriate amount of time.
	/// </summary>
	void CheckAssetCollisions() {
		Collider[] hitColliders = Physics.OverlapBox(gameObject.transform.position,
			transform.localScale / 2, Quaternion.identity, _filterMask);
		int collisions = hitColliders.Length;
		foreach (Collider collisionObject in hitColliders) {
			// If an object is labeled with the "CollisionObject" tag, then it can be considered as
			// not colliding, as it will not be placed because of legality.
			if (collisionObject.tag == "CollisionObject") {
				collisions--;
			}
		}
		if (collisions > 1) {
			gameObject.tag = "CollisionObject";
			foreach (Collider collisionObject in hitColliders) {
				if (collisionObject.gameObject.layer == _assetLayer
						&& collisionObject.gameObject.GetComponent<MeshRenderer>() != null) {
					_originalMaterial = collisionObject.gameObject.GetComponent<MeshRenderer>()
											.material;
					collisionObject.gameObject.GetComponent<MeshRenderer>()
						.material = _errorMaterial;
					StartCoroutine(RevertMaterialAndDestroy(
						_originalMaterial,
						collisionObject.gameObject));
				}
			}
			MapEditorManager.LastEncounteredObject = hitColliders[0].gameObject;
		}
	}

	/// <summary>
	/// Changes a map object's material back to its original material from <c>_errorMaterial</c>
	/// and destroys the placed asset causing the collision. This is required during collision
	/// handling.
	/// </summary>
	/// <param name="_originalMaterial">
	/// The <c>Material</c> corresponding to the collision <c>GameObject</c>.
	/// </param>
	/// <param name="collisionObject">
	/// The <c>GameObject</c> that is experiencing collision, to be highlighted briefly.
	/// </param>
	IEnumerator RevertMaterialAndDestroy(Material _originalMaterial, GameObject collisionObject) {
		yield return new WaitForSecondsRealtime(0.5f);
		collisionObject.gameObject.GetComponent<MeshRenderer>().material = _originalMaterial;
		if (collisionObject == gameObject)
		{
			Destroy(gameObject);
		}
	}

	/// <summary>
	/// Checks asset placement and ensures that assets cannot be placed over UI elements.
	/// </summary>
	void CheckAssetOnUI() {
		RaycastingLibrary.RayLibrary rayLib = new RaycastingLibrary.RayLibrary();
		if (rayLib.IsPointerOverLayer(_uiLayer)) {
			Destroy(gameObject);
		}
	}
}