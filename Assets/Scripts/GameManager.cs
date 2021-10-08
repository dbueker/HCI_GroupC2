using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public ChatBot chatbot;
    public SpeechToText speechToText;
    public GameObject Sophie;
    public GameObject Jody;
    public GameObject David;

    void Start()
    {
        Invoke("triggerAssistant", 2);

        // Disable the coach's Scripts to prevent the Animations from starting
        Sophie.GetComponent<SophieMovement>().enabled = false;
        Jody.GetComponent<JodyMovement>().enabled = false;
        David.GetComponent<DavidMovement>().enabled = false;
    }

    void triggerAssistant ()
    {
        // Trigger the Watson Assistant and mute SpeechToText to prevent Assistant from recording itself
        speechToText.Active = false;
        chatbot.sendMessage("Start");
    }

    public void ReloadScene ()
    {
        SceneManager.LoadScene("ExampleConversationalAgent");
    }

    public void EndGame ()
    {
        Application.Quit();
    }

}
