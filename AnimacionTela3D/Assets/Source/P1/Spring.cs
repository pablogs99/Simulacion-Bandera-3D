using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spring

{

    public Node nodeA, nodeB;

    public float Length0;
    public float Length;

    public float rigidez;

    public Vector3 Pos;
    public float dumping;


    public Spring(Node node1, Node node2, float Rigidez, float dumpingB)
    {
        nodeA = node1;
        nodeB = node2;
        rigidez = Rigidez;
        Length0 = Length = (nodeA.pos - nodeB.pos).magnitude;
        this.dumping = dumpingB * this.rigidez;
        
    }

    

    public void UpdateLength()
    {
        Length = (nodeA.pos - nodeB.pos).magnitude;
    }

    public void ComputeForces()
    {
        Vector3 u = nodeA.pos - nodeB.pos;
        u.Normalize();
        Vector3 force = -rigidez * (Length - Length0) * u - dumping * Vector3.Dot(u, (nodeA.vel - nodeB.vel)) * u;
        nodeA.force += force;
        nodeB.force -= force;
    }
    
}
