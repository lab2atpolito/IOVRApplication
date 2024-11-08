using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Meta.WitAi.TTS.Utilities;
using Oculus.Voice;
using Meta.WitAi.Json;
using System.Collections.Generic;
using Meta.WitAi;
using OpenAI;
using System.IO;


// A struct to help in creating the Json object to be sent to the rasa server
public class PostMessageJson
{
    public string msg;
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
    
        private OpenAIApi openai = new OpenAIApi();
        private DateTime startTime;
        private string gptModel = "gpt-4o-mini";
        //private string gptModel = "ft:gpt-4o-mini-2024-07-18:personal:io-tenth-experiment:ANtYAzny";

        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "You are a friendly, specialized assistant guiding the user through an intraosseous (IO) access simulation in a VR app. The patient is a female adult, and the required needle in this case is the blue 25mm needle. You’ll receive contextual messages from the VR app about the current simulation stage to help provide accurate guidance. Answer Guidelines: Keep responses under 140 characters, including spaces. This limit is strict. Respond warmly to greetings (e.g., ‘Hello! How can I help with your IO procedure?’). For unrelated questions (e.g., general knowledge like ‘What’s the capital of France?’), respond with: ‘I’m here to help with intraosseous injections! If you’re done, feel free to close this session.’ If asked if actions are correct, say: ‘When done correctly, you’ll hear a sound and can proceed to the next step.’ Answer in context if questions relate to prior ones. If the user says a word slightly wrong but you understand the intended meaning based on context, respond using the correct word. If the user asks to view the spot, place, he is referring to the access point. Viewing Requests: If the user requests to view any of the following, respond: ‘Now you will be able to see [item].’ Items include: Access point, Inclination of the drill, EZ Stabilizer, EZ-IO Power Driver, EZ Connect extension set, catheter, stylet, Needle sets by color and size: Pink needle: 15mm, Blue needle: 25mm, Yellow needle: 45mm. For other items, respond: ‘Unfortunately, I cannot show that, but you can ask further questions.’ Device and Procedure Reminders: Remind users of device details or steps as needed, especially about the correct needle size and color for each part of the procedure. Important: All responses must fit within 140 characters, including spaces, and prioritize brevity and clarity.";
        //private string prompt = "You are a knowledgeable assistant specializing in intraosseous injections. Provide brief, precise answers within 140 characters. If asked a question unrelated to intraosseous injections, respond with, 'I’m here to help with intraosseous injections! If you’re done, feel free to close this session.' When a user asks to view something, respond with: 'Now you will be able to see' followed by the name of the item or object.";
        //private string prompt = "You are a knowledgeable assistant specializing in intraosseous injections. Provide brief, precise answers within 140 characters. When a user asks to view something, respond with: 'Now you will be able to see' followed by the name of the item or object.";


        [SerializeField] public TTSSpeaker _speaker;

        [SerializeField] private GameObject _drillingInclinationSuggestion;
        [SerializeField] private GameObject _insertionPositionSuggestion;
        [SerializeField] private GameObject _15mmNeedle;
        [SerializeField] private GameObject _25mmNeedle;
        [SerializeField] private GameObject _40mmNeedle;
        [SerializeField] private GameObject _connector;
        [SerializeField] private GameObject _stabilizer;
        [SerializeField] private GameObject _drill;
        [SerializeField] private GameObject _catheter;
        [SerializeField] private GameObject _stylet;

