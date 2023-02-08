using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicBoundingBox : MonoBehaviour {
    private static int _dynamicSideLength;  // Side length of the bounding box in number of assets

    public static int DynamicSideLength {
        get { return _dynamicSideLength; }
        set { _dynamicSideLength = value; }
    }
}
