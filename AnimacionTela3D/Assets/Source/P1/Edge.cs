using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge 
{
    public float EdgeA;
    public float EdgeB;
    public float EdgeOther;

    public Edge(float edgeA, float edgeB, float Other) 
    {
        if (edgeA < edgeB)
        {
            this.EdgeA = edgeA;
            this.EdgeB = edgeB;
           this.EdgeOther = Other;
        }
        else if (edgeB < edgeA)
        {
            this.EdgeA = edgeB;
            this.EdgeB = edgeA;
            this.EdgeOther = Other;
        }
    }
}

  
