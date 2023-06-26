using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftTissueDeformer : MonoBehaviour
{
    public MeshFilter meshFilter;

    public ComputeShader computeShader;

    public float mass = 0.05f;
    public float springK = 100.0f;
    public float damping = 0.95f;

    public int numInteractions = 10;

    public float timeStep = 0.01f;

    //Bufferr for the vertex data
    private ComputeBuffer _vertexBuffer;
    private ComputeBuffer _velocityBuffer;

    //Buffers for the spring data
    private ComputeBuffer _springBuffer;
    private ComputeBuffer _restLengthBuffer;
    private ComputeBuffer _springStrengthBuffer;

    private void Start()
    {
        Vector3[] vertices = meshFilter.mesh.vertices;

        _vertexBuffer = new ComputeBuffer(vertices.Length, sizeof(float) * 3);
        _vertexBuffer.SetData(vertices);
        _velocityBuffer = new ComputeBuffer(vertices.Length, sizeof(float) * 3);

        int[] indices = meshFilter.mesh.triangles;
        Vector3[] normals = meshFilter.mesh.normals;
        int numSprings = indices.Length / 3;
        int[] springIndices = new int[numSprings];
        float[] restLengths = new float[numSprings];
        float[] springStrength = new float[numSprings];
        for (int i = 0; i < numSprings; i++)
        {
            int index1 = indices[i * 3];
            int index2 = indices[i * 3 + 1];
        }

    }
}
