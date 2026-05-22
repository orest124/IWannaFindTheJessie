using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.IO;

public class Meny : MonoBehaviour
{
    private const string pathToConfig = "/Configuration.json";
    private const string pathToCharacter = "/Saves/Player Memory.json";


    public JsonConfig config;


    private SaveSystem _save;

    public MusicThemeControler music {get; private set;}
    private SoundControler sound;
    
    [SerializeField] float premenyStayTime;
    [SerializeField] float spd;

    [SerializeField] Image GlobalFone;




    void Awake()
    {
        GlobalFone.gameObject.SetActive(true);
        music = FindAnyObjectByType<MusicThemeControler>();
        sound = FindAnyObjectByType<SoundControler>();
        _save = GetComponent<SaveSystem>();
        LoadConfig();
    }

    void Start()
    {
        Preparation();
        //завести булл на те чи показувати пре меню
        StartCoroutine(PreMenuLoading());
        
         
    }



    public void RemoveData() => _save.RemoveAllMemory();
    public void NewGameButton(int scene)
        { SetContinueValue(false); StartCoroutine(SceneLoading(scene)); }
    public void ContinueButton()
        { SetContinueValue(true); StartCoroutine(SceneLoading(config.L_I_N_T_P_B_A_C)); }


    IEnumerator SceneLoading(int scene) 
    {
        float t = 0;
        while (t < 1) {
            t += Time.fixedDeltaTime * spd * 2f;
            GlobalFone.color = new Color(GlobalFone.color.r,GlobalFone.color.g,GlobalFone.color.b,t);
                yield return fixTime;
        }
        SceneManager.LoadScene(scene);
    }



    public void JoyType(bool tipe) 
    {
        Buttons[4].SetActive(    tipe); 
        Buttons[5].SetActive(   !tipe); 
        config.JoysticTipe     = tipe;
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
        Buttons[0].SetActive(       state); 
        Buttons[1].SetActive(      !state); 

        music.SetVolume();
        sound.SetVolume();
    }


    public void ToPlayMeny() =>
        NextPaje(() => MenyButtons.SetActive(false), () => PlayButtons.SetActive(true));
    public void WithPlayToMeny() =>
        NextPaje(() => MenyButtons.SetActive(true), () => PlayButtons.SetActive(false));

    public void ToSeting() =>
        NextPaje(() => MenyButtons.SetActive(false), () => SetingButtons.SetActive(true));
    public void WithSeting()
    {
        SaveConfig();   
        NextPaje(() => MenyButtons.SetActive(true), () => SetingButtons.SetActive(false));
    }




    IEnumerator PreMenuLoading() {
        
        float u = 0;
        float t = 1;
        PreMenyTheme.SetActive(true);
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
        PreMenyTheme.SetActive(false);
        MenyThem.SetActive(true);
        t = 1;
        while (t > 0.02)
        {
            t -= Time.fixedDeltaTime * spd * 3;
            GlobalFone.color = new Color(GlobalFone.color.r,GlobalFone.color.g,GlobalFone.color.b,t);
            yield return fixTime;
        }

    }



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


    WaitForSeconds timer1 = new WaitForSeconds(1);
    WaitForFixedUpdate fixTime = new WaitForFixedUpdate();
    IEnumerator ExitGame()
    {
        SaveConfig();
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
    



    
    private void Preparation()
    {
        //Нехай кнопка контінуе міняється залежно від сейва
        AllVolumeState(config.globalVolumeMod);
        MusicState(config.musicVolumeMod);
        JoyType(config.JoysticTipe);


        bool state = CheckEmpty();
        ContOn.SetActive(!state); ContOff.SetActive(state);
        

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
    private void SaveConfig()
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



    [Header("Volume Page")]
    [SerializeField] GameObject[] Buttons;

    [Header("Continye Page")]
    [SerializeField] GameObject ContOn;
    [SerializeField] GameObject ContOff;
        
    [Header("Global Page")]
    [SerializeField] GameObject MenyButtons;
    [SerializeField] GameObject PlayButtons;
    [SerializeField] GameObject SetingButtons;

    [Header("PreMeny Page")]
    [SerializeField] GameObject PreMenyTheme;
    [SerializeField] GameObject MenyThem;
}