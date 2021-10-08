using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour
{
    public GameObject gm;
    public GameObject button;

    void Start ()
    {
        button.SetActive(false);
    }

    public void EndGame ()
    {
        gm.GetComponent<GameManager>().EndGame();
    }
}
