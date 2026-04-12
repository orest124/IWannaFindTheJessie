using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSprite : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    public Sprite idleSprite;
    public Sprite[] animationSprite;
    public Sprite[] FallAnimationSprite;
    public float animationTime = 0.025f;
    private int animationFrame;
    public bool loop = true;
    public bool idle = true;
    public bool fall = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnDisable()
    {
        spriteRenderer.enabled = false;
    }
    void OnEnable()
    {
        spriteRenderer.enabled = true;
    }
    void Start()
    {
        StartCoroutine(NextFrame());
    }
    IEnumerator NextFrame()
    {
        while (true)
        {
            yield return new WaitForSeconds(animationTime);
            
            animationFrame++;
            if (loop && animationFrame >= animationSprite.Length)
                animationFrame = 0;

            if(fall)
            {
                spriteRenderer.sprite = FallAnimationSprite[animationFrameFall];
                if(animationFrameFall < FallAnimationSprite.Length - 1) animationFrameFall++;
            }
            else if(!fall && !_redyToMove)
            {
                spriteRenderer.sprite = FallAnimationSprite[animationFrameFall];
                if(animationFrameFall > 0) animationFrameFall--;
                if(animationFrameFall == 0) _redyToMove = true;
            }
            else if (idle) 
                spriteRenderer.sprite = idleSprite;

            
            else if (animationFrame >= 0 && animationFrame < animationSprite.Length)
                spriteRenderer.sprite = animationSprite[animationFrame];

            
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
