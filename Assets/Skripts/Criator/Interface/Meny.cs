using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Meny : MonoBehaviour
{
    [SerializeField]Movement pl;
    [SerializeField] GameObject LoadingThem;
    [SerializeField] Image[] Thems;
    [SerializeField] TextMeshProUGUI[] texts;
    [SerializeField] GameObject MenyThem;
    [SerializeField] GameObject OptionsThem;
    
    [SerializeField]MusicThemeControler music;
    [SerializeField]P_SoundAndPhoto player;
    [SerializeField] float spd;
    
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !pl.OptionsOpen)     InOptions(true);
        else if(Input.GetKeyDown(KeyCode.Escape) && pl.OptionsOpen) InOptions(false);
        
    }
    public void Play() {
        StartCoroutine(Loading());
    }
    public void InOptions(bool state) {
        pl.SetStop(state);
        OptionsThem.SetActive(state);
        pl.OptionsOpen = state;

    }
    public void MusicState(bool state) {
        music.gameObject.SetActive(state);
    }
    public void AllMusicState(bool state) {
        music.gameObject.SetActive(state);
        player.stepVolume = state? 0.25f:0;
    }
    public void Exit() {

        StartCoroutine(ExitGame());
    }
    IEnumerator Loading()
    {
        LoadingThem.SetActive(true);
        float t = 0;
        while (t < 1)
        {
            t += Time.fixedDeltaTime * spd*5f;
            foreach (var i in texts)
            {
                Color c = i.color;
                i.color = new Color(c.r,c.g,c.b,t);
            }
            foreach (var i in Thems)
            {
                Color c = i.color;
                i.color = new Color(c.r,c.g,c.b,t);
            }
            yield return null;
        }
        MenyThem.SetActive(false);
        
        yield return new WaitForSeconds(4);
        t = 1;
        while (t > 0.02)
        {
            t -= Time.fixedDeltaTime * spd;
            foreach (var i in texts)
            {
                Color c = i.color;
                i.color = new Color(c.r,c.g,c.b,t);
            }
            foreach (var i in Thems)
            {
                Color c = i.color;
                i.color = new Color(c.r,c.g,c.b,t);
            }
            yield return null;
        }
        LoadingThem.SetActive(false);
        foreach (var i in texts)
        {
            Color c = i.color;
            i.color = new Color(c.r,c.g,c.b,1);
        }
        foreach (var i in Thems)
        {
            Color c = i.color;
            i.color = new Color(c.r,c.g,c.b,1);
        }

        pl.MenuMod(false);
    }
    [Header("Exit Theme")]
    [SerializeField] GameObject ExitThem;
    [SerializeField] Image EThem;
    [SerializeField] Sprite[] ExitImage;
    [SerializeField] float ExitFrameTime;
    IEnumerator ExitGame()
    {
        EThem.sprite = ExitImage[0];
        ExitThem.SetActive(true);
        yield return new WaitForSeconds(ExitFrameTime);
        foreach (var f in ExitImage)
        {
            EThem.sprite = f;

            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        Application.Quit();
        ExitThem.SetActive(false);
    }
}
