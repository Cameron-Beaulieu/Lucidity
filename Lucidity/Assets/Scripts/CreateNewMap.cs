using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class CreateNewMap : MonoBehaviour
{
    [SerializeField] private InputField mapName;
    [SerializeField] private Dropdown mapSizeDropdown;
    [SerializeField] private Dropdown biomeDropdown;
    [SerializeField] private Button createBtn;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private Toggle startingAssetsToggle;
    private Text _errorMessage;
    public static string mapSize;

    // Start is called before the first frame update
    private void Start() {
        createBtn.onClick.AddListener(CreateMapClickHandler);
        cancelBtn.onClick.AddListener(CancelMapClickHandler);
        _errorMessage = GameObject.Find("ErrorMessage").GetComponent<Text>();
    }

    public string getMapSize() {
        switch(mapSizeDropdown.value) {
        case 0:
            return "Small";
        case 1:
            return "Medium";
        case 2:
            return "Large";
        default:
            return "Medium";
        }
    }

    public Biome getBiome() {
        switch(biomeDropdown.value) {
        case 0:
            return new Biome(BiomeType.Forest);
        case 1:
            return new Biome(BiomeType.Desert);
        case 2:
            return new Biome(BiomeType.Ocean);
        default:
            return new Biome(BiomeType.Forest);
        }
    }


    public void CreateMapClickHandler() {
        if (String.IsNullOrWhiteSpace(mapName.text)) {
            _errorMessage.text = "You must provide a file name to create a map";
            mapName.Select();
            return;
        }
        
        if (CreateFile()) {
            SceneManager.LoadScene("MapEditor", LoadSceneMode.Single);
        }
    }

    /// <summary>
    /// Creates a json file at a location specified by the user. Returns true if the file creation
    /// is successful. Otherwise an error message is displayed on the UI and false is returned.
    /// </summary>
    private bool CreateFile() {
        string directory = EditorUtility.OpenFolderPanel("Select Directory", "", "");
        // cancelled selecting a directory
        if (directory.Equals("")) { return false; }

        string fileName = mapName.text;
        
        fileName = directory + "/" + fileName + ".json";

        if (!File.Exists(fileName)) {
            MapData jsonContent = new MapData(getMapSize(), getBiome());
            File.WriteAllText(fileName, jsonContent.Serialize());
            return true;
        }
        
        _errorMessage.text = "There is already a map with that name in the chosen directory";
        mapName.Select();
        return false;
    }

    public void CancelMapClickHandler() {
        Debug.Log("Cancel button clicked");
    }
}
