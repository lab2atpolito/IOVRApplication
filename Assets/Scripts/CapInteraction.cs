using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;
using Oculus.Interaction.HandGrab;
using DG.Tweening;

public class CapInteraction : MonoBehaviour
{
    private Needle _needle;

    [SerializeField] private HandGrabInteractable _handGrabL;
    [SerializeField] private HandGrabInteractable _handGrabR;

    void Start()
    {
        _needle = this.gameObject.GetComponentInParent<Needle>();
    }

    void Update()
    {
        if(_needle.GetState() == NeedleState.SNAPPED)
        {
            _handGrabL.Enable();
            _handGrabR.Enable();
        }
        else
        {
            _handGrabL.Disable();
            _handGrabR.Disable();
        }
    }

    public void ClearParent()
    {
        this.transform.parent = null;
    }

    public void Grab()
    {
        ClearParent();
    }

    public void Release()
    {
        ClearParent();
        this.transform.DOScale(0f, 1f)
                .OnComplete(() => Destroy(this))
                .SetEase(Ease.Linear);
    }
}
