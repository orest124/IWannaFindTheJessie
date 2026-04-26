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

    void OnDisable()
    {
        sp.enabled = false;
    }
    void OnEnable()
    {
        sp.enabled = true;
    }
    void Start()
    {
        StartCoroutine(NextFrame());
    }
    IEnumerator NextFrame()
    {
        while (true)
        {
            yield return new WaitForSeconds(fall?fallAnimationTime:animationTime);
            
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
