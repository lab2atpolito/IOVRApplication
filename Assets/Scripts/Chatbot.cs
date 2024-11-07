using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using Meta.WitAi.TTS.Utilities;
using Oculus.Voice;
using Meta.WitAi.Json;
using System.IO;

// A struct to help in creating the Json object to be sent to the rasa server
public class PostMessageJson
{
    public string message;
    public string sender;
}

[Serializable]
// A class to extract multiple json objects nested inside a value
public class RootReceiveMessageJson
{
    public ReceiveMessageJson[] messages;
}

[Serializable]
// A class to extract a single message returned from the bot
public class ReceiveMessageJson

{
    public string recipient_id;
    public string text;
}

namespace Meta.WitAi.TTS.Samples
{
    public class Chatbot : MonoBehaviour
    {
        [Header("Attributes")]
        public string npcMsg;

        [Header("UI")]
        public TextMeshProUGUI npcText;
        public TMP_InputField inputField;

        [Header("Voice")]
        [SerializeField] private AppVoiceExperience appVoiceExperience;
        [SerializeField] private bool showJson;

        // Whether voice is activated
        public bool IsActive => _active;
        private bool _active = false;
        public string conversation;
        string myFilePath, filename, filePath;

        private DateTime startTime;


        private const string rasa_url = "http://localhost:5005/webhooks/rest/webhook";
    
        [SerializeField] public TTSSpeaker _speaker;

        [SerializeField] private GameObject _drillingInclinationSuggestion;
        [SerializeField] private GameObject _insertionPositionSuggestion;
        [SerializeField] private GameObject _15mmNeedle;
        [SerializeField] private GameObject _25mmNeedle;
        [SerializeField] private GameObject _40mmNeedle;
        [SerializeField] private GameObject _connector;
        [SerializeField] private GameObject _stabilizer;
        [SerializeField] private GameObject _drill;

        void Start ()
        {
            npcText.text = "Hey! I'll be your virtual assistant during the Intraosseous Insertion simulation." +
                " Please enter your name in the window in front of you to start the simulation " +
                "and feel free to ask me any questions if you have any doubts.";
            myFilePath = Application.persistentDataPath + "/TestsRasa";
            if (!Directory.Exists(myFilePath))
            {
                Debug.Log("Creating new folder");
                Directory.CreateDirectory(myFilePath);
            }
            filename = "TestRasa_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt";

            filePath = Path.Combine(myFilePath, filename);

            conversation = " assistant (auto): \n" + npcText.text + "\n";
            StartCoroutine(Wait(3f));
        }
  


        public void SendMessageToRasa(string s)
        {
            conversation = conversation + "\n user: \n" + inputField.text;
            startTime = DateTime.Now;
            //Debug.Log("Input: " + s);
            if (s != "") {
            //inputField.text = s;
            // Create a json object from user message
            PostMessageJson postMessage = new PostMessageJson
            {
                sender = "user",
                message = s
            };

            string jsonBody = JsonUtility.ToJson(postMessage);
            print("User json : " + jsonBody);

            // Create a post request with the data to send to Rasa server

            StartCoroutine(PostRequest(rasa_url, jsonBody));
            }
        }

         public void SendMessageToRasa () {
            if(inputField.text == "Listening..." || inputField.text == "Processing...")
            {
                inputField.text = "";
            }
            conversation = conversation + "\n user: \n" + inputField.text;
            startTime = DateTime.Now;
            // get user messasge from input field, create a json object 
            // from user message and then clear input field
            string s = inputField.text;
            inputField.text = "";

            // if user message is not empty, send message to bot
            if (s != "") {
                // create json from message
                PostMessageJson postMessage = new PostMessageJson {
                    sender = "user",
                    message = s
                };
                string jsonBody = JsonUtility.ToJson(postMessage);

            
           

                // Create a post request with the data to send to Rasa server
                StartCoroutine(PostRequest(rasa_url, jsonBody));
            }
        }

        private IEnumerator PostRequest(string url, string jsonBody)
        {
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                byte[] rawBody = new System.Text.UTF8Encoding().GetBytes(jsonBody);
                request.uploadHandler = (UploadHandler)new UploadHandlerRaw(rawBody);
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                ReceiveResponse(request.downloadHandler.text);
            }
        }

