using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfacePainting : MonoBehaviour
{
    [SerializeField] private Texture2D _brush;
    [SerializeField] private Texture2D _maskBase;

    [SerializeField] private Material _material;

    private Texture2D _templateMask;

    [SerializeField] private Camera _mainCamera; 

    // Start is called before the first frame update
    private void Start()
    {
        CreateTexture();
    }

    private void CreateTexture()
    {
        _templateMask = new Texture2D(_maskBase.width, _maskBase.height);
        _templateMask.SetPixels(_maskBase.GetPixels());
        _templateMask.Apply();

        _material.SetTexture("MaskTexture", _templateMask);
    }

    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            if(Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit hit))
            {
                Vector2 textureCoord = hit.textureCoord;

                int pixelX = (int)(textureCoord.x * _templateMask.width);
                int pixelY = (int)(textureCoord.y * _templateMask.height);

                for(int i=0; i < _brush.width; i++)
                {
                    for(int j=0; j < _brush.height; j++)
                    {
                        Color pixelBrush = _brush.GetPixel(i, j);
                        Color pixelMask = _templateMask.GetPixel( pixelX + i, pixelY + j);

                        _templateMask.SetPixel(pixelX + i, pixelY + j, new Color(0, pixelMask.g * pixelBrush.g, 0));
                    }
                }

                _templateMask.Apply();
            }
        }
    }
}
