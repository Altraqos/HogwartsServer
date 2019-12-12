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
    public Text sText;
    public float colorRed;
    public float colorGreen;
    public float colorBlue;
    public float colorAlpha;
    public float spellDuration = 2;
    private KeywordRecognizer kRecognizer;
    private Dictionary<string, Action> actions = new Dictionary<string, Action>();

    public void OnEnable()
    {
        actions.Add("kedavra", AvadaKedavra);
        actions.Add("crucio", Crucio);
        actions.Add("aguamenti", Aguamenti);
        actions.Add("alohomora", Alohomora);
        actions.Add("reducto", Reducto);
        kRecognizer = new KeywordRecognizer(actions.Keys.ToArray());
        kRecognizer.OnPhraseRecognized += recognizedSpeech;
        kRecognizer.Start();
    }

    private void recognizedSpeech(PhraseRecognizedEventArgs speech)
    {
        sText.text = speech.text;
        actions[speech.text].Invoke();
    }

    private void AvadaKedavra()
    {
        StopCoroutine("spellEffect");
        colorRed = 0;
        colorGreen = 255;
        colorBlue = 0;
        colorAlpha = 255;
        StartCoroutine("spellEffect");
    }

    private void Crucio()
    {
        StopCoroutine("spellEffect");
        colorRed = 210;
        colorGreen = 0;
        colorBlue = 0;
        colorAlpha = 255;
        StartCoroutine("spellEffect");
    }

    private void Aguamenti()
    {
        StopCoroutine("spellEffect");
        colorRed = 0;
        colorGreen = 50;
        colorBlue = 210;
        colorAlpha = 255;
        StartCoroutine("spellEffect");
    }

    private void Alohomora()
    {
        StopCoroutine("spellEffect");
        colorRed = 255;
        colorGreen = 255;
        colorBlue = 0;
        colorAlpha = 255;
        StartCoroutine("spellEffect");
    }

    private void Reducto()
    {
        StopCoroutine("spellEffect");
        colorRed = 224;
        colorGreen = 10;
        colorBlue = 10;
        colorAlpha = 255;
        StartCoroutine("spellEffect");
    }

    public IEnumerator spellEffect()
    {
        var main = pSystem.main;
        main.startColor = new Color(colorRed, colorGreen, colorBlue, colorAlpha);
        pSystem.Play();
        yield return new WaitForSecondsRealtime(spellDuration);
        pSystem.Stop();
        StopCoroutine("spellEffect");
    }
}
