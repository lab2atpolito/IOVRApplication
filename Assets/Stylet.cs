using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Stylet : MonoBehaviour
{
    public void RemoveStylet()
    {
        this.GetComponentInParent<NeedleInteraction>().RemoveStylet();
        this.transform.parent = null;
        this.transform.DOScale(0f, 1f)
                .OnComplete(() => Destroy(this))
                .SetEase(Ease.InOutSine);
    }
}
