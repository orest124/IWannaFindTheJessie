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


    void Awake()
    {
        curentArt = GetComponent<SpriteRenderer>();
        RemoveState();
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
            curentArt.sprite = _isPressed? ClousedArt: OpenArt;
            foreach (var d in dore) d?.Check();
            if(_isPressed == true)SpecialAction?.Invoke();
        }
    }
    [System.Serializable]
    
    public class PressedEvent : UnityEvent{}
    [FormerlySerializedAs("Special Action")]
    [SerializeField] private PressedEvent SpecialAction = new PressedEvent();

    public void RemoveState() =>IsPressed = CheckPoint();
    private bool CheckPoint() => Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("Rook"));
    internal void ChengPresed(bool state, bool player = false)
    {
        if(!PlayerPressed && player) return;
        IsPressed = nideToPressed? state : true;
    }
}
