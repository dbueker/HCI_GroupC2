/**
* (C) Copyright IBM Corp. 2018, 2020.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/
#pragma warning disable 0649

using System.Collections;
using IBM.Cloud.SDK;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using IBM.Cloud.SDK.Utilities;
using IBM.Watson.Assistant.V2;
using IBM.Watson.Assistant.V2.Model;
using UnityEngine;
using System.Collections.Generic;
using System.Security.Cryptography;
using System;
using Newtonsoft.Json;
using UnityEngine.Networking;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;



public class ChatBot : MonoBehaviour
{
	[Tooltip("Copy apikey from service credentials that are generated for your IBM cloud account.")]
    [SerializeField]
    private string iamApikey;
    [Tooltip("Copy url from service credentials that are generated for your IBM cloud account.")]
    [SerializeField]
    private string serviceUrl;
    [Tooltip("The version date with which you would like to use the service in the form YYYY-MM-DD.")]
    [SerializeField]
    private string versionDate;
    [Tooltip("Copy Assistant ID from IBM Watson Assistant > Settings > API Details.")]
    [SerializeField]
    private string assistantId;
	[Tooltip("GameObject with TextToSpeech component to pass the chat bot response.")]
    public TextToSpeech tts;

    public SpeechToText speechToText;
    
    private AssistantService service;

    private bool listening = false;

    private bool createSessionTested = false;
    private bool deleteSessionTested = false;
    private string sessionId;
    private HashAlgorithm hashAlgorithm;

    //References to the coaches for enabling their scripts
    public GameObject Sophie;
    public GameObject Jody;
    public GameObject David;

    public GameObject restartButton;
    public GameObject exitButton;

    private void Start()
    {
        LogSystem.InstallDefaultReactors();
        hashAlgorithm = new SHA1CryptoServiceProvider();

        Runnable.Run(CreateService());
    }


    private IEnumerator CreateService()
    {
        if (string.IsNullOrEmpty(iamApikey))
        {
            throw new IBMException("Plesae provide IAM ApiKey for the service.");
        }

        //  Create credential and instantiate service
        IamAuthenticator authenticator = new IamAuthenticator(apikey: iamApikey);

        //  Wait for tokendata
        while (!authenticator.CanAuthenticate())
            yield return null;

        service = new AssistantService(versionDate, authenticator);
        if (!string.IsNullOrEmpty(serviceUrl))
        {
            service.SetServiceUrl(serviceUrl);
        }

        Runnable.Run(CreateSession());
    }

    private IEnumerator CreateSession() {

    	Log.Debug("ChatBot", "Attempting to CreateSession");
        service.CreateSession(OnCreateSession, assistantId);

        while (!createSessionTested)
        {
            yield return null;
        }
    }

    public void sendMessage(string inputText) {

        var input = new MessageInput()
        {
            Text = inputText,
            Options = new MessageInputOptions()
            {
                ReturnContext = true
            }
        };

        input.Entities = new List<RuntimeEntity>();

        service.Message(OnMessage, assistantId, sessionId, input: input);
        
    }

    private void OnMessage(DetailedResponse<MessageResponse> response, IBMError error)
    {
        Log.Debug("ChatBot.OnMessage()", "response: {0}", response.Response);

        List<AgentResponse> allResponsesTTS = new List<AgentResponse>();

        for(int i = 0; i < response.Result.Output.Generic.Count; i++)
        {
            AgentResponse responseTTS = new AgentResponse();

            // Create and play audio file
            if(response.Result.Output.Generic[i].ResponseType == "text")
            {
                string informalSentence = response.Result.Output.Generic[i].Text;
                //string intent = response.Result.Output.Intents[0].Intent;

                if (informalSentence.Contains("This is Sophie"))
                {
                    Invoke("selectSophie", 2);
                    Invoke("buttonsAppear", 3);
                }

                if (informalSentence.Contains("This is Jody"))
                {
                    Invoke("selectJody", 2);
                    Invoke("buttonsAppear", 3);
                }

                if (informalSentence.Contains("This is David"))
                {
                    Invoke("selectDavid", 2);
                    Invoke("buttonsAppear", 3);
                }

                responseTTS.text = informalSentence;
            } 
            else if(response.Result.Output.Generic[i].ResponseType == "suggestion")
            {
             //   responseTTS.text = response.Result.Output.Generic[i].Suggestions[0].Output["generic"][0]["text"].ToString();
            }

            byte[] hashedText = hashAlgorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(responseTTS.text));
            responseTTS.fileName = "Audio/" + Convert.ToBase64String(hashedText).Replace("/", "-");

            allResponsesTTS.Add(responseTTS);
        }

        // Send collected data to text to speech service
        Runnable.Run(tts.sendMessage(allResponsesTTS));
    }

    private void OnDeleteSession(DetailedResponse<object> response, IBMError error)
    {
        Log.Debug("ChatBot.OnDeleteSession()", "Session deleted.");
        deleteSessionTested = true;
    }

    private void OnCreateSession(DetailedResponse<SessionResponse> response, IBMError error)
    {
        Log.Debug("ChatBot.OnCreateSession()", "Session: {0}", response.Result.SessionId);
        sessionId = response.Result.SessionId;
        createSessionTested = true;
    }

    //select* Methods for accessing the Characters Animator and Triggering the Animation

    public void selectSophie ()
    {
        Sophie.GetComponent<SophieMovement>().enabled = true;
        Sophie.GetComponent<Animator>().Play("Walking");
    }

    public void selectJody ()
    {
        Jody.GetComponent<JodyMovement>().enabled = true;
        Jody.GetComponent<Animator>().Play("Walking");
    }

    public void selectDavid ()
    {
        David.GetComponent<DavidMovement>().enabled = true;
        David.GetComponent<Animator>().Play("HappyWalk");
    }

    void buttonsAppear () {
        restartButton.SetActive(true);
        exitButton.SetActive(true);
    }
}
