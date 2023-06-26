using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField] private Image _loadingIcon;
    [SerializeField] private TextMeshProUGUI _loadingText;

    void Start()
    {
        DOTween.To(() => _loadingIcon.transform.rotation.eulerAngles,
            angle => _loadingIcon.transform.rotation = Quaternion.Euler(angle),
            new Vector3(0, 0, 360), 2f)
            .SetEase(Ease.Linear)
            .SetLoops(-1);
        DOTween.To(() => _loadingText.color.a,
            alpha => {
                Color newColor = _loadingText.color;
                newColor.a = alpha;
                _loadingText.color = newColor;
            },
            0.5f,
            1f)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
