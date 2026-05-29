using UnityEngine.Events;
using UnityEngine.Serialization;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System;

public class Button : MonoBehaviour
{

    [HideInInspector]
    public List<Dore> dore = new();
    [Header("Sprite")]
    private SpriteRenderer curentArt;
    [SerializeField] private Sprite OpenArt;
    [SerializeField] private Sprite ClousedArt;
    private SoundControler sound;



    void Awake()
    {
        curentArt = GetComponent<SpriteRenderer>();
        sound = FindAnyObjectByType<SoundControler>();
    }
    [Header("Special")]
    [SerializeField] private bool nideToPressed;
    [SerializeField] private bool PlayerPressed;
    [SerializeField] private bool _isPressed;
    [SerializeField] private bool _curentState;

    public bool IsPressed
    {
        get { return _isPressed; }
        set { 
            if(_isPressed == value) return;
            _isPressed = value;
            Art().sprite = _isPressed? ClousedArt: OpenArt;
            if(DontNeedTimer)
            {
                if(value == true ) PressedEffect(); 
                else UnPressedEffect(); 
            }
            else
            {
                StopAllCoroutines();
                Sound(value? 1: silentMod, value? 0: 2);

                if(_isPressed == true) StartCoroutine(Timer(() => PressedEffect()));
                else StartCoroutine(Timer(() => UnPressedEffect())); 
                
            }
        }
    }

    [SerializeField] float silentMod;
    [SerializeField] float effectorMod;
    private void PressedEffect()
    {
        if(_curentState) return;

        _curentState = true;
        foreach (var d in dore) d?.Check();
        Sound(effectorMod, 1);

        SpecialAction?.Invoke();
    }
    private void UnPressedEffect()
    {
        if(!_curentState) return;

        _curentState = false;
        foreach (var d in dore) d?.Check();

        
    }
    [System.Serializable]
    
    public class PressedEvent : UnityEvent{}
    [FormerlySerializedAs("Special Action")]
    [SerializeField] private PressedEvent SpecialAction = new PressedEvent();






    //Потрібне квадратним кнопкам щоб ті знали що їх вже ніщо не тримає
    private bool ExeptedState = false;

    public void ChengPresed(bool state, bool player = false)
    {
        if(!PlayerPressed && player) return;
        ExeptedState = state;
        IsPressed = nideToPressed? state : true;
    
    }
    public void NormalizedState() => IsPressed = ExeptedState;
    public bool СheckState(bool s) => IsPressed == s;
    [SerializeField] float duration;
    [SerializeField] bool DontNeedTimer;
    private WaitForFixedUpdate fix = new WaitForFixedUpdate();
    IEnumerator Timer(Action action)
    {
        float t = 0;
        float curentDuration = IsPressed? duration : duration * 0.6f;
        while (t < curentDuration)
        {
            yield return fix;
            t += Time.fixedDeltaTime;
        }
        action.Invoke();
        

    }








    private void Sound(float mod = 1, int tipeButtons = 0)
    {
        if(sound != null) sound.ButtonSound(mod, tipe: tipeButtons);
        else
        {
            sound = FindAnyObjectByType<SoundControler>();
            sound.ButtonSound(tipe: tipeButtons);
        }
    }
    private SpriteRenderer Art()
    {
        if(curentArt != null) return curentArt;
        else
        {
            curentArt = GetComponent<SpriteRenderer>();
            return curentArt;
        }
    }
}
