using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Fade : MonoBehaviour
{
    private CanvasGroup uigroup;
    private float timer;

    private bool fade = false;

    private void Awake()
    {
        uigroup = gameObject.GetComponent<CanvasGroup>();
        timer = 5;
    }
    public void HideUI()
    {
        fade = true;
    }

    private void Update()
    {
        if (fade)
        {
            if (uigroup.alpha > 0)
            {
                uigroup.alpha -= Time.deltaTime;
            }
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        else
        {
            HideUI();
        }
    }

}
