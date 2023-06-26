using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Sticker : MonoBehaviour
{
    [SerializeField] private bool _isAttached;
    [SerializeField] private bool _isPhysicOn;

    [SerializeField] private Rigidbody[] _bones;
    [SerializeField] private Collider[] _colliders;

    [SerializeField] private float _maxDistance = 0.02f;

    void Start()
    {
        _isAttached = true;
    }

    void Update()
    {
        if( !_isPhysicOn)
        {
            Transform stabilizer = this.transform.parent;
            float distance = Vector3.Distance(stabilizer.position, this.transform.position);
            if (distance > _maxDistance)
                _isAttached = false;
        } 
    }

    public void DisableGravity()
    {
        foreach( Rigidbody bone in _bones)
        {
            bone.useGravity = false;
        }
        foreach( Collider collider in _colliders)
        {
            collider.enabled = false;
        }
        _isPhysicOn = false;
    }

    public void EnableGravity()
    {
        foreach (Rigidbody bone in _bones)
        {
            bone.useGravity = true;
        }
        foreach (Collider collider in _colliders)
        {
            collider.enabled = true;
        }
        _isPhysicOn = true;
    }

    public bool IsAttached()
    {
        return _isAttached;
    }

    public bool IsPhysicOn()
    {
        return _isPhysicOn;
    }

    public void Grab()
    {
        
    }

    public void Release()
    {
        if( !_isAttached )
        {
            this.transform.parent = null;
            this.transform.DOScale(0f, 1f)
                .OnComplete(() => this.DestroySticker())
                .SetEase(Ease.Linear);
        }
    }

    public void DisableGrabbable()
    {
        this.GetComponent<Collider>().enabled = false;
    }

    private void DestroySticker()
    {
        foreach(Transform child in transform)
        {
            Destroy(child);
        }
        Destroy(transform);
    }

    public void EnableGrabbable()
    {
        this.GetComponent<Collider>().enabled = true;
    }
}
