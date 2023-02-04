using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Biome {
	public enum BiomeType {
		Forest,
		Desert,
		Ocean
	}
	private string _groundColour;
	private BiomeType _name;
	// TODO: Add baseAssets list of MapObjects to class

	public string GroundColour {
		get { return _groundColour; }
		set { _groundColour = value; }
	}

	public BiomeType Name {
		get { return _name; }
		set { _name = value; }
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
