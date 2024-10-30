using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Meta.WitAi.TTS.Utilities;
using Oculus.Voice;
using Meta.WitAi.Json;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Meta.WitAi;
using OpenAI;
using Samples.Whisper;
using Unity.VisualScripting;
using UnityEngine.UI;


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
        [SerializeField] private Button recordButton;
        //[SerializeField] private AppVoiceExperience appVoiceExperience;
        [SerializeField] private bool showJson;

        
        public bool IsActive => _active;
        private bool _active = false;
        
        private readonly string fileName = "output.wav";
        private readonly int duration = 7;
        
        private AudioClip clip;
        private bool isRecording;
        private float time;
        
        public float silenceDuration = 3.0f; 
        public float silenceThreshold = 0.01f;
        private float silenceTime = 0f;
        
        private bool silenceDetected = false;
        private bool voiceDetected = false;
        
        private DateTime startTime;
    
        private OpenAIApi openai = new OpenAIApi();

        private List<ChatMessage> messages = new List<ChatMessage>();
        private string prompt = "You are a helpful assistant specialized in intraosseous injections. Give short, concise answers. If asked question that are out of context, don't provide the answer and suggest the user to quit the game if they are done asking questions on intraosseous injections. When the user asks to see something, always write 'Now you will be able to see' and then the name of the object";
        
        
        
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
            Debug.Log("In Start");
            npcText.text = "Hey! I'll be your virtual assistant during the Intraosseous Insertion simulation." +
                " Please enter your name in the window in front of you to start the simulation " +
                "and feel free to ask me any questions if you have any doubts.";
            recordButton.onClick.AddListener(ToggleRecording);
            StartCoroutine(Wait(3f));
        }
  


        
        
        private async void SendMessageToRasa()
        {
            Debug.Log("In SendMessageToRasa");
            Debug.Log("Input field:" + inputField.text);
            startTime = DateTime.Now;
            
            
            
           
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };
            
            Debug.Log("newMessage:" + newMessage);
            

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text; 
            
            messages.Add(newMessage);
            
            recordButton.enabled = false;
            inputField.enabled = false;
            
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "ft:gpt-4o-mini-2024-07-18:personal:io-tenth-experiment:ANtYAzny",
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();
                
                messages.Add(message);
                npcText.text = message.Content;

                StartCoroutine(LogTimeOnSpeechStart());
                _speaker.SpeakQueued(message.Content);
                
                
                string text = message.Content;
                if (text.Contains("able to see") && text.Contains("access point"))
                {
                    SetActiveSuggestion(_insertionPositionSuggestion);
                }
                else if (text.Contains("able to see") && (text.Contains("angle")||text.Contains("inclination")))
                {
                    SetActiveSuggestion(_drillingInclinationSuggestion);
                }
                else if (text.Contains("able to see") && text.Contains("needle") && text.Contains("correct") && text.Contains("insert"))
                {
                    Needle needle = _drill.GetComponent<PowerDrill>().GetNeedle();
                    EnableLine(needle);
                    Debug.Log("ENTRATO");
                }
                else if(text.Contains("able to see") && text.Contains("15mm"))
                {
                    EnableSuggestion(_15mmNeedle);
                }
                else if(text.Contains("able to see") && text.Contains("25mm"))
                {
                    EnableSuggestion(_25mmNeedle);
                }
                else if(text.Contains("able to see") && text.Contains("40mm"))
                {
                    EnableSuggestion(_40mmNeedle);
                }
                else if(text.Contains("able to see") && (text.Contains("Connect") || text.Contains("connect")))
                {
                    EnableSuggestion(_connector);
                }
                else if(text.Contains("able to see") && (text.Contains("Stabilizer") || text.Contains("stabilize")))
                {
                    EnableSuggestion(_stabilizer);
                }     
                else if(text.Contains("able to see") && (text.Contains("Power") || text.Contains("power") || text.Contains("drill") || text.Contains("driver")))
                {
                    EnableSuggestion(_drill);
                }
            }
            else
            {
                Debug.LogWarning("No text was generated from this prompt.");
            }

            ClearAfterDelay();

            recordButton.enabled = true;
            inputField.enabled = true;
        }
        
        
        private IEnumerator LogTimeOnSpeechStart()
        {
            while (!_speaker.IsSpeaking)
            {
                yield return null; 
            }
            
            TimeSpan timeElapsed = DateTime.Now - startTime;
            Debug.Log($"Time elapsed from SendMessageToRasa to actual speech start: {timeElapsed.TotalMilliseconds} ms");
        }




        private IEnumerator Wait(float seconds)
        {
            Debug.Log("In Wait");
            yield return new WaitForSeconds(seconds);  
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

        public void ClearAfterDelay()
        {
            StartCoroutine(ClearTextCoroutine());
        }

        private IEnumerator ClearTextCoroutine()
        {
            yield return new WaitForSeconds(3);
            inputField.text = "";
        }
        
        private void StartRecording()
        {
            Debug.Log("In Start recording");
            if (Microphone.devices.Length > 0)
            {
                //isRecording = true;
                //recordButton.enabled = false;
                
                #if !UNITY_WEBGL
                    clip = Microphone.Start(Microphone.devices[0], false, duration, 44100);
                #endif
            }
            else
            {
                Debug.Log("No microphone detected");
            }
            
        }
        
       

        
        private async void EndRecording()
        {
            time = 0;
            silenceTime = 0f;
            Debug.Log("In End recording");
            if (isRecording)
            {
                isRecording = false;
                Microphone.End(Microphone.devices[0]);
                Debug.Log("Recording stopped due to silence.");
            }else if (!voiceDetected)
            {
                //voiceDetected = false;
                Debug.Log("Empty message");
                npcText.text = "Sorry, I didn't hear anything. If you want to ask me a question, please try again.";
                _speaker.SpeakQueued(npcText.text);
                return;
            }
            voiceDetected = false;
        //#if !UNITY_WEBGL
            //Microphone.End(null);
        //#endif
            inputField.text = "Transcribing...";
            byte[] data = SaveWav.Save(fileName, clip);
            
            var req = new CreateAudioTranscriptionsRequest
            {
                FileData = new FileData() {Data = data, Name = "audio.wav"},
                // File = Application.persistentDataPath + "/" + fileName,
                Model = "whisper-1",
                Language = "en",
                //Prompt = "IO, IO access, Intraosseous Access, indications of use, contraindications, coagulopathy, infusion, EZ Connector, EZ Power Drill, EZ Stabilizer, insertion, information, cardiac arrest, medication, access point, perform, emergency, inclination, What are "
                Prompt = "Transcribe the audio, focusing on medical terminology related to intraosseous (IO) injections. Prioritize accuracy over completeness. Avoid generating text during periods of silence. If a word or phrase is unclear, simply transcribe it phonetically or mark it as unclear. Recognise key words like: IO, IO access, Intraosseous Access, indications of use, contraindications, coagulopathy, infusion, EZ Connector, EZ Power Drill, EZ Stabilizer, insertion, information, cardiac arrest, medication, access point, perform, emergency, inclination, What are "
            };
            var res = await openai.CreateAudioTranscription(req);

            
            inputField.text = res.Text;
            //SendMessageToRasa();
            recordButton.enabled = true;
        }

        private void ToggleRecording()
        {
            Debug.Log("Toggle");
            Debug.Log(isRecording);
            if (isRecording)
            {
                isRecording = false;
                EndRecording();
                Microphone.End(Microphone.devices[0]);
            }
            else
            {
                isRecording = true;
                StartRecording();
            }
            
        }
        

        private void Update()
        {
            if (isRecording)
            {
                CheckSilence();
                time += Time.deltaTime;
                
                if (time >= duration)
                {
                    isRecording = false;
                    EndRecording();
                }
            }
        }
        
        void CheckSilence()
        {
            //Debug.Log("In Check Silence");
            int micPosition = Microphone.GetPosition(Microphone.devices[0]);
            float[] samples = new float[2048];
            int startPosition = micPosition - samples.Length;
            if (startPosition < 0)
            {
                Debug.Log("Invalid Start Position");
                return;

            }
            clip.GetData(samples, startPosition);

            float averageVolume = 0f;

            
            foreach (float sample in samples)
            {
                averageVolume += Mathf.Abs(sample);
            }
            averageVolume /= samples.Length;
            
            if ((averageVolume > silenceThreshold) && !voiceDetected)
            {
                Debug.Log("Voice detected");
                voiceDetected = true;
            }

            if (voiceDetected)
            {
                if (averageVolume < silenceThreshold)
                {
                    silenceTime += Time.deltaTime;
                   // Debug.Log("Silence detected");
                    if (silenceTime >= silenceDuration) 
                    {
                        Debug.Log("Time detected");
                        EndRecording();
                        //silenceTime = 0f;
                    }
                }
                else
                {
                    //Debug.Log("Sound detected");
                    silenceTime = 0f;
                }  
                
            }
            
        }
        
        /*void CheckSilence()
        {
            int micPosition = Microphone.GetPosition(Microphone.devices[0]);
            float[] samples = new float[2048];
            int startPosition = micPosition = micPosition - samples.Length;
            
            clip.GetData(samples, startPosition);
            //micPosition - samples.Length
            
            float averageVolume = 0f;

            if (startPosition >= 0)
            {
                foreach (float sample in samples)
                {
                    averageVolume += Mathf.Abs(sample);
                }
                averageVolume /= samples.Length;

                if ((averageVolume > silenceThreshold) && !voiceDetected)
                {
                    Debug.Log("Voice detected");
                    voiceDetected = true;
                }

                if (voiceDetected)
                {
                    if ((averageVolume < silenceThreshold))
                    {
                        if (!silenceDetected)
                        {
                            Debug.Log("Silence detected");
                            silenceDetected = true;
                            StartCoroutine(StopAfterSilence());
                        }
                    }
                    else
                    {
                        Debug.Log("Silence stopped");
                        silenceDetected = false;
                        StopCoroutine(StopAfterSilence()); 
                    }
                
                }

            }
            
        }*/

        IEnumerator StopAfterSilence()
        {
            Debug.Log("In StopAfterSilence");
            yield return new WaitForSeconds(silenceDuration);
       
            if (silenceDetected)
            {
                EndRecording();
                silenceDetected = false;
            }
        }
        

        public void Repeat()
        {
            Debug.Log("In Repeat");
            _speaker.Speak(npcText.text);
        }
    }
}
