using UnityEngine.Events;
using UnityEngine.Serialization;
using System.Collections.Generic;
using UnityEngine;
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
        CheckPoint();
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
            Sound();
            Art().sprite = _isPressed? ClousedArt: OpenArt;
            foreach (var d in dore) d?.Check();
            if(_isPressed == true)SpecialAction?.Invoke();
        }
    }
    [System.Serializable]
    
    public class PressedEvent : UnityEvent{}
    [FormerlySerializedAs("Special Action")]
    [SerializeField] private PressedEvent SpecialAction = new PressedEvent();

    public void ChengPresed(bool state, bool player = false)
    {
        if(!PlayerPressed && player) return;
        IsPressed = nideToPressed? state : true;
    
    }
    public void CheckPoint() 
    {
        Collider2D c = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("Rook"));
        c?.GetComponent<Rock>().ButtonCheck(transform.position);
    }
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
