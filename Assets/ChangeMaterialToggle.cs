using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeMaterialToggle : MonoBehaviour
{
    private SkinnedMeshRenderer _renderer;
    [SerializeField] private Material _material1;
    [SerializeField] private Material _material2;
    private bool _isMaterial1Active = true;

    void Start()
    {
        _renderer = this.GetComponent<SkinnedMeshRenderer>();
        if (_renderer == null)
        {
            Debug.LogError("Il GameObject non ha un componente Renderer!");
            return;
        }
        _renderer.material = _material1;
    }

    public void ToggleMaterial()
    {
        if (_isMaterial1Active)
        {
            _renderer.material = _material2;
        }
        else
        {
            _renderer.material = _material1;
        }

        _isMaterial1Active = !_isMaterial1Active;
    }
}
