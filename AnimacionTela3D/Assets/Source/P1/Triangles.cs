using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Triangles
{
   public Node nodeA, nodeB, nodeC;

    public float VertexA;
    public float VertexB;
    public float VertexC;

    public Vector3 force;

    public float Semiperimetro;
    public float area;
    public Vector3 vel;

    public Vector3 n;

 //// Use this for initialization
public Triangles(Node A, Node B, Node C)
 {
        nodeA = A;
        nodeB = B;
        nodeC = C;

    this.VertexA = (nodeA.pos - nodeB.pos).magnitude;
    this.VertexB = (nodeB.pos - nodeC.pos).magnitude;
    this.VertexC = (nodeC.pos - nodeA.pos).magnitude;

    this.vel=(nodeA.vel+nodeB.vel+nodeC.vel)/3;
    this.Semiperimetro = (VertexA + VertexB + VertexC) / 2;

    area = (float)Math.Sqrt(Semiperimetro * (Semiperimetro - VertexA) * (Semiperimetro - VertexB) * (Semiperimetro - VertexC));
       
 }

public void ComputeForces(Vector3 Dirviento, float K)
  {
        n = Vector3.Cross(nodeB.pos - nodeA.pos, nodeC.pos - nodeA.pos);

        force = K*area*(Vector3.Dot(n,(Dirviento - vel)))*n;
        nodeA.force += force / 3;
        nodeB.force += force / 3;
        nodeC.force += force / 3;

       
    }

 


}