        void Start ()
        {
            Debug.Log("In Start");
            npcText.text = "Hey! I'll be your virtual assistant during the Intraosseous Insertion simulation." +
                " Please enter your name in the window in front of you to start the simulation " +
                "and feel free to ask me any questions if you have any doubts.";
           
            myFilePath = Application.persistentDataPath + "/TestsOpenAI";
            if (!Directory.Exists(myFilePath))
            {
                Debug.Log("Creating new folder");
                Directory.CreateDirectory(myFilePath);
            }
            filename = "TestOpenAI_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".txt";

            filePath = Path.Combine(myFilePath, filename);



            //AddMessageToChat();
            

            conversation = "\n assistant (auto): \n" + npcText.text +"\n";
            StartCoroutine(Wait(3f));
        }
        
        
        private async void SendMessageToRasa()
        {
            conversation = conversation + "\n user: \n" + inputField.text;
            Debug.Log("In SendMessageToRasa");
            Debug.Log("Input field:" + inputField.text);
            /*if (npcText != null)
            {
                npcText.text = "message received";  
            }*/
            startTime = DateTime.Now;
            if (inputField.text == "")
            {
                npcText.text = "I didn't understand. Could you repeat?";
                _speaker.SpeakQueued(npcText.text);
                conversation = "\n assistant (auto): \n" + npcText.text + "\n";
                StartCoroutine(LogTimeOnSpeechStart());
            }
            else
            {
                var newMessage = new ChatMessage()
                {
                    Role = "user",
                    Content = inputField.text
                };

                Debug.Log("newMessage:" + newMessage);


                if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text;

                messages.Add(newMessage);

                //button.enabled = false;
                inputField.text = "";
                inputField.enabled = false;

                // Complete the instruction
                var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
                {
                    //Model = "ft:gpt-4o-mini-2024-07-18:personal:io-tenth-experiment:ANtYAzny",
                    Model = gptModel,
                    Messages = messages
                });

                if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
                {
                    var message = completionResponse.Choices[0].Message;
                    message.Content = message.Content.Trim();

                    messages.Add(message);
                    npcText.text = message.Content;
                    _speaker.SpeakQueued(npcText.text);
                    StartCoroutine(LogTimeOnSpeechStart());
                    


                    string text = message.Content;
                    if (text.Contains("able to see") && text.Contains("point") || text.Contains("site"))
                    {
                        SetActiveSuggestion(_insertionPositionSuggestion);
                    }
                    if (text.Contains("able to see") && (text.Contains("angle") || text.Contains("inclination")))
                    {
                        SetActiveSuggestion(_drillingInclinationSuggestion);
                    }
                    /*else if (text.Contains("able to see") && text.Contains("needle") && text.Contains("correct") && text.Contains("insert"))
                    {
                        Needle needle = _drill.GetComponent<PowerDrill>().GetNeedle();
                        EnableLine(needle);
                        Debug.Log("ENTRATO");
                    }*/
                    if (text.Contains("able to see") && text.Contains("15mm") || text.Contains("pink"))
                    {
                        EnableSuggestion(_15mmNeedle);
                    }
                    if (text.Contains("able to see") && text.Contains("25mm") || text.Contains("blue"))
                    {
                        EnableSuggestion(_25mmNeedle);
                    }
                    if (text.Contains("able to see") && text.Contains("40mm") || text.Contains("yellow"))
                    {
                        EnableSuggestion(_40mmNeedle);
                    }
                    if (text.Contains("able to see") && (text.Contains("Connect") || text.Contains("connect")))
                    {
                        EnableSuggestion(_connector);
                    }
                    if (text.Contains("able to see") && (text.Contains("Stabilizer") || text.Contains("stabilize")))
                    {
                        EnableSuggestion(_stabilizer);
                    }
                    if (text.Contains("able to see") && (text.Contains("Power") || text.Contains("power") || text.Contains("drill") || text.Contains("driver")))
                    {
                        EnableSuggestion(_drill);
                    }
                    if (text.Contains("able to see") && (text.Contains("catheter")))
                    {
                        EnableSuggestion(_catheter);
                    }
                    if (text.Contains("able to see") && (text.Contains("stylet")))
                    {
                        EnableSuggestion(_stylet);
                    }
                }
                else
                {
                    Debug.LogWarning("No text was generated from this prompt.");
                }
                conversation = conversation + "\n assistant: \n" + npcText.text;

            }


