using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private Renderer[] _renderers;
    [SerializeField] private Material _innerGlowMaterial;
    private Material _materialInstance;

    [SerializeField] private bool _isEnabled = false;

    [SerializeField, Range(0, 10)] private float _duration;
    [SerializeField, Range(0, 10)] private float _maxOpacity;
    [SerializeField, Range(0, 10)] private float _minOpacity;

    void Start()
    {
        _renderers = this.GetComponentsInChildren<Renderer>();
        _materialInstance = new Material(_innerGlowMaterial);
        _maxOpacity = 0.2f;
    }

    public void EnableInnerGlowEffect()
    {
        StartCoroutine(InTransitionOpacity(_minOpacity, _maxOpacity, _duration));
    }

    public void DisableInnerGlowEffect()
    {
        StartCoroutine(OutTransitionOpacity(_maxOpacity, _minOpacity, _duration));
    }

    public void EnableMaterial()
    {
        if (!_isEnabled)
        {
            foreach (var renderer in _renderers)
            {
                // Append glow inner shader
                var materials = renderer.sharedMaterials.ToList();
                materials.Add(_materialInstance);
                renderer.materials = materials.ToArray();
            }
            _isEnabled = true;
        } 
    }

    public void DisableMaterial()
    {
        if (_isEnabled)
        {
            foreach (var renderer in _renderers)
            {
                // Append glow inner shader
                var materials = renderer.sharedMaterials.ToList();
                materials.Remove(_materialInstance);
                renderer.materials = materials.ToArray();
            }
            _isEnabled = false;
        }
    }

    private IEnumerator InTransitionOpacity(float startValue, float endValue, float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        _materialInstance.SetFloat("_WristOpacity", 0f);
        foreach (var renderer in _renderers)
        {
            // Append glow inner shader
            var materials = renderer.sharedMaterials.ToList();
            materials.Add(_materialInstance);
            renderer.materials = materials.ToArray();
        }

        while (Time.time < endTime)
        {
            float normalizedTime = (Time.time - startTime) / duration;
            float _alphaCurrentValue = Mathf.Lerp(startValue, endValue, normalizedTime);
            _materialInstance.SetFloat("_WristOpacity", _alphaCurrentValue);
            yield return null;
        }

        _materialInstance.SetFloat("_WristOpacity", endValue);
    }

    private IEnumerator OutTransitionOpacity(float startValue, float endValue, float duration)
    {
        float startTime = Time.time;
        float endTime = startTime + duration;

        while (Time.time < endTime)
        {
            float normalizedTime = (Time.time - startTime) / duration;
            float _alphaCurrentValue = Mathf.Lerp(startValue, endValue, normalizedTime);
            _materialInstance.SetFloat("_WristOpacity", _alphaCurrentValue);
            yield return null;
        }

        _materialInstance.SetFloat("_WristOpacity", endValue);

        foreach (var renderer in _renderers)
        {
            // Remove glow inner shader
            var materials = renderer.sharedMaterials.ToList();
            materials.Remove(_materialInstance);
            renderer.materials = materials.ToArray();
        }
    }
}
