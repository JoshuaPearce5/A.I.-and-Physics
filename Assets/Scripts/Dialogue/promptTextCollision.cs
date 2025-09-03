using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class promptTextCollision : MonoBehaviour
{
    public TextMeshProUGUI promptText;

    private float fadeTime;
    private bool fadingIn;

    private bool textCollision;

    void Start()
    {
        promptText.CrossFadeAlpha(0, 0.0f, false);
        fadeTime = 0;
        fadingIn = false;
        textCollision = false;
    }

    void Update()
    {
        if (fadingIn)
        {
            FadeIn();
        }
        else if (promptText.color.a != 0)
        {
            promptText.CrossFadeAlpha(0, 0.25f, false);
        }
    }

    void FadeIn()
    {
        promptText.CrossFadeAlpha(1, 0.25f, false);

        fadeTime += Time.deltaTime;

        if (!textCollision && promptText.color.a == 1 && fadeTime > 3f)
        {
            fadingIn = false;
            fadeTime = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            fadingIn = true;
            textCollision = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            textCollision = false;
        }
    }
}