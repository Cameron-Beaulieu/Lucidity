using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorManager : MonoBehaviour {
    public List<AssetController> AssetButtons;
    public List<GameObject> AssetPrefabs;
    public int CurrentButtonPressed;

    private void Update() {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);

        if (Input.GetMouseButtonDown(0)
                && AssetButtons[CurrentButtonPressed].Clicked) {
            Instantiate(AssetPrefabs[CurrentButtonPressed],
                        new Vector3(worldPosition.x, worldPosition.y, 0),
                        Quaternion.identity);
        }
    }
}
