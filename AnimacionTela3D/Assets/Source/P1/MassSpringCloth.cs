using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class MassSpringCloth : MonoBehaviour
{
    public MassSpringCloth()
    {
        this.Paused = true;
        this.TimeStep = 0.01f;
        this.Gravity = new Vector3(0.0f, -9.81f, 0.0f);
        this.Rigidez = 100;
        this.RigidezFlexion = 10;
        this.DumpingAlpha= 0.1f;
        this.DumpingAlpha = 0.01f;
        IntegrationMethod = Integration.Symplectic;
    }

    #region InEditorVariables

    public bool Paused;
    public float TimeStep;
    public float Mass;
    public int Rigidez;
    public int RigidezFlexion;
    public float DumpingAlpha; //nodos
    public float DumpingBeta; //muelles
    public Vector3 Gravity;
    public Vector3 DirViento;
    public float FuerzaViento;
    public Integration IntegrationMethod;

    Mesh mesh;
    Vector3[] vertices;
    public List<Node> nodos = new List<Node>(); //nodos
    public List<Spring> springs = new List<Spring>(); //muelles
    public List<Fixer> fixers = new List<Fixer>(); // para fijar con objetos
    public List<Edge> Edges = new List<Edge>(); //aristas (duplicadas)
    public List<Edge> EdgesTrac = new List<Edge>(); //aristas sin duplicar y ordenadas de traccion
    public List<Edge> EdgesFlex = new List<Edge>(); //aristas de flexión
    public List<Triangles> triangulos = new List<Triangles>(); //triangulos

    #endregion

    #region OtherVariables

    public enum Integration
    {
        Explicit = 0,
        Symplectic = 1,
    };
    #endregion

    #region MonoBehaviour

    public void Start()
    {
        //Se inicializa todo
        mesh = this.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        int[] triangles = mesh.triangles;


    //Nodos--------------------------------------------------------------
        //creacion y añadir nodos a la lista nodos
        for (int j = 0; j < vertices.Length; j++)
        {
            Node nodo1 = new Node(vertices[j], Mass, DumpingAlpha); //se crea el nodo
            //se añade la posicion de cada vertice a la posicion de cada nodo
            nodos.Add(nodo1); //se añade el nodo a la lista de nodos
        }
        //fijas nodos al gameobject con el fixer
        foreach (Node nodo in nodos)
        {
            foreach (Fixer fixer in fixers)
            {
                if (fixer.isInside(nodo.pos) == true)
                {
                    nodo.isFixed = true;
                    break;
                }
            }
        }

    //Aristas--------------------------------------------------------------
        //algoritmo de busqueda de las aristas
        int w = 3;
        for (int j = 0; j < triangles.Length; j += 3)
        {
            Edges.Add(new Edge(triangles[j], triangles[j + 1], triangles[j + 2]));
            Edges.Add(new Edge(triangles[j], mesh.triangles[j + 2], triangles[j + 1]));
            Edges.Add(new Edge(triangles[j + 1], triangles[j + 2], triangles[j]));
            w += 3;
        }

        //ordenar lista
        EdgeComparer comparer = new EdgeComparer();
        Edges.Sort(comparer);

        //bucle para encontrar repetidas usando lista auxiliar EdgesTrac
        for (int j = 0; j < Edges.Count; j++)
        {
            for (w = 0; w < Edges.Count; w++)
            {
                //aristas duplicadas
                if (Edges[j].EdgeA == Edges[w].EdgeA && Edges[j].EdgeB == Edges[w].EdgeB)
                {
                    EdgesTrac.Remove(Edges[w]);
                    
                    if (!EdgesTrac.Contains(Edges[j]))
                    {
                        EdgesTrac.Add(Edges[j]);
                    }
                }
            }
        }

        //aristas de flexión
        int i = 0;
        for (int j = 0; j < EdgesTrac.Count - 1; j += 3)
        {
            if (EdgesTrac[j].EdgeA != 10 + i)
                if (EdgesTrac[j].EdgeA == EdgesTrac[j + 1].EdgeA)
                    EdgesFlex.Add(new Edge(EdgesTrac[j].EdgeB, EdgesTrac[j + 1].EdgeB, 0));
            if (EdgesTrac[j].EdgeA == 10 + i)
            {
                j -= 2;
                i += 11;
            }
        }

        //añadir muelles(springs) a la lista de springs
        //muelles traccion
        for (int j = 0; j < EdgesTrac.Count; j++)
        {
            int x = (int)EdgesTrac[j].EdgeA;
            int y = (int)EdgesTrac[j].EdgeB;
            springs.Add(new Spring(nodos[x], nodos[y], Rigidez,DumpingBeta));   
        }
        //muelles flexion
        for (int j = 0; j < EdgesFlex.Count; j++)
        {
            int x = (int)EdgesFlex[j].EdgeA;
            int y = (int)EdgesFlex[j].EdgeB;
            springs.Add(new Spring(nodos[x], nodos[y], RigidezFlexion, DumpingBeta));

        }


        for (int j = 0; j < triangles.Length; j += 3)
        {
            Triangles tri = new Triangles(nodos[triangles[j]], nodos[triangles[j + 1]], nodos[triangles[j + 2]]);
            triangulos.Add(tri);
        }


        //tranformar de coord locales a globales las posiciones de cada nodo.
        int h = 0;
        foreach (Node nodo in nodos)
        {
            //nodo.pos = transform.TransformPoint(nodo.pos);
            nodo.pos = transform.TransformPoint(vertices[h]);
           h++;
        }

    }
    public void Update() //actualizacion visual
{
    //Procedure to update vertex positions
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = new Vector3[mesh.vertexCount];

    //la posicion del nodo se le tiene que aplicar al vertice de la malla. 
    //esto se hace para que se pase la pos global al sistema de referencia local.
          int i = 0;
        foreach (Node nodo in nodos)
        {
            vertices[i] = transform.InverseTransformPoint(nodo.pos);
            i++;
        }
        
        //se actualizan los vertices del mesh.
        mesh.vertices = vertices;
     
}

    public void FixedUpdate()
    //fisicas
    //SOLVE FISICS OF A MASS SPRING MODEL (como el step simpletic)
    {
        if (this.Paused)
           return; // Not simulating

        if (Input.GetKeyUp(KeyCode.P))
            this.Paused = !this.Paused;

        for (int j = 0; j< TimeStep; j++) {
        
        // Select integration method
            switch (this.IntegrationMethod)
            {
                case Integration.Explicit: this.StepExplicit(); break;
                case Integration.Symplectic: this.StepSymplectic(); break;
                default:
                    throw new System.Exception("[ERROR] Should never happen!");
            }
        }
       
        
    }

    private void StepExplicit()
    {
        foreach (Node node in nodos)
        {
            
                node.force = Vector3.zero;
                node.ComputeForces(Gravity);
               
        }

        foreach (Spring spring in springs)
        {
            spring.ComputeForces();
        }

        foreach (Triangles tri in triangulos)
        {
            tri.ComputeForces(DirViento,FuerzaViento);
        }

        foreach (Node node in nodos)
        {
            if (!node.isFixed)
            {
                
                node.pos += TimeStep * node.vel;
                node.vel += TimeStep / node.mass * node.force;

            }
        }
        foreach (Spring spring in springs)
        {
            spring.UpdateLength();
        }

    }

    private void StepSymplectic()
    {
  
        foreach (Node node in nodos)
        {
           node.force = Vector3.zero;
           node.ComputeForces(Gravity);
           
        }

        foreach (Spring spring in springs)
        {
            spring.ComputeForces();
        }

        foreach (Triangles tri in triangulos)
        {
            tri.ComputeForces(DirViento, FuerzaViento);
        }

        foreach (Node node in nodos)
        {
            if (!node.isFixed)
            {
                node.vel += TimeStep / node.mass * node.force;
                node.pos += TimeStep * node.vel;   
            }

        }
        foreach (Spring spring in springs)
        {
            spring.UpdateLength();
        }
    }
        #endregion

}

