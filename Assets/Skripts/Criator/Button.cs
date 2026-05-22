using UnityEngine.Events;
using UnityEngine.Serialization;
using System.Collections.Generic;
using UnityEngine;

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

    public bool IsPressed
    {
        get { return _isPressed; }
        set { 
            if(_isPressed == value) return;
            _isPressed = value;
            foreach (var d in dore) d?.Check();
            Sound();
            Art().sprite = _isPressed? ClousedArt: OpenArt;
            if(_isPressed == true)SpecialAction?.Invoke();
        }
    }
    [System.Serializable]
    
    public class PressedEvent : UnityEvent{}
    [FormerlySerializedAs("Special Action")]
    [SerializeField] private PressedEvent SpecialAction = new PressedEvent();

    public bool ExeptedState = false;





    public void ChengPresed(bool state, bool player = false)
    {
        if(!PlayerPressed && player) return;
        ExeptedState = state;
        IsPressed = nideToPressed? state : true;
    
    }
    public void NormalizedState() => IsPressed = ExeptedState;
    public bool СheckState(bool s) => IsPressed == s;
    









    private void Sound()
    {
        if(sound != null) sound.ButtonSound();
        else
        {
            sound = FindAnyObjectByType<SoundControler>();
            sound.ButtonSound();
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
