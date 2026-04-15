using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dore : MonoBehaviour {
    Collider2D coll;
    SpriteRenderer curentArt;
    [HideInInspector] public Dore PrimeDore;

    [SerializeField] bool Prime;
    [SerializeField] bool EndDore;

    public bool Vertical;
    public bool Bihaend;

    public bool Closed;
    [SerializeField] bool state;
    [SerializeField] bool curentState;
    [SerializeField] bool AllDone = false;

    

    

    [SerializeField] ZoneOptimizatoin myZone;
    public Transform startPos;



    public List<PhotoPictures> CurentPhoto = new();
    [SerializeField] List<Rock> DinamicObj = new();
    [SerializeField] List<ButtonState_Pres> buttons = new();
    [SerializeField] List<ButtonState_State> buttonsState = new();
    public List<Dore> ChildDore = new();


    public DooreSprites sp = new();
    private int PressedButtons;
    private int Count;
    
    
    private void Awake() 
    {
        curentArt = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
        Count = buttons.Count + buttonsState.Count;;
        if(Count != 0)
        {
            foreach (var b in buttons) 
                { b.button.dore.Add(this); }
        }
        if(!Closed || !Prime) OpenDore(state);

        if(Prime)
        {
            if(!Closed) OpenDore(true);

            PrimeDore = this;
            foreach (var b in ChildDore) 
            {
            b.PrimeDore = this; 
            b.myZone = myZone;
            }
        }
            
        
    }
    public void Check() {

        if(Closed) return;
        PressedButtons = 0;
        foreach (var b in buttons) 
        {
            if (b.isValid()) PressedButtons++; 
            else continue;
        }
        foreach (var b in buttonsState) 
        {
            if (b.isValid()) PressedButtons++; 
            else continue;
        }
        if(PressedButtons == Count) OpenDore(true);
        // else OpenDore(false);
    }

    public void OpenDore(bool _state)
    {
        curentArt.sprite = ChengedSprite(_state);
        curentArt.sortingOrder = _state? 10:sp.clousedOrder;
        
        coll.offset = ChengedColliderOffset(_state);
        coll.isTrigger = _state;

        curentState = _state;
        End = !Prime && _state;
        if(EndDore && _state) PrimeDore.AllDone = true;
        
    }

        private Sprite ChengedSprite(bool open)
    {
        if(open) return Vertical? sp.OpenArt_Ver: sp.OpenArt_Hor;
        return Vertical? sp.ClousedArt_Ver : sp.ClousedArt_Hor;
    }
    private Vector3 ChengedColliderOffset(bool open)
    {
        Vector3 newOffset = new();
        if(open)
        {
            newOffset.x = Vertical? 2:0;
            newOffset.y = Vertical? 0:2;
            if(Bihaend) return -newOffset;
        }
        return newOffset;
    }
    public void Restartlavel()
    {
        foreach (var o in DinamicObj) 
            { o.Restart(); }
        foreach (var b in buttons) 
            { b.ReturnState(); }
        
        if(AllDone) return;
        
        foreach (var d in ChildDore) 
        { 
            d.End = false;
            d.Check();
            d.OpenDore(false);
        }
        
            
    }
    public bool End;
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == sp.pl.gameObject)
        {

            sp.pl.lavelMode(PrimeDore);
            // myZone.OpenNeighbor();
            if(!Prime || AllDone) return;
            OpenDore(false);
            
        }
    }
}

[System.Serializable]
public class ButtonState_Pres
{
    public Button button;
    public bool state;
    public void ReturnState() => button.IsPressed = false;
    public bool isValid() => button.IsPressed == state;
}
[System.Serializable]
public class ButtonState_State
{
    public ButtonStates button;
    public int state;
    public void ReturnState() => button.RemoveState();
    public bool isValid() => button.stateNomb == state;
}
[System.Serializable]
public class DooreSprites
{
    public Movement pl;
    public Sprite OpenArt_Hor;
    public Sprite ClousedArt_Hor;
    public Sprite OpenArt_Ver;
    public Sprite ClousedArt_Ver;
    public int clousedOrder;
}
