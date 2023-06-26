using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskElementUI : MonoBehaviour
{
    private bool _isCompleated = false;
    private Button _button;

    [SerializeField] private TextMeshProUGUI _header;
    [SerializeField] private TextMeshProUGUI _description;
    [SerializeField] private TextMeshProUGUI _timestamp;
    [SerializeField] private Image _icon;

    [SerializeField] private Sprite _completedIcon;

    private TimeManager _timeManager;


    // Start is called before the first frame update
    void Start()
    {
        _button = this.GetComponent<Button>();
        _timeManager = FindObjectOfType<TimeManager>();
    }

    // Update is called once per frame
    void Update()
    {
       if (_isCompleated){
            _button.interactable = true;
       }
       else
       {
            _button.interactable = false;
       }
    }

    public void SetHeader(string header) => _header.text = header;
    public void SetDescription(string description) => _description.text = description;

    public void SetAsCompleated(string timestamp)
    {
        _isCompleated = true;
        _icon.sprite = _completedIcon;
        _timestamp.text = timestamp;
    }
}
