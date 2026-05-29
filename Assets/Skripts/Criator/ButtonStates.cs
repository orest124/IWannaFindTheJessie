using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonStates : MonoBehaviour
{
    [HideInInspector]
    public List<Dore> dore = new();
    [SerializeField] LayerMask HardestObj;

    [SerializeField] SpriteRenderer curentArt;
    [SerializeField] Sprite[] stateImage;
    [SerializeField] Sprite[] statePressedImage;
    public int stateNomb;
    private int startNomb;
    private SoundControler sound;

    public bool isLost => stateNomb == stateImage.Length - 2;

    void Awake()
    {
        stateNomb --;
        NextState();
        RemoveState();
        sound = FindAnyObjectByType<SoundControler>();

    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            StartCoroutine(Timer(() => Pressed(true)));
            
        } 
    }
    [SerializeField] float volumeMod;
    private void Pressed(bool v)
    {
        if(v)
        {
            NextState();
        }
        sound.ButtonSound(v? 1 : volumeMod,v? 0 : 2);
        ChengeSprite(v);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Pressed(false);
        } 
    }
    private void NextState(bool remove = false)
    {
        if(stateNomb < stateImage.Length - 1)stateNomb ++;
        else stateNomb = 0;
        if(remove) stateNomb = startNomb;
        ChengeSprite(true);
        foreach (var d in dore) d?.Check();
        
    }
    private void ChengeSprite(bool state)
    {
        if(state == false) curentArt.sprite = stateImage[stateNomb];
        else curentArt.sprite = statePressedImage[stateNomb];
    }
    public void RemoveState() => NextState(true);





    [SerializeField] float duration;
    private WaitForFixedUpdate fix = new WaitForFixedUpdate();
    IEnumerator Timer(Action action)
    {
        float t = 0;
        while (t < duration)
        {
            yield return fix;
            t += Time.fixedDeltaTime;
        }
        action.Invoke();
        

    }
}
