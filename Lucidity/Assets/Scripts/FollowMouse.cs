using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using RaycastingLibrary;

public class FollowMouse : MonoBehaviour
{

    private int UILayer = 5;

    // Update is called once per frame
    void Update()
    {
        Vector2 worldPosition = MapEditorManager.getMousePosition();
        transform.position = new Vector3(worldPosition.x, worldPosition.y, 90f);
        if (IsPointerOverLayer(UILayer)){
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else if (!IsPointerOverLayer(UILayer)) {
            gameObject.GetComponent<MeshRenderer>().enabled = true;
        }
    }

/*
    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverLayer(int checkedLayer)
    {
        return IsPointerOverLayer(GetEventSystemRaycastResults(), checkedLayer);
    }
 
 
    //Returns 'true' if we touched or hovering on Unity UI element.
    public bool IsPointerOverLayer(List<RaycastResult> eventSystemRaysastResults, int checkedLayer)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == checkedLayer)
                return true;
        }
        return false;
    }
 
 
    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
*/
    
}
