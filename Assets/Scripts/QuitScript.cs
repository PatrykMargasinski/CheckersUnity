using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitScript : MonoBehaviour
{
    void Start()
    {
        //gameObject.SetActive(false);
    }
    public void Quit()
    {
        Application.Quit();
    }
}