        public void ReceiveResponse(string response)
        {
            // Deserialize response recieved from the bot
            RootReceiveMessageJson root = JsonUtility.FromJson<RootReceiveMessageJson>("{\"messages\":" + response + "}");

            if (root.messages != null && root.messages.Length > 0)
            {
                Debug.Log("Bot: " + root.messages[0].text);

                // Display the bot's response
                npcText.text = root.messages[0].text;

                // Speak the bot's response using Text-to-Speech
                _speaker.SpeakQueued(root.messages[0].text);
                StartCoroutine(LogTimeOnSpeechStart());

                conversation = conversation + " assistant: \n" + npcText.text +"\n";

                string text = root.messages[0].text;
                if (text == "Now you will be able to see the correct insertion point for the needle on the patient's leg")
                {
                    SetActiveSuggestion(_insertionPositionSuggestion);
                }
                else if (text == "Now you will be able to see a blue-colored line on the scene indicating the correct angle of the drill")
                {
                    SetActiveSuggestion(_drillingInclinationSuggestion);
                }
                else if (text == "Now you will be able to see on the selected needle the white line which can help you to verify that the needle is inserted correctly")
                {
                    Needle needle = _drill.GetComponent<PowerDrill>().GetNeedle();
                    EnableLine(needle);
                    Debug.Log("ENTRATO");
                }
                else if(text == "Now you will be able to see in the scene the 15mm needle set outlined")
                {
                    EnableSuggestion(_15mmNeedle);
                }
                else if(text == "Now you will be able to see in the scene the 25mm needle set outlined")
                {
                    EnableSuggestion(_25mmNeedle);
                }
                else if(text == "Now you will be able to see in the scene the 45mm needle set outlined")
                {
                    EnableSuggestion(_40mmNeedle);
                }
                else if(text == "Now you will be able to see on the floor the EZ Connect extension set outlined")
                {
                    EnableSuggestion(_connector);
                }
                else if(text == "Now you will be able to see on the floor the EZ Stabilizer dressing outlined")
                {
                    EnableSuggestion(_stabilizer);
                }     
                else if(text == "Now you will be able to see in the scene the EZ-IO Power Driver outlined")
                {
                    EnableSuggestion(_drill);
                }
            }
            else
            {
                Debug.LogWarning("No messages received from the bot.");
            }

        }

        private IEnumerator Wait(float seconds)
        {
            yield return new WaitForSeconds(seconds);  // Wait for 20 seconds
            _speaker.SpeakQueued(npcText.text);
        }

        private void SetActiveSuggestion(GameObject gameObject)
        {
            StartCoroutine(SetActiveSuggestionCoroutine(gameObject));
        }

        private void EnableSuggestion(GameObject gameObject)
        {
            StartCoroutine(EnableSuggestionCoroutine(gameObject));
        }

        private void EnableLine(Needle needle)
        {
            StartCoroutine(EnableBlueLineCoroutine(needle));
        }

        IEnumerator EnableSuggestionCoroutine(GameObject gameObject)
        {
            gameObject.GetComponent<Outline>().enabled = true;
            //Play audio
            yield return new WaitForSeconds(20f);
            gameObject.GetComponent<Outline>().enabled = false;
        }

        IEnumerator EnableBlueLineCoroutine(Needle needle)
        {
            needle.ShowLine();
            yield return new WaitForSeconds(40f);
            needle.HideLine();
        }

        IEnumerator SetActiveSuggestionCoroutine(GameObject gameObject)
        {
            gameObject.SetActive(true);
            //Play audio
            yield return new WaitForSeconds(20f);
            gameObject.SetActive(false);
        }

        public void SetText(string text)
        {
            inputField.text = text;
        }

