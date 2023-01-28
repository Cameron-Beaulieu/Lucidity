using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartupScreen : MonoBehaviour
{
    [SerializeField] private Button newMapBtn;
    [SerializeField] private Button loadMapBtn;
    // Start is called before the first frame update
    private void Start() {
        newMapBtn.onClick.AddListener(NewMapClickHandler);
        loadMapBtn.onClick.AddListener(LoadMapClickHandler);
    }
    
    public void NewMapClickHandler() {
        Debug.Log("Create new map button clicked");
    }

    public void LoadMapClickHandler() {
        Debug.Log("Load map button clicked");
    }
}
