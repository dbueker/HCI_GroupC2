using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reload : MonoBehaviour
{
    public GameObject gm;
    public GameObject button;

    void Start ()
    {
        button.SetActive(false);
    }

    public void ReloadScene ()
    {
        gm.GetComponent<GameManager>().ReloadScene();
    }
}
