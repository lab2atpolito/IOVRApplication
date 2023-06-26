using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftTissueSimulator : MonoBehaviour
{
    [SerializeField, Range(0,1)]
    private float _damping;

    [SerializeField]
    private float _nodeMass;

    [SerializeField, Range(0,1)]
    public float _sniffness;

    private List<GameObject> masses = new List<GameObject>();
    private List<SpringJoint> springs = new List<SpringJoint>();

    private Dictionary<int, HashSet<int>> connectedVertices;

    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;

        //Nodes creation
        for( int i=0; i<mesh.vertices.Length; i++)
        {
            GameObject node = new GameObject("Node_" + i);
            node.transform.position = transform.TransformPoint(mesh.vertices[i]);
            node.AddComponent<Rigidbody>();
            node.GetComponent<Rigidbody>().mass = _nodeMass;
            node.GetComponent<Rigidbody>().isKinematic = true;
            node.AddComponent<SphereCollider>();
            node.GetComponent<SphereCollider>().radius = 0.05f;
            masses.Add(node);
            node.transform.parent = this.transform;
        }

        // Initialize the dictionary of connected vertices
        connectedVertices = new Dictionary<int, HashSet<int>>();

        // Loop through all the edges in the mesh
        int[] triangles = mesh.triangles;
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Get the indices of the vertices for this triangle
            int vertexIndexA = triangles[i];
            int vertexIndexB = triangles[i + 1];
            int vertexIndexC = triangles[i + 2];

            // Check if a spring joint already exists between vertex A and B
            if (!HasSpringJoint(vertexIndexA, vertexIndexB))
            {
                // If not, create a new spring joint
                SpringJoint springJoint = masses[vertexIndexA].AddComponent<SpringJoint>();
                springJoint.connectedBody = masses[vertexIndexB].GetComponent<Rigidbody>();
                springJoint.spring = _sniffness;
                springJoint.damper = _damping;
                springJoint.autoConfigureConnectedAnchor = false;
                springJoint.anchor = masses[vertexIndexA].transform.position;
                springJoint.connectedAnchor = masses[vertexIndexB].transform.position;
                Debug.DrawLine(masses[vertexIndexA].transform.position, masses[vertexIndexB].transform.position, Color.red);


                // Add vertex B to the list of connected vertices for vertex A
                if (!connectedVertices.ContainsKey(vertexIndexA))
                {
                    connectedVertices[vertexIndexA] = new HashSet<int>();
                }
                connectedVertices[vertexIndexA].Add(vertexIndexB);

                // Add vertex A to the list of connected vertices for vertex B
                if (!connectedVertices.ContainsKey(vertexIndexB))
                {
                    connectedVertices[vertexIndexB] = new HashSet<int>();
                }
                connectedVertices[vertexIndexB].Add(vertexIndexA);
            }
        }
    }

    private bool HasSpringJoint(int vertexIndexA, int vertexIndexB)
    {
        if (connectedVertices.ContainsKey(vertexIndexA) && connectedVertices[vertexIndexA].Contains(vertexIndexB))
        {
            return true;
        }
        if (connectedVertices.ContainsKey(vertexIndexB) && connectedVertices[vertexIndexB].Contains(vertexIndexA))
        {
            return true;
        }
        return false;
    }

    void FixedUpdate()
    {
        // Aggiornamento della posizione e della velocità dei nodi
        /*for (int i = 0; i < nodes.Count; i++)
        {
            Rigidbody node = nodes[i];
            Vector3 force = Vector3.zero;
            for (int j = 0; j < springs.Count; j++)
            {
                SpringJoint spring = springs[j];
                if (spring.connectedBody == node)
                {
                    force += spring.currentForce;
                }
                else if (spring.connectedBody != null && spring.connectedBody != node)
                {
                    force -= spring.currentForce;
                }
            }
            node.AddForce(force);
        }*/
    }
}
