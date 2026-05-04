using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Meny : MonoBehaviour
{

    private Movement pl;
    public JsonConfig config;
    private SaveSystem _save;

    private MusicThemeControler music;
    private P_SoundAndPhoto p_Sound;
    private SoundControler sound;
    public float globalVolumeMod;
    public float musicVolumeMod;
    public float soundVolumeMod;


    [SerializeField] bool DontNidMeny;
    [SerializeField] float premenyStayTime;
    [SerializeField] float spd;

    [SerializeField] Image GlobalFone;





    [Header("Continye Page")]
    [SerializeField] GameObject ContOn;
    [SerializeField] GameObject ContOff;

    void Awake()
    {
        pl = FindAnyObjectByType<Movement>(); pl.SetMeny(this);
        _save = FindAnyObjectByType<SaveSystem>();

        music = FindAnyObjectByType<MusicThemeControler>();
        p_Sound = pl.s;
        sound = FindAnyObjectByType<SoundControler>();
    }

    void Start()
    {
        config = _save.GetConfig();
        Preparation();
        
        if(DontNidMeny) return;
        GlobalFone.gameObject.SetActive(true);
        StartCoroutine(PreMenuLoading());
        
    }
    [Header("Volume Page")]
    [SerializeField] GameObject[] Buttons;
    private void Preparation()
    {
        AllVolumeState(config.globalVolumeMod);
        MusicState(config.musicVolumeMod);
        JoyType(config.JoysticTipe);

        bool state = _save.CheckEmpty();
        ContOn.SetActive(!state); ContOff.SetActive(state);
        

    }

    
    
    public void Load()
    {
        _save.LoadAllData();
        Play();
    } 
    public void Save() => pl.memory.SaveState();

    public void RestartGame() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    public void RemoveData() => _save.RemoveAllMemory();

    [Header("Game Options Page")]
    [SerializeField] GameObject GameOptionsButton;
    [SerializeField] GameObject GameOptionsSetingButton;
    public void InOptions(bool state) {
        pl.SetStop(state);
        OptionsThem.SetActive(state);
        pl.OptionsOpen = state;
        if(!state)
        {
            GameOptionsSetingButton.SetActive(false);
            GameOptionsButton.SetActive(true);
        }

    }

    public void JoyType(bool tipe) 
    {
        config.JoysticTipe = tipe;
        pl.JoysticMod = tipe;

        Buttons[8].SetActive(  tipe); 
        Buttons[9].SetActive(  tipe); 
        Buttons[10].SetActive(!tipe);
        Buttons[11].SetActive(!tipe);
    }

    public void MusicState(bool state) {
        config.musicVolumeMod = state;
        music.LovalModVolume = state? 1:0;
        music.SetVolume();

        Buttons[4].SetActive( state); 
        Buttons[5].SetActive( state); 
        Buttons[6].SetActive(!state);
        Buttons[7].SetActive(!state);

    }
    public void AllVolumeState(bool state) {
        config.globalVolumeMod = state;
        music.GlobalModVolume = state? 1:0;
        p_Sound.GlobalModVolume = state? 1:0;
        sound.GlobalModVolume = state? 1:0;
        music.SetVolume();
        p_Sound.SetVolume();
        sound.SetVolume();

        Buttons[0].SetActive( state); 
        Buttons[1].SetActive( state); 
        Buttons[2].SetActive(!state);
        Buttons[3].SetActive(!state);

    }

    
    [Header("Global Page")]
    [SerializeField] GameObject MenyButtons;
    [SerializeField] GameObject PlayButtons;
    public void ToPlayMeny() =>
        NextPaje(() => MenyButtons.SetActive(false), () => PlayButtons.SetActive(true));
        public void WithPlayToMeny() =>
        NextPaje(() => MenyButtons.SetActive(true), () => PlayButtons.SetActive(false));






    IEnumerator PreMenuLoading() {
        
        pl.SetStop(true);
        float u = 0;
        float t = 1;
        PreMenyTheme?.SetActive(true);
        yield return timer1;
        while (t > 0.02)
        {
            t -= Time.fixedDeltaTime * spd * 3;
            GlobalFone.color = new Color(GlobalFone.color.r,GlobalFone.color.g,GlobalFone.color.b,t);
            yield return fixTime;
        }
        while(u < premenyStayTime)
        {
            u += Time.fixedDeltaTime * spd;
            if(Input.anyKey) 
            {
                break;
            }
            yield return fixTime;
        }
        while (t < 1) {
            t += Time.fixedDeltaTime * spd * 3f;
            GlobalFone.color = new Color(GlobalFone.color.r,GlobalFone.color.g,GlobalFone.color.b,t);
            yield return fixTime;
        }
        PreMenyTheme?.SetActive(false);
        MenyThem?.SetActive(true);
        t = 1;
        while (t > 0.02)
        {
            t -= Time.fixedDeltaTime * spd * 3;
            GlobalFone.color = new Color(GlobalFone.color.r,GlobalFone.color.g,GlobalFone.color.b,t);
            yield return fixTime;
        }

    }
    [Header("PreMeny Page")]
    [SerializeField] GameObject PreMenyTheme;
    [SerializeField] GameObject MenyThem;



WaitForSeconds time05 = new WaitForSeconds(0.3f);
    IEnumerator BlackFon(Action action, Action action2)
    {
        float t = 0;
        while (t < 1) {
            t += Time.fixedDeltaTime * spd * 2;
            GlobalFone.color = new Color(GlobalFone.color.r,GlobalFone.color.g,GlobalFone.color.b,t);
                yield return fixTime;

        }

        action?.Invoke();
        action2?.Invoke();

        t = 1;
        while (t > 0.02)
        {
            t -= Time.fixedDeltaTime * spd * 2;
            GlobalFone.color = new Color(GlobalFone.color.r,GlobalFone.color.g,GlobalFone.color.b,t);
                yield return fixTime;

        }
    }
    public void NextPaje(Action action, Action action2) => StartCoroutine(BlackFon(action,action2));




    IEnumerator Loading() {

        _save.SaveConfig(config);
        LoadingText.text = _save.GetRandomContext();
        float t = 0;
        while (t < 1) {
            t += Time.fixedDeltaTime * spd * 2f;
            GlobalFone.color = new Color(GlobalFone.color.r,GlobalFone.color.g,GlobalFone.color.b,t);
                yield return fixTime;

        }
        MenyThem?.SetActive(false);
        LoadingThem?.SetActive(true);

        t = 1;
        while (t > 0.02)
        {
            t -= Time.fixedDeltaTime * spd * 2;
            GlobalFone.color = new Color(GlobalFone.color.r,GlobalFone.color.g,GlobalFone.color.b,t);
                yield return fixTime;

        }

        yield return new WaitForSeconds(loadingTime); // очікуєм кінця загрузки

        t = 0;
        while (t < 1) {
            t += Time.fixedDeltaTime * spd * 2f;
            GlobalFone.color = new Color(GlobalFone.color.r,GlobalFone.color.g,GlobalFone.color.b,t);
            yield return fixTime;
        }
        LoadingThem?.SetActive(false);
        yield return time05;
        pl.SetStop(false);
        t = 1;
        while (t > 0.02)
        {
            t -= Time.fixedDeltaTime * spd * 2;
            GlobalFone.color = new Color(GlobalFone.color.r,GlobalFone.color.g,GlobalFone.color.b,t);
            yield return fixTime;
        }
    }
    [Header("Loading Page")]
    [SerializeField] GameObject LoadingThem;
    [SerializeField] GameObject OptionsThem;
    [SerializeField] TextMeshProUGUI LoadingText;
    [SerializeField] float loadingTime;
    WaitForSeconds timer1 = new WaitForSeconds(1);

    public void Play() => StartCoroutine(Loading());


    


WaitForFixedUpdate fixTime = new WaitForFixedUpdate();
    IEnumerator ExitGame()
    {
        _save.SaveConfig(config);
        Image.sprite = ExitImage[0];
        ExitThem.SetActive(true);
        yield return new WaitForSeconds(ExitFrameTime);
        yield return fixTime;
        foreach (var f in ExitImage)
        {
            Image.sprite = f;

            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        Application.Quit();
        ExitThem.SetActive(false);
    }
    [Header("Exit Theme")]
    [SerializeField] GameObject ExitThem;
    [SerializeField] Image Image;
    [SerializeField] Sprite[] ExitImage;
    [SerializeField] float ExitFrameTime;
    public void Exit() => StartCoroutine(ExitGame());
}
public class JsonConfig : Entyty
{
    public bool JoysticTipe;
    public bool globalVolumeMod;
    public bool musicVolumeMod;
    public bool Fun;
}