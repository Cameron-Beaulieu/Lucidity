using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RaycastingLibrary;

namespace RaycastingLibrary {
  
  public class RayLibrary {

    /// <summary>
    /// Returns 'true' if we touched or hovering on Unity UI element.
    /// </summary>
    public bool IsPointerOverLayer(int checkedLayer) {
      return IsPointerOverLayer(GetEventSystemRaycastResults(), checkedLayer);
    }
 
    
    /// <summary>
    /// Returns 'true' if we touched or hovering on Unity UI element.
    /// </summary>
    public bool IsPointerOverLayer(List<RaycastResult> eventSystemRaysastResults, int checkedLayer) {
      for (int index = 0; index < eventSystemRaysastResults.Count; index++) {
        RaycastResult curRaysastResult = eventSystemRaysastResults[index];
        if (curRaysastResult.gameObject.layer == checkedLayer){
          return true;
        }
      }
      return false;
    }
 
    /// <summary>
    /// Gets all event system raycast results of current mouse or touch position.
    /// </summary>
    public List<RaycastResult> GetEventSystemRaycastResults() {
      PointerEventData eventData = new PointerEventData(EventSystem.current);
      eventData.position = Input.mousePosition;
      List<RaycastResult> raysastResults = new List<RaycastResult>();
      EventSystem.current.RaycastAll(eventData, raysastResults);
      return raysastResults;
    }
  }
}