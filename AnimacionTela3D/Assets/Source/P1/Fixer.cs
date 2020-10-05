using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fixer : MonoBehaviour
{
    // Start is called before the first frame update
    // Possibilities of the Fixer
    public int size;
    public bool isInside (Vector3 pos)
    {
        Bounds bounds = GetComponent<Collider>().bounds;
        bool isInside = bounds.Contains(pos);
        return isInside;

      
    }
}

 
