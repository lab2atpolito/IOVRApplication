using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexturePaintManager : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Shader _paintShader;

    [SerializeField] private RenderTexture _mask;
    [SerializeField] private Material _currentMaterial, _paintMaterial;
    private RaycastHit _hit;

    [SerializeField, Range(1,500)] private float _size;
    [SerializeField, Range(0,1)] private float _strength;

    void Start()
    {
        _paintMaterial = new Material(_paintShader); // Shader Script Material
        _paintMaterial.SetVector("_Color", Color.red);

        _currentMaterial = GetComponent<MeshRenderer>().material; //ShaderGraph Material

        _mask = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat);
        _currentMaterial.SetTexture("_Mask", _mask);
        
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out _hit))
            {
                if (_hit.collider.gameObject.tag == "PaintableObject")
                {
                    _paintMaterial.SetVector("_Coordinates", new Vector4(_hit.textureCoord.x, _hit.textureCoord.y, 0, 0));
                    _paintMaterial.SetFloat("_Strength", _strength);
                    _paintMaterial.SetFloat("_Size", _size);
                    RenderTexture temp = RenderTexture.GetTemporary(_mask.width, _mask.height, 0, RenderTextureFormat.ARGBFloat);
                    Graphics.Blit(_mask, temp);
                    Graphics.Blit(temp, _mask, _paintMaterial);
                    RenderTexture.ReleaseTemporary(temp);
                    Debug.Log("Collision");
                }
            }
        }
        
    }
}
