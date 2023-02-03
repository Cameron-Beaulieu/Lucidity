using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome {
	public enum BiomeType
	{
		Forest,
		Desert,
		Ocean
	}
	private BiomeType _name;
	private string _groundColour;
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
}
