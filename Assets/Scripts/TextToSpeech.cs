/**
* Copyright 2020 IBM Corp. All Rights Reserved.
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

using IBM.Watson.TextToSpeech.V1;
using IBM.Watson.TextToSpeech.V1.Model;
using IBM.Cloud.SDK.Utilities;
using IBM.Cloud.SDK.Authentication;
using IBM.Cloud.SDK.Authentication.Iam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using IBM.Cloud.SDK;

public class TextToSpeech : MonoBehaviour
{
    [Tooltip("Copy apikey from service credentials that are generated for your IBM cloud account.")]
    [SerializeField]
    private string iamApikey;
    [Tooltip("Copy url from service credentials that are generated for your IBM cloud account.")]
    [SerializeField]
    private string serviceUrl;
    [Tooltip("GameObject with SpeechToText component to restart recording when the chat bot stopped talking.")]
    public SpeechToText stt;
    [Tooltip("Audio source for playing the audio clip [optional].")]
    public AudioSource audioSource;

    private TextToSpeechService service;
    private string allisionVoice = "en-US_LisaV3Voice"; // alternative: "en-US_AllisonV3Voice"
    private string synthesizeMimeType = "audio/wav";
    private AudioClip lastSynthesizedClip;

    private void Start()
    {
        if(!audioSource) {
            audioSource = this.gameObject.AddComponent<AudioSource>();
        }
        LogSystem.InstallDefaultReactors();
        Runnable.Run(CreateService());
    }

    public IEnumerator sendMessage(List<AgentResponse> responses)
    {

        foreach(AgentResponse response in responses)
        {
            if(System.IO.File.Exists(Application.dataPath + "/Resources/" + response.fileName + ".wav"))
            {
                AudioClip clip = Resources.Load<AudioClip>(response.fileName);
                yield return PlayClip(clip);
            }
            else 
            {
                yield return Synthesize(response.text, Application.dataPath + "/Resources/" + response.fileName + ".wav");
                //AudioClip clip = Resources.Load<AudioClip>(response.fileName);
       
                yield return PlayClip(lastSynthesizedClip);
            }
        }
        
        Log.Debug("TextToSpeech", "Response received!");
        
        stt.Active = true;
    }

    private IEnumerator CreateService()
    {
        if (string.IsNullOrEmpty(iamApikey))
        {
            throw new IBMException("Please add IAM ApiKey to the Iam Apikey field in the inspector.");
        }

        IamAuthenticator authenticator = new IamAuthenticator(apikey: iamApikey);

        while (!authenticator.CanAuthenticate())
        {
            yield return null;
        }

        service = new TextToSpeechService(authenticator);
        if (!string.IsNullOrEmpty(serviceUrl))
        {
            service.SetServiceUrl(serviceUrl);
        }
    }

    private void OnError(string error)
    {
        Log.Debug("TextToSpeech.OnError()", "Error! {0}", error);
    }

    private IEnumerator Synthesize(string synthesizeText, string fileName)
        {
            byte[] synthesizeResponse = null;
            AudioClip clip = null;
            service.Synthesize(
                callback: (DetailedResponse<byte[]> response, IBMError error) =>
                {
                    synthesizeResponse = response.Result;
                    Log.Debug("TextToSpeech", "Synthesize done!");
                    clip = WaveFile.ParseWAV(fileName, synthesizeResponse);
                    lastSynthesizedClip = clip;
                    SavWav.Save(fileName, clip);
                    PlayClip(clip);
                },
                text: synthesizeText,
                voice: allisionVoice,
                accept: synthesizeMimeType
            );
            while (synthesizeResponse == null)
                yield return null;
        }

    public IEnumerator PlayClip(AudioClip clip)
    {
        if (Application.isPlaying && clip != null)
        {
            audioSource.spatialBlend = 0.0f;
            audioSource.loop = false;
            audioSource.clip = clip;
            audioSource.Play();

            yield return WaitForClipEnd(audioSource);
        }

        yield break;
    }

    private IEnumerator WaitForClipEnd(AudioSource source)
    {
        while (source.isPlaying)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1);

        source.clip = null;
    }
}
