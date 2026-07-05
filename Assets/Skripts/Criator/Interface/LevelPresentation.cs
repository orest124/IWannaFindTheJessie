using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelPresentation{
    MonoBehaviour o;
    TextMeshProUGUI text;
    Image fon;
    WaitForFixedUpdate fix;
    
    public LevelPresentation(MonoBehaviour _o, TextMeshProUGUI _text, Image _fon)
    {
        o = _o;
        text = _text;
        fon = _fon;
        fix = new WaitForFixedUpdate();
    }
    public Coroutine StartPresentation(float duration = 1, float pause = 0) => o.StartCoroutine(StartPresent(duration, pause));
    IEnumerator StartPresent(float duration, float pause)
    {
        int SkaleAlpha = 50;
        float t = 0;
        float t2 = 0;
        Color c = fon.color;
        Color tc = text.color;
        while (t < duration) 
        {
            t += duration * Time.fixedDeltaTime;
            t2 += duration * 0.01f * SkaleAlpha * Time.fixedDeltaTime;
            float n = t / duration;
            float nf = t2 / duration;
            fon.color = new Color( c.r, c.g, c.b, nf );
            text.color = new Color( tc.r, tc.g, tc.b, n );
            yield return fix;
        }

        yield return new WaitForSeconds(pause);

        t = duration;
        t2 = duration * 0.01f * SkaleAlpha;
        while (t > 0)
        {
            t -= duration * Time.fixedDeltaTime;
            t2 -= duration * 0.01f * SkaleAlpha * Time.fixedDeltaTime;
            float n = t / duration;
            float nf = t2 / duration;
            fon.color = new Color( c.r, c.g, c.b, nf );
            text.color = new Color( tc.r, tc.g, tc.b, n );
            
            yield return fix;

        }   
    }
}