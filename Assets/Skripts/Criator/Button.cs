using UnityEngine.Events;
using UnityEngine.Serialization;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    [HideInInspector]
    public List<Dore> dore = new();
    [SerializeField] LayerMask HardestObj;
    [SerializeField] float radius;

    private SpriteRenderer curentArt;
    [SerializeField] Sprite OpenArt;
    [SerializeField] Sprite ClousedArt;


    void Awake()
    {
        curentArt = GetComponent<SpriteRenderer>();
        
    }
    [SerializeField] bool nideToPressed;
    [SerializeField] bool _isPressed;
    public bool IsPressed
    {
        get { return _isPressed; }
        set { 
            if(_isPressed == value) return;
            _isPressed = value;
            curentArt.sprite = _isPressed? ClousedArt: OpenArt;
            foreach (var d in dore) d?.Check();
            if(_isPressed == true)SpecialAction?.Invoke();
            //dd
        }
    }
    [System.Serializable]
    
    public class PressedEvent : UnityEvent{}
    [FormerlySerializedAs("Special Action")]
    [SerializeField] private PressedEvent SpecialAction = new PressedEvent();
    
    void Update()
    {
        if (!nideToPressed && !IsPressed && CheckPoint()) IsPressed = true;
        if (nideToPressed) IsPressed = CheckPoint();
        
    }
    private bool CheckPoint() => Physics2D.OverlapPoint(transform.position, HardestObj);
}
