using System.Collections;
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
    private bool _isPressed;
    public bool IsPressed
    {
        get { return _isPressed; }
        set { 
            _isPressed = value;
            curentArt.sprite = _isPressed? ClousedArt: OpenArt;
            foreach (var d in dore) d?.Check();
        }
    }
    
    void Update()
    {
        if (!nideToPressed && !IsPressed && CheckPoint()) IsPressed = true;
        if (nideToPressed) IsPressed = CheckPoint();
        
    }
    private bool CheckPoint() => Physics2D.OverlapPoint(transform.position, HardestObj);
}
