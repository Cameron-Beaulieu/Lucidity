using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartupScreen : MonoBehaviour {
	[SerializeField] private Button newMapButton;
	[SerializeField] private Button loadMapButton;

	private void Start() {
		newMapButton.onClick.AddListener(NewMapClickHandler);
		loadMapButton.onClick.AddListener(LoadMapClickHandler);
	}
	
	/// <summary>
	/// Button handler for <c>newMapButton</c>, selected through in the Unity editor.
	/// </summary>
	public void NewMapClickHandler() {
		Debug.Log("Create new map button clicked");
	}

	/// <summary>
	/// Button handler for <c>loadMapButton</c>, selected through in the Unity editor.
	/// </summary>
	public void LoadMapClickHandler() {
		Debug.Log("Load map button clicked");
	}
}
