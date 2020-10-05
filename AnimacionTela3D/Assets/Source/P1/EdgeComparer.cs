using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeComparer : IComparer<Edge>
{
    public int Compare(Edge a, Edge b)
    {
        if (a.EdgeA > b.EdgeA)
        {
            return 1;
        }
        else if (a.EdgeA < b.EdgeA)
        {
            return -1;
        }
        else if (a.EdgeA == b.EdgeA)
        {
            if (a.EdgeB > b.EdgeB)
            {
                return 1;
            }
            else if (a.EdgeB < b.EdgeB)
            {
                return -1;
            }
            
        else 
        {
         return 0;
        }

        }
        return -1;
    }
}
