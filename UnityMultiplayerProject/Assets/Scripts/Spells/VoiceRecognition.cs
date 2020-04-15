using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class VoiceRecognition : MonoBehaviour
{
    public ParticleSystem pSystem;
    //public Text sText;
    public float cRed;
    public float cGreen;
    public float cBlue;
    public float cAlpha;
    public float sDuration = 2;
    private KeywordRecognizer kRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    public void OnEnable()
    {
		pSystem.Stop();
        actions.Add("kedavra", AvadaKedavra);
        actions.Add("crucio", Crucio);
        actions.Add("aquamenti", Aguamenti);
        actions.Add("alohomora", Alohomora);
        actions.Add("reducto", Reducto);
        kRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        kRecognizer.OnPhraseRecognized += recognizedSpeech;
        kRecognizer.Start();
    }

    private void recognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        //sText.text = speech.text;
        Debug.Log(speech.text);
        actions[speech.text].Invoke();
    }

    private void DefaultSpellFunction()
    {
        StopCoroutine("spellEffect");
        cRed = 255;
        cGreen = 255;
        cBlue = 255;
        cAlpha = 255;

        /*
         * Add the main spell functionality in here.
         * Set the colours above to whatever the spell needs to be, add the real spell underneath.
         * And finally start the spell coroutine.
         */

        StartCoroutine("spellEffect");
    }
    
    private void AvadaKedavra()
    {
        StopCoroutine("spellEffect");
        cRed = 0;
        cGreen = 255;
        cBlue = 0;
        cAlpha = 255;



        StartCoroutine("spellEffect");
    }

    private void Crucio()
    {
        StopCoroutine("spellEffect");
        cRed = 210;
        cGreen = 0;
        cBlue = 0;
        cAlpha = 255;



        StartCoroutine("spellEffect");
    }

    private void Aguamenti()
    {
        StopCoroutine("spellEffect");
        cRed = 0;
        cGreen = 50;
        cBlue = 210;
        cAlpha = 255;



        StartCoroutine("spellEffect");
    }

    private void Alohomora()
    {
        StopCoroutine("spellEffect");
        cRed = 255;
        cGreen = 255;
        cBlue = 0;
        cAlpha = 255;



        StartCoroutine("spellEffect");
    }

    private void Reducto()
    {
        StopCoroutine("spellEffect");
        cRed = 224;
        cGreen = 10;
        cBlue = 10;
        cAlpha = 255;



        StartCoroutine("spellEffect");
    }

    public IEnumerator spellEffect()
    {
        var main = pSystem.main;
        main.startColor = new Color(cRed, cGreen, cBlue, cAlpha);
        pSystem.Play();
        yield return new WaitForSecondsRealtime(sDuration);
        pSystem.Stop();
        StopCoroutine("spellEffect");
    }
}
