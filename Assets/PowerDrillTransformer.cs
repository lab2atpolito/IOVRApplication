using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction;

public class PowerDrillTransformer : MonoBehaviour, ITransformer
{
    private PowerDrill _drill; 

    private IGrabbable _grabbable;
    private Pose _grabDeltaInLocalSpace;

    private void Start()
    {
        _drill = this.gameObject.GetComponent<PowerDrill>();
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
        Pose grabPoint = _grabbable.GrabPoints[0];
        var targetTransform = _grabbable.Transform;
        if( _drill.IsEmpty() || (!_drill.IsEmpty() && _drill.GetNeedle().gameObject.GetComponent<NeedleInteraction>().GetCurrentState() == State.IN_AIR))
        {
            targetTransform.rotation = grabPoint.rotation * _grabDeltaInLocalSpace.rotation;
            targetTransform.position = grabPoint.position - targetTransform.TransformVector(_grabDeltaInLocalSpace.position);
        }
        else
        {
            float speed = 1f;
            NeedleInteraction needle = _drill.GetNeedle().gameObject.GetComponent<NeedleInteraction>();

            if (needle.GetCurrentState() == State.IN_SKIN)
            {
                speed = 0.01f;
            }
            else if(needle.GetCurrentState() == State.IN_BONE)
            {
                if (!_drill.IsDrilling())
                    speed = 0.0001f;
                else
                    speed = 0.005f;
            }
            else if(needle.GetCurrentState() == State.MEDULLARY_CAVITY)
            {
                speed = 0.05f;
            }

            if(!needle.HasReachedMaxDepth())
            {
                // Update Position
                Vector3 desiredPosition = grabPoint.position - targetTransform.TransformVector(_grabDeltaInLocalSpace.position);
                Vector3 localPosition = targetTransform.InverseTransformPoint(desiredPosition);
                localPosition.x = 0f;
                localPosition.y = 0f;
                targetTransform.position = targetTransform.TransformPoint(localPosition * speed);
            }
            else
            {
                Vector3 desiredPosition = grabPoint.position - targetTransform.TransformVector(_grabDeltaInLocalSpace.position);
                Vector3 localPosition = targetTransform.InverseTransformPoint(desiredPosition);
                if(localPosition.z < 0f)
                {
                    localPosition.x = 0f;
                    localPosition.y = 0f;
                    targetTransform.position = targetTransform.TransformPoint(localPosition * speed);
                }
            }

            // Update Rotation
            Quaternion desiredRotation = grabPoint.rotation * _grabDeltaInLocalSpace.rotation;
            Quaternion localRotation = Quaternion.Inverse(targetTransform.rotation) * desiredRotation;
            Vector3 rotationAngles = localRotation.eulerAngles;
            float forwardRotation = rotationAngles.z;
            Quaternion limitedRotation = targetTransform.rotation * Quaternion.Euler(0f, 0f, forwardRotation);
            targetTransform.rotation = limitedRotation;
        }
    }

    public void EndTransform() { }
}