            //button.enabled = true;
            inputField.enabled = true;
            inputField.text = "";
        }
        

        private IEnumerator Wait(float seconds)
        {
            Debug.Log("In Wait");
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
            Debug.Log("In EnableSuggestionCoroutine");
            gameObject.GetComponent<Outline>().enabled = true;
            //Play audio
            yield return new WaitForSeconds(20f);
            gameObject.GetComponent<Outline>().enabled = false;
        }

        IEnumerator EnableBlueLineCoroutine(Needle needle)
        {
            Debug.Log("In EnableBlueLineCoroutine");
            needle.ShowLine();
            yield return new WaitForSeconds(40f);
            needle.HideLine();
        }

        IEnumerator SetActiveSuggestionCoroutine(GameObject gameObject)
        {
            Debug.Log("In SetActiveSuggestionCoroutine");
            gameObject.SetActive(true);
            //Play audio
            yield return new WaitForSeconds(20f);
            gameObject.SetActive(false);
        }

        public void SetText(string text)
        {
            Debug.Log("In SetText");
            inputField.text = text;
        }

        private IEnumerator LogTimeOnSpeechStart()
        {
            while (!_speaker.IsSpeaking)
            {
                yield return null;
            }

            TimeSpan timeElapsed = DateTime.Now - startTime;
            Debug.Log($"Time elapsed from SendMessageToRasa to actual speech start: {timeElapsed.TotalMilliseconds} ms");
            conversation = conversation + "\n time : \n" +  timeElapsed.TotalMilliseconds + "ms\n";
        }

        // Add delegates
        private void OnEnable()
        {
            Debug.Log("In OnEnable");
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
            Debug.Log("In OnDisable");
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
            Debug.Log("In OnRequestTranscript");
            inputField.text = transcript;
        }

        // Listen start
        private void OnListenStart()
        {
            Debug.Log("In OnListenStart");
            inputField.text = "Listening...";
        }

        // Listen stop
        private void OnListenStop()
        {
            Debug.Log("In OnListenStop");
            inputField.text = "Processing...";
        }

        // Listen stop
        private void OnListenForcedStop()
        {
            Debug.Log("In OnListenForcedStop");
            if (!showJson)
            {
                inputField.text = "";
            }
            OnRequestComplete();
        }

        // Request response
        private void OnRequestResponse(WitResponseNode response)
        {
            Debug.Log("In OnRequestResponse");
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
        private void OnRequestError(string error, string msg)
        {
            Debug.Log("In OnRequestError");
            if (!showJson)
            {
                inputField.text = $"<color=\"red\">Error: {error}\n\n{msg}</color>";
            }
            OnRequestComplete();
        }
        // Deactivate
        private void OnRequestComplete()
        {
            Debug.Log("In OnRequestComplete");
            _active = false;
        }

        // Toggle activation
        public void ToggleActivation()
        {
            Debug.Log("In ToggleActivation");
            SetActivation(!_active);
        }
        // Set activation
        public void SetActivation(bool toActivated)
        {
            Debug.Log("In SetActivation");
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

        public async void OnStartButton()
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = prompt
            };


            Debug.Log("newMessage:" + newMessage);

            messages.Add(newMessage);

            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                //Model = "ft:gpt-4o-mini-2024-07-18:personal:io-tenth-experiment:ANtYAzny",
                Model = gptModel,
                Messages = messages
            });

            var newMessage2 = new ChatMessage()
            {
                Role = "assistant",
                Content = "The first thing you have to do is to grab the ankle and straighten the leg."
            };


            Debug.Log("newMessage:" + newMessage2);

            messages.Add(newMessage2);

            completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                //Model = "ft:gpt-4o-mini-2024-07-18:personal:io-tenth-experiment:ANtYAzny",
                Model = gptModel,
                Messages = messages
            });
        }

        public async void OnNext()
        {
            StartCoroutine(saveNpcText());
            //AddMessageToChat();
            var newMessage = new ChatMessage()
            {
                Role = "assistant",
                Content = npcText.text
            };


            Debug.Log("newMessage:" + newMessage);

            messages.Add(newMessage);

            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                //Model = "ft:gpt-4o-mini-2024-07-18:personal:io-tenth-experiment:ANtYAzny",
                Model = gptModel,
                Messages = messages
            });

        }

        public async void AddMessageToChat()
        {
            var newMessage = new ChatMessage()
            {
                Role = "assistant",
                Content = npcText.text
            };


            Debug.Log("newMessage:" + newMessage);


            if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text;

            messages.Add(newMessage);

            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                //Model = "ft:gpt-4o-mini-2024-07-18:personal:io-tenth-experiment:ANtYAzny",
                Model = gptModel,
                Messages = messages
            });
        }

        private IEnumerator saveNpcText()
        {
            yield return new WaitForSeconds(0.3f);
            conversation = conversation + "\n assistant (auto): \n" + npcText.text;

        }

            public void Repeat()
        {
            Debug.Log("In Repeat");
            _speaker.Speak(npcText.text);
            
        }


        private void OnApplicationQuit()
        {
            File.WriteAllText(filePath, conversation);
            Debug.Log("saved to file to "+ filePath);
        }
    }
}
