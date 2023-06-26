using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class NeedleCustomTransformer : MonoBehaviour, ITransformer
{
    private NeedleInteraction _needleInteraction;

    private IGrabbable _grabbable;
    private Pose _grabDeltaInLocalSpace;

    void Start()
    {
        _needleInteraction = this.GetComponent<NeedleInteraction>();
    }

    public void Initialize(IGrabbable grabbable)
    {
        _grabbable = grabbable;
    }

    public void BeginTransform()
    {
        Pose grabPoint = _grabbable.GrabPoints[0];
        var targetTransform = _grabbable.Transform;
        _grabDeltaInLocalSpace = new Pose(targetTransform.InverseTransformVector(grabPoint.position - targetTransform.position),
                                        Quaternion.Inverse(grabPoint.rotation) * targetTransform.rotation);
    }

    public void UpdateTransform()
    {
        if(_needleInteraction.GetCurrentState() == State.IN_AIR)
        {
            Pose grabPoint = _grabbable.GrabPoints[0];
            var targetTransform = _grabbable.Transform;
            targetTransform.rotation = grabPoint.rotation * _grabDeltaInLocalSpace.rotation;
            targetTransform.position = grabPoint.position - targetTransform.TransformVector(_grabDeltaInLocalSpace.position);
        }
    }

    public void EndTransform() { }
}
