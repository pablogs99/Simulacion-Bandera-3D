using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node
{
    public Vector3 pos;
    public Vector3 vel;
    public Vector3 force;
    public float mass;
 

    //fuerza de amortiguamiento
    public float dumping;

    //para detectar si los nodos estan fijos
    public bool isFixed=false;

    
    //// Use this for initialization
    public Node(Vector3 posicion, float Mass, float dumpingA)
    {
       this.pos = posicion;
       this.mass = Mass;

       //se inicializa el amortiguamiento
       this.dumping = dumpingA * this.mass;
    }

    public void ComputeForces(Vector3 Gravedad)
    {
        force = mass* Gravedad -dumping * vel;
    }
}
