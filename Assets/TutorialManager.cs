using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialManager : MonoBehaviour
{
    [Header("Tutorial")]
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    [SerializeField] private Button _previousButton;
    [SerializeField] private Button _nextButton;
    [SerializeField] private Button _finishButton;

    [SerializeField] private GameObject _videoPlayerPanel;
    [SerializeField] private GameObject _textPanel;
    [SerializeField] private GameObject _buttonsPanel;



    [Header("Input Method Selection")]
    [SerializeField] private GameObject _selectionText;
    [SerializeField] private GameObject _selectionButtons;

    private string[] _handsTitles;
    private string[] _controllersTitles;
    private string[] _handsDescriptions;
    private string[] _controllersDescriptions;
    public VideoClip[] _handsVideos;
    public VideoClip[] _controllersVideos;

    private int _currentIndex;
    private string[] _currentTitles;
    private string[] _currentDescriptions;
    private VideoClip[] _currentVideos;
    
    void Start()
    {
        _handsTitles = new string[]
        {
            "Interacting with the user interface.",
            "Grabbing objects.",
            "Release objects.",
            "Grabbing the power drill.",
            "Using the power drill.",
            "Using the syringe."
        };

        _controllersTitles = new string[]
        {
            "Interacting with the user interface.",
            "Grabbing objects.",
            "Release objects.",
            "Grabbing the power drill.",
            "Using the power drill.",
            "Using the syringe."
        };

        _handsDescriptions = new string[]
        {
            "Point your hand towards an interface and pinch with your index and middle fingers to interact.",
            "Grab objects by bringing your hand closer and pinching with your fingers.",
            "Release objects by opening your index and middle fingers outward.",
            "Grab the power drill by bringing your hand closer and closing your palm.",
            "Activate the power drill by flexing your index inward. Flex your finger outward to stop using it.",
            "After grabbing the syringe, inject it by flexing your index and middle fingers inward."
        };

        _controllersDescriptions = new string[]
        {
            "Point the controller towards an interface and press the index trigger to interact.",
            "Grab objects by bringing the virtual hand closer and pressing the index trigger on the controller.",
            "Release objects by releasing the index trigger on the controller.",
            "Grab the power drill by bringing the virtual hand closer and pressing the hand trigger on the controller.",
            "Activate the power drill by the index trigger on the controller. Release the trigger to stop using it.",
            "After grabbing the syringe, inject it by pressing the index trigger on the controller."
        };

        _currentIndex = 0;
    }

    public void ChooseHands()
    {
        _currentTitles = _handsTitles;
        _currentDescriptions = _handsDescriptions;
        _currentVideos = _handsVideos;
        StartTutorial();
        UpdateUI();
    }

    public void ChooseControllers()
    {
        _currentTitles = _controllersTitles;
        _currentDescriptions = _controllersDescriptions;
        _currentVideos = _controllersVideos;
        StartTutorial();
        UpdateUI();
    }

    public void StartTutorial()
    {
        _selectionText.SetActive(false);
        _selectionButtons.SetActive(false);

        _videoPlayerPanel.SetActive(true);
        _textPanel.SetActive(true);
        _buttonsPanel.SetActive(true);
    }

    public void UpdateUI()
    {
        _titleText.text = _currentTitles[_currentIndex];
        _descriptionText.text = _currentDescriptions[_currentIndex];
        _videoPlayer.clip = _currentVideos[_currentIndex];
        _videoPlayer.Prepare();

        _previousButton.interactable = _currentIndex != 0;
        _nextButton.interactable = _currentIndex != (_currentTitles.Length - 1);
        _finishButton.gameObject.SetActive(_currentIndex == (_currentTitles.Length - 1));
    }

    public void OnPreviousButtonClicked()
    {
        _currentIndex--;
        UpdateUI();
    }

    public void OnNextButtonClicked()
    {
        _currentIndex++;
        UpdateUI();
    }
}
