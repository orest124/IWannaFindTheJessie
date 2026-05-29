using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSprite : MonoBehaviour
{
    public SpriteRenderer sp;

    public Sprite idleSprite;
    public Sprite[] animationSprite;
    public Sprite[] FallAnimationSprite;
    public float animationTime = 0.025f;
    public float fallAnimationTime = 0.025f;
    private int animationFrame;
    public bool loop = true;
    public bool idle = true;
    public bool fall = false;

    void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
    }
    void FixedUpdate()
    {
        if (LoopTimer() == false) return;
            
        loopT = 0;
        animationFrame++;
        if (loop && animationFrame >= animationSprite.Length)
            animationFrame = 0;

        if(fall)
        {
            sp.sprite = FallAnimationSprite[animationFrameFall];
            if(animationFrameFall < FallAnimationSprite.Length - 1) animationFrameFall++;
        }
        else if(!fall && !_redyToMove)
        {
            sp.sprite = FallAnimationSprite[animationFrameFall];
            if(animationFrameFall > 0) animationFrameFall--;
            if(animationFrameFall == 0) _redyToMove = true;
        }
        else if (idle) 
            sp.sprite = idleSprite;

        
        else if (animationFrame >= 0 && animationFrame < animationSprite.Length)
            sp.sprite = animationSprite[animationFrame];
    }


    float loopT = 0;
    public bool LoopTimer()
    {
        loopT += Time.fixedDeltaTime;
        if(loopT > (fall?fallAnimationTime:animationTime)) return true;
        else return false;
        
    }

    void OnDisable()
    {
        sp.enabled = false;
    }
    void OnEnable()
    {
        sp.enabled = true;
    }
    
    public void SetAlpha(float a = 0)
    {
        sp.color = new Color(sp.color.r,sp.color.g,sp.color.b, a);
    }
    
    int animationFrameFall = 0;

    public void Falling()
    {
        _redyToMove = false;
        fall = true;
    }
    public void StendUp() => fall = false;

    
    bool _redyToMove = true;
    public bool RedyToMove() => _redyToMove;

}