        // Add delegates
        private void OnEnable()
        {
            appVoiceExperience.VoiceEvents.OnRequestCreated.AddListener(OnRequestStarted);
            appVoiceExperience.VoiceEvents.OnPartialTranscription.AddListener(OnRequestTranscript);
            appVoiceExperience.VoiceEvents.OnFullTranscription.AddListener(OnRequestTranscript);
            appVoiceExperience.VoiceEvents.OnStartListening.AddListener(OnListenStart);
            appVoiceExperience.VoiceEvents.OnStoppedListening.AddListener(OnListenStop);
            appVoiceExperience.VoiceEvents.OnStoppedListeningDueToDeactivation.AddListener(OnListenForcedStop);
            appVoiceExperience.VoiceEvents.OnStoppedListeningDueToInactivity.AddListener(OnListenForcedStop);
            appVoiceExperience.VoiceEvents.OnResponse.AddListener(OnRequestResponse);
            appVoiceExperience.VoiceEvents.OnError.AddListener(OnRequestError);
        }
        // Remove delegates
        private void OnDisable()
        {
            appVoiceExperience.VoiceEvents.OnRequestCreated.RemoveListener(OnRequestStarted);
            appVoiceExperience.VoiceEvents.OnPartialTranscription.RemoveListener(OnRequestTranscript);
            appVoiceExperience.VoiceEvents.OnFullTranscription.RemoveListener(OnRequestTranscript);
            appVoiceExperience.VoiceEvents.OnStartListening.RemoveListener(OnListenStart);
            appVoiceExperience.VoiceEvents.OnStoppedListening.RemoveListener(OnListenStop);
            appVoiceExperience.VoiceEvents.OnStoppedListeningDueToDeactivation.RemoveListener(OnListenForcedStop);
            appVoiceExperience.VoiceEvents.OnStoppedListeningDueToInactivity.RemoveListener(OnListenForcedStop);
            appVoiceExperience.VoiceEvents.OnResponse.RemoveListener(OnRequestResponse);
            appVoiceExperience.VoiceEvents.OnError.RemoveListener(OnRequestError);
        }

        // Request began
        private void OnRequestStarted(WitRequest r)
        {
            // Store json on completion
            if (showJson) r.onRawResponse = (response) => inputField.text = response;
            // Begin
            _active = true;
        }

        // Request transcript
        private void OnRequestTranscript(string transcript)
        {
            inputField.text = transcript;
        }

        // Listen start
        private void OnListenStart()
        {
            inputField.text = "Listening...";
        }

        // Listen stop
        private void OnListenStop()
        {
            inputField.text = "Processing...";
        }

        // Listen stop
        private void OnListenForcedStop()
        {
            if (!showJson)
            {
                inputField.text = "";
            }
            OnRequestComplete();
        }

        // Request response
        private void OnRequestResponse(WitResponseNode response)
        {
            if (!showJson)
            {
                if (!string.IsNullOrEmpty(response["text"]))
                {
                    inputField.text = response["text"];
                }
                else
                {
                    inputField.text = "";
                }
            }
            OnRequestComplete();
        }
        // Request error
        private void OnRequestError(string error, string message)
        {
            if (!showJson)
            {
                inputField.text = $"<color=\"red\">Error: {error}\n\n{message}</color>";
            }
            OnRequestComplete();
        }
        // Deactivate
        private void OnRequestComplete()
        {
            _active = false;
        }

        // Toggle activation
        public void ToggleActivation()
        {
            SetActivation(!_active);
        }
        // Set activation
        public void SetActivation(bool toActivated)
        {
            if (_active != toActivated)
            {
                _active = toActivated;
                if (_active)
                {
                    appVoiceExperience.Activate();
                }
                else
                {
                    appVoiceExperience.Deactivate();
                }
            }
        }

        public void Repeat()
        {
            _speaker.Speak(npcText.text);
        }

        private IEnumerator LogTimeOnSpeechStart()
        {
            while (!_speaker.IsSpeaking)
            {
                yield return null;
            }

            TimeSpan timeElapsed = DateTime.Now - startTime;
            Debug.Log($"Time elapsed from SendMessageToRasa to actual speech start: {timeElapsed.TotalMilliseconds} ms");
            conversation = conversation + "\n time : \n" + timeElapsed.TotalMilliseconds + "ms\n";
        }


        public void OnNext()
        {
            StartCoroutine(saveNpcText());
        }

        private IEnumerator saveNpcText()
        {
            yield return new WaitForSeconds(0.3f);
            conversation = conversation + " assistant (auto): \n" + npcText.text;

        }



        private void OnApplicationQuit()
        {
            File.WriteAllText(filePath, conversation);
            Debug.Log("saved to file to " + filePath);
        }

    }
}
