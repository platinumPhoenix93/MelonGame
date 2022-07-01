using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextFadeScript : MonoBehaviour
{

    private GameObject player;
    private TextMeshPro text;


    void Start()
    {
        player = GameObject.Find("Player");
        text = GetComponent<TextMeshPro>();

    }


    private void FixedUpdate()
    {
        //Get distance from text to player
        float dist = Vector2.Distance(player.transform.position, transform.position);
        Debug.Log("Distance to player: " + dist);

        //Fade text in if close
        if (dist < 3)
        {
            StartCoroutine(FadeIn());
        }
        //Fade text out if far
        else
        {
            StartCoroutine(FadeOut());
        }
    }

    //Changes text alpha from 0 to max over 1 second
    IEnumerator FadeIn()
    {
        while(text.color.a < 1)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (1 * Time.deltaTime));
            yield return null;
        }

        yield break;
    }


    //Changes text alpha from max to 0 over 1 second
    IEnumerator FadeOut()
    {
        while (text.color.a > 0)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (1 * Time.deltaTime));
            yield return null;
        }

        yield break;
    }
}
