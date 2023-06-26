using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TaskGUI : MonoBehaviour
{
    [SerializeField] private Transform _canvas;
    [SerializeField] private TextMeshProUGUI _header;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _currentTaskIndex;

    [SerializeField] private Vector3 _canvasScale;
    [SerializeField] private float _duration;

    public void UpdateUI(string newDescription)
    {
        _canvas.DOScale(new Vector3(0, 0, 0), _duration).SetEase(Ease.Linear).OnComplete(() =>
        {
            _description.text = newDescription;
            _canvas.DOScale(_canvasScale, _duration).SetEase(Ease.Linear);
        });
    }
}
