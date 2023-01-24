using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateNewMap : MonoBehaviour
{
  [SerializeField] private InputField mapName;
  [SerializeField] private Dropdown mapSizeDropdown;
  [SerializeField] private Dropdown biomeDropdown;
  [SerializeField] private Button createBtn;
  [SerializeField] private Button cancelBtn;
  [SerializeField] private Toggle startingAssetsToggle;

  // Start is called before the first frame update
  private void Start() {
    createBtn.onClick.AddListener(CreateMapClickHandler);
    cancelBtn.onClick.AddListener(CancelMapClickHandler);
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
    Debug.Log("Create button clicked");
    Debug.Log("Map name: " + mapName.text);
    Debug.Log("Map size: " + getMapSize());
    Debug.Log("Biome: " + getBiome().getName());
    Debug.Log("Start with assets: " + startingAssetsToggle.isOn);
  }

  public void CancelMapClickHandler() {
    Debug.Log("Cancel button clicked");
  }
}
