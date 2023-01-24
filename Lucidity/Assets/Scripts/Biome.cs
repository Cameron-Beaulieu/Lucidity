using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BiomeType {Forest, Desert, Ocean}

public class Biome {

  // TODO: Add baseAssets array of MapObjects to class
  private BiomeType _name;
  private string _groundColour;

  public Biome(BiomeType name) {
    _name = name;
    
    // TODO: Update colours from placeholder values
    switch(_name) {
      case BiomeType.Forest:
        _groundColour = "495c2e";
        break;
      case BiomeType.Desert:
        _groundColour = "b38f72";
        break;
      case BiomeType.Ocean:
        _groundColour = "66a6d1";
        break;
    }
  }

  public BiomeType getName() {
    return _name;
  }

  public string getGroundColour() {
    return _groundColour;
  }
}
