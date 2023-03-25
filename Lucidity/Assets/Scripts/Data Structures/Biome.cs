using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Biome {
    public enum BiomeType {
        Forest
    }
    [SerializeField] private BiomeType _name;
    [SerializeField] private string _groundColour;
    // TODO: Add baseAssets list of MapObjects to class

    public BiomeType Name {
        get { return _name; }
        set { _name = value; }
    }

    public string GroundColour {
        get { return _groundColour; }
        set { _groundColour = value; }
    }

    /// <summary>
    /// Biome constructor, initializing the name and ground colour.
    /// </summary>
    /// <param name="name">
    /// Enumerated <c>BiomeType</c> corresponding to the desired biome.
    /// </param>
    public Biome (BiomeType name) {
        _name = name;
        // TODO: Update colours from placeholder values
        switch(_name) {
            case BiomeType.Forest:
                _groundColour = "5d875c";
                break;
        }
    }

    /// <summary>
    /// Biome constructor, initializing the name and ground colour.
    /// </summary>
    /// <param name="groundColour">
    /// Ground colour as a hex value <c>string</c> corresponding to the desired biome.
    /// </param>
    public Biome(string groundColour) {
        _groundColour = groundColour;
        switch(groundColour) {
            case "5d875c":
                _name = BiomeType.Forest;
                break;
        }
    }
}
