using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinnedMeshColliderUpdate : MonoBehaviour
{
    SkinnedMeshRenderer _skinnedMeshRenderer;
    MeshCollider _meshCollider;

    private float time; 

    void Start()
    {
        _skinnedMeshRenderer = this.GetComponent<SkinnedMeshRenderer>();
        _meshCollider = this.GetComponent<MeshCollider>();
        time = 0f;
        UpdateCollider();
    }

    void Update()
    {
        time += Time.deltaTime;
        if(time > 1f) //Mesh Collider Updates every 2sec
        {
            time = 0f;
            //UpdateCollider();
        }
        
    }

    private void UpdateCollider()
    {
        Mesh colliderMesh = new Mesh();
        colliderMesh.name = "Body_mesh_updated";
        _skinnedMeshRenderer.BakeMesh(colliderMesh);
        _meshCollider.sharedMesh = null;
        _meshCollider.sharedMesh = colliderMesh;
    }
}
