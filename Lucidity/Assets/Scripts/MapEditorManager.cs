using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapEditorManager : MonoBehaviour {
    public List<AssetController> AssetButtons;
    public List<GameObject> AssetPrefabs;
    public List<GameObject> AssetImage;
    public int CurrentButtonPressed;
    public InputField CountInput;
    public int Count;

    private void Start() {
        Count = 1;
    }

    private void Update() {
        Vector2 worldPosition = getMousePosition();

        if (Input.GetMouseButtonDown(0)
                && AssetButtons[CurrentButtonPressed].Clicked) {
            for (int i = 0; i < Count; i++) {
                Instantiate(AssetPrefabs[CurrentButtonPressed],
                            new Vector3(worldPosition.x + i*2, worldPosition.y, 0),
                            Quaternion.identity);
            }
        }
    }

    public static Vector2 getMousePosition() {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        return Camera.main.ScreenToWorldPoint(screenPosition);
    }

    public void ReadCountInput(string s) {
        Count = int.Parse(s);
        // Restrict input to only be positive
        if (Count < 0) {
            Count *= -1;
            CountInput.text = "" + Count;
        }
    }
}
