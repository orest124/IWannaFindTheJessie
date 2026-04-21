using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Meny : MonoBehaviour
{

    [SerializeField]Movement pl;
    
    [SerializeField] Image GlobalFone;
    [SerializeField] GameObject MenyThem;
    [SerializeField] GameObject PreMenyThem;
    [SerializeField] GameObject LoadingThem;

    [SerializeField] Image[] Thems;
    [SerializeField] TextMeshProUGUI[] texts;
    [SerializeField] GameObject OptionsThem;
    
    [SerializeField]MusicThemeControler music;
    [SerializeField]P_SoundAndPhoto player;
    [SerializeField] float spd;
    void Awake()
    {
        StartCoroutine(Loading(3,PreMenyThem, MenyThem));
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !pl.OptionsOpen)     InOptions(true);
        else if(Input.GetKeyDown(KeyCode.Escape) && pl.OptionsOpen) InOptions(false);
        
    }
    public void Play() {
        StartCoroutine(Loading(0, MenyThem, _action: () => pl.MenuMod(false)));
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
    IEnumerator Loading(float timer, GameObject one = null, GameObject two = null, Action _action = null)
    {
        yield return new WaitForSeconds(timer);

        float t = 0;
        while (t < 1)
        {
            t += Time.fixedDeltaTime * spd*5f;
            Color c = GlobalFone.color;
            GlobalFone.color = new Color(c.r,c.g,c.b,t);
            yield return null;
        }
        one?.SetActive(false);
        two?.SetActive(true);
        
        yield return new WaitForSeconds(1);
        t = 1;
        while (t > 0.02)
        {
            t -= Time.fixedDeltaTime * spd;
            
            Color c = GlobalFone.color;
            GlobalFone.color = new Color(c.r,c.g,c.b,t);
            
            yield return null;
        }
        _action?.Invoke();
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
