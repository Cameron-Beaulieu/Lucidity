using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RaycastingLibrary {
	
	public class RayLibrary {

		/// <summary>
		/// Retrieves and returns all event system raycast results of current mouse or touch.
		/// position.
		/// </summary>
		/// <returns>
		/// List of <c>RaycastResult</c> corresponding to current mouse or touch.
		/// </returns>
		public List<RaycastResult> GetEventSystemRaycastResults() {
			PointerEventData eventData = new PointerEventData(EventSystem.current);
			eventData.position = Input.mousePosition;
			List<RaycastResult> raysastResults = new List<RaycastResult>();
			EventSystem.current.RaycastAll(eventData, raysastResults);
			return raysastResults;
		}

		/// <summary>
		/// Invoke this overloaded method to check if raycast is on an element of the layer
		/// corresponding to <c>checkedLayer</c>.
		/// </summary>
		/// <param name="checkedLayer">
		/// <c>int</c> corresponding to the layer number that was checked.
		/// </param>
		/// <returns>
		/// <c>true</c> if touching/hovering over a Unity UI element; <c>false</c> otherwise.
		/// </returns>
		public bool IsPointerOverLayer(int checkedLayer) {
			return IsPointerOverLayer(GetEventSystemRaycastResults(), checkedLayer);
		}

		/// <summary>
		/// Checks if given raycasts are on an element of the layer corresponding to
		/// <c>checkedLayer</c>.
		/// </summary>
		/// <param name="eventSystemRaycastResults">
		/// List of <c>RaycastResult</c> to check against given layer.
		/// </param>
		/// <param name="checkedLayer">
		/// <c>int</c> corresponding to the layer number that was checked.
		/// </param>
		/// <returns>
		/// <c>true</c> if touching/hovering over a Unity UI element; <c>false</c> otherwise.
		/// </returns>
		public bool IsPointerOverLayer(List<RaycastResult> eventSystemRaycastResults,
									   int checkedLayer) {
			for (int index = 0; index < eventSystemRaycastResults.Count; index++) {
				RaycastResult currentRaysastResult = eventSystemRaycastResults[index];
				if (currentRaysastResult.gameObject.layer == checkedLayer) {
					return true;
				}
			}
			return false;
		}
	}
}
