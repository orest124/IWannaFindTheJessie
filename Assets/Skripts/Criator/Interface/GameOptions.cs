using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.IO;

public class GameOptions : MonoBehaviour {
    [SerializeField] bool DontNidMeny;
    [SerializeField] bool inLavel;
    [SerializeField] bool _loadData;
    private MusicThemeControler music;
    private SoundControler sound;
    private P_SoundAndPhoto p_Sound;
    private SaveSystem _save;
    Movement pl;

    private JsonConfig config;
    
    private const string pathToConfig = "/Configuration.json";
    private const string pathToUAContext = "UAContext.json";
    private const string pathToCharacter = "/Saves/Player Memory.json";

    void Awake()
    {
        music = FindAnyObjectByType<MusicThemeControler>();
        sound = FindAnyObjectByType<SoundControler>();
        sound.SetOnline(false);
        

        GetContext();
        LoadConfig();

        _save = GetComponent<SaveSystem>();

        pl = FindAnyObjectByType<Movement>(); pl.meny = this; pl.SetMusic(music); p_Sound = pl.s;
        Preparation();
    }

    void Start()
    {
        if(inLavel) pl.inLavel = true;
        if(DontNidMeny) loadingTime = 0;
        
        GlobalFone.gameObject.SetActive(true);
        pl.SetStop(true);
        Play();
         
    }
    private void Preparation()
    {
        AllVolumeState(config.globalVolumeMod);
        MusicState(config.musicVolumeMod);
        JoyType(config.JoysticType);
        MoveType(config.MoveType);
    }
    public void MoveType(bool isSakaban) 
    {
        pl.MotionMod = isSakaban;
    }
    public void JoyType(bool tipe) 
    {
        Buttons[4].SetActive(    tipe); 
        Buttons[5].SetActive(   !tipe); 
        config.JoysticType     = tipe;
        pl.JoysticMod          = tipe;
    }
    public void MusicState(bool state) {
        config.musicVolumeMod = state;
        music.LovalModVolume  = state? 1:0;
        Buttons[2].SetActive(   state); 
        Buttons[3].SetActive(  !state); 

        music.SetVolume();

    }
    public void AllVolumeState(bool state) {
        config.globalVolumeMod    = state;
        music.GlobalModVolume     = state? 1:0;
        sound.GlobalModVolume     = state? 1:0;
        p_Sound.GlobalModVolume   = state? 1:0;
        Buttons[0].SetActive(       state); 
        Buttons[1].SetActive(      !state); 

        music.  SetVolume();
        sound.  SetVolume();
        p_Sound.SetVolume();

    }
    public void InOptions(bool state) {
        pl.SetStop(state);
        OptionsThem.SetActive(state);
        pl.OptionsOpen = state;
        if(!state)
        {
            SaveConfig();
            GameOptionsSetingButton.SetActive(false);
            GameOptionsButton.SetActive(true);
        }
    }
    public void Load()
    {
        SetContinueValue(false);
        _save.LoadAllData();
    } 
    public void ReloadScene()
    {
        SetContinueValue(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    } 
    
    public void Play() => StartCoroutine(Loading());
    public void ToMeny() => StartCoroutine(SceneLoading(0));
    public void Save() => _save.SaveState();


    string[] context;
    private void GetContext()
    {
        context = File.ReadAllLines(Application.dataPath + "/" + pathToUAContext);
    }
    public string GetRandomContext()
    {
        int i = Random.Range(0, context.Length);
        return context[i];
    }
        public bool CheckEmpty()
    {
        if(!File.Exists(Application.dataPath + "/" + pathToCharacter))
        {
            Directory.CreateDirectory(Application.dataPath + "/Saves");
            File.WriteAllText(Application.dataPath + "/" + pathToCharacter, string.Empty);
            return true;
        }
        var stateJson = File.ReadAllText(Application.dataPath + "/" + pathToCharacter);
        if(stateJson == string.Empty) return true;
        else return false;
    }
    public void SaveConfig()
    {
        string save = JsonConvert.SerializeObject(config);
        File.WriteAllText(Application.dataPath + "/" + pathToConfig, save);
    }
    
    private void LoadConfig()
    {
        config = new();
        if ( !File.Exists(Application.dataPath + "/" + pathToConfig))
        {
            SaveConfig();
            return;
        }
        var save = File.ReadAllText(Application.dataPath + "/" + pathToConfig);
        try { config = JsonConvert.DeserializeObject<JsonConfig>(save); }
        catch { config = new(); }
    }
    private void SetContinueValue(bool state)
    {
        config.Continue = state;
        SaveConfig();
    }    
    public void SaveSceneIndex()
    {
        config.L_I_N_T_P_B_A_C = SceneManager.GetActiveScene().buildIndex;
        SaveConfig();
    }
    [Header("Volume Page")]
    [SerializeField] GameObject[] Buttons;

        IEnumerator Loading() {

        float t = 0;
        LoadingText.text = GetRandomContext();
        LoadingThem?.SetActive(true);

        t = 1;
        while (t > 0.02)
        {
            t -= Time.fixedDeltaTime * spd;
            GlobalFone.color = new Color(GlobalFone.color.r,GlobalFone.color.g,GlobalFone.color.b,t);
                yield return fixTime;

        }
        if(_loadData)
        {
            SetContinueValue(true);
        }
        if(config.Continue) Load();
        pl.memory.SetStats();
        yield return new WaitForSeconds(loadingTime); // очікуєм кінця загрузки

        t = 0;
        while (t < 1) {
            t += Time.fixedDeltaTime * spd;
            GlobalFone.color = new Color(GlobalFone.color.r,GlobalFone.color.g,GlobalFone.color.b,t);
            yield return fixTime;
        }
        LoadingThem?.SetActive(false);
        yield return time05;
        pl.SetStop(false);
        t = 1;
        while (t > 0.02)
        {
            t -= Time.fixedDeltaTime * spd;
            GlobalFone.color = new Color(GlobalFone.color.r,GlobalFone.color.g,GlobalFone.color.b,t);
            yield return fixTime;
        }
        sound.SetOnline();


    }
    IEnumerator SceneLoading(int scene) 
    {
        float t = 0;
        while (t < 1) {
            t += Time.fixedDeltaTime * spd;
            GlobalFone.color = new Color(GlobalFone.color.r,GlobalFone.color.g,GlobalFone.color.b,t);
                yield return fixTime;
        }
        SceneManager.LoadScene(scene);
    }
    public void BlackFon(Action a) => StartCoroutine(BlackFonProces(a));
    [SerializeField] float BlackSpd;
    [SerializeField] float BlackPause;
    IEnumerator BlackFonProces(Action action)
    {
        float t = 0;
        while (t < 1) {
            t += Time.fixedDeltaTime * BlackSpd;
            GlobalFone.color = new Color(GlobalFone.color.r,GlobalFone.color.g,GlobalFone.color.b,t);
                yield return fixTime;
        }
        action.Invoke();
        yield return new WaitForSeconds(BlackPause);
        t = 1;
        while (t > 0)
        {
            t -= Time.fixedDeltaTime * BlackSpd;
            GlobalFone.color = new Color(GlobalFone.color.r,GlobalFone.color.g,GlobalFone.color.b,t);
                yield return fixTime;

        }
    }

    [SerializeField] float spd;
    [SerializeField] float loadingTime;
    [SerializeField] Image GlobalFone;
    WaitForSeconds time05 = new WaitForSeconds(0.5f);
    WaitForFixedUpdate fixTime = new WaitForFixedUpdate();

    [Header("Game Options Page")]
    [SerializeField] GameObject OptionsThem;
    [SerializeField] GameObject GameOptionsButton;
    [SerializeField] GameObject GameOptionsSetingButton;
    
    [Header("Loading Page")]
    [SerializeField] GameObject LoadingThem;
    [SerializeField] TMPro.TextMeshProUGUI LoadingText;

}
