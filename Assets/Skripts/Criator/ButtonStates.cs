using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonStates : MonoBehaviour
{
    [HideInInspector]
    public List<Dore> dore = new();
    [SerializeField] LayerMask HardestObj;

    private SpriteRenderer curentArt;
    [SerializeField] Sprite[] stateImage;
    public int stateNomb;

    void Awake()
    {
        curentArt = GetComponent<SpriteRenderer>();
        stateNomb --;
        NextState();
        
    }
    // private bool CheckPoint() => Physics2D.OverlapPoint(transform.position, HardestObj);
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            NextState();
        } 
    }
    private void NextState(bool remove = false)
    {
        if(stateNomb < stateImage.Length - 1)stateNomb ++;
        else stateNomb = 0;
        if(remove) stateNomb = 0;
        curentArt.sprite = stateImage[stateNomb];
        foreach (var d in dore) d?.Check();
        
    }
    public void RemoveState() => NextState(true);
}
