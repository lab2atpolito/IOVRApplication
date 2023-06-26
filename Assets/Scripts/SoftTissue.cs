using UnityEngine;

public class SoftTissue : MonoBehaviour
{
    [SerializeField]
    private float bounceSpeed = 60f;
    [SerializeField]
    private float fallForce = 90f;
    [SerializeField]
    private float stiffness = 0.1f;

    private MeshFilter _meshFilter;
    private Mesh _mesh;

    SoftVertex[] softVertices;
    Vector3[] currentMeshVertices;

    // Start is called before the first frame update
    void Start()
    {
        _meshFilter = this.GetComponent<MeshFilter>();
        _mesh = _meshFilter.mesh;

        GetVertices();
    }

    private void GetVertices()
    {
        softVertices = new SoftVertex[_mesh.vertices.Length];
        currentMeshVertices = new Vector3[_mesh.vertices.Length];
        for(int i = 0; i < _mesh.vertices.Length; i++)
        {
            softVertices[i] = new SoftVertex(i, _mesh.vertices[i], _mesh.vertices[i], Vector3.zero);
            currentMeshVertices[i] = _mesh.vertices[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVertices();
    }

    private void UpdateVertices()
    {
        for(int i = 0; i < softVertices.Length; i++)
        {
            softVertices[i].UpdateVelocity(bounceSpeed);
            softVertices[i].Settle(stiffness);

            softVertices[i].currentVertexPosition += softVertices[i].currentVelocity * Time.deltaTime;
            currentMeshVertices[i] = softVertices[i].currentVertexPosition;
        }

        _mesh.vertices = currentMeshVertices;
        _mesh.RecalculateBounds();
        _mesh.RecalculateNormals();
        _mesh.RecalculateTangents();
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("COLLISION!");
        ContactPoint[] collisionPoints = collision.contacts;
        foreach(ContactPoint contactPoint in collisionPoints)
        {
            Debug.Log(contactPoint.point.ToString());
            Vector3 inputPoint = contactPoint.point + (contactPoint.point * .1f);
            ApplyPressureToPoint(inputPoint, fallForce);
        }
    }

    private void ApplyPressureToPoint(Vector3 _point, float _pressure)
    {
        foreach (SoftVertex vertex in softVertices)
        {
            vertex.ApplyPressureToVertex(transform, _point, _pressure);
        }
    }
}
