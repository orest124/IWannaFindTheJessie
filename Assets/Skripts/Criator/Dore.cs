using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Dore : MonoBehaviour {
    Collider2D coll;
    SpriteRenderer curentArt;
    [HideInInspector] public Dore PrimeDore;
    [SerializeField] bool FlappyDore;
    [SerializeField] bool EndDore;
    [HideInInspector] public List<PersonStepInfo> steps = new();
    [HideInInspector] public int stepCount = 0;


    public bool Prime;

    public bool Vertical;
    public bool Bihaend;

    public bool Closed;
    public bool Opened;
    public bool AllDone = false;
    public bool End;

    [SerializeField] bool state;
    [SerializeField] bool curentState;


    [SerializeField] ZoneOptimizatoin myZone;
    public Transform startPos;

    public List<PhotoPictures> CurentPhoto = new();
    public List<Rock> DinamicObj = new();
    public List <MemoriAtRock> memoryAtRock = new();

    [SerializeField] List<Button> PressButtons = new();
    [SerializeField] List<Button> NotNidButtons = new();
    [SerializeField] List<ButtonStates> NotNidButtonsState = new();
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
        Count = buttons.Count + buttonsState.Count + PressButtons.Count;
        if(Count != 0)
        {
            foreach (var b in buttons)             b.button.dore.Add(this); 
            foreach (var b in PressButtons)        b.dore.Add(this); 
            foreach (var b in NotNidButtons)       b.dore.Add(this); 
            foreach (var b in NotNidButtonsState)  b.dore.Add(this); 
            foreach (var b in buttonsState)        b.button.dore.Add(this); 
        }
            
        if(Prime)
        {
            PrimeDore = this;
            foreach (var b in ChildDore) 
            {
                b.PrimeDore = this; 
                b.myZone = myZone;
            }
        }
        size = Vertical? new Vector2(0.1f,3): new Vector2(3,0.1f);
        offset = ChengedColliderOffset();
        RemoveDore();
        
    }
    void Update()
    {
        if(curentState == true) Trigger();
    }

    public void Check() {

        if(Closed || Opened) return;
        PressedButtons = 0;

        foreach (var b in buttons) if (b.isValid()) PressedButtons++; 
        foreach (var b in buttonsState) if (b.isValid()) PressedButtons++; 
        foreach (var b in PressButtons) if (b.IsPressed == true) PressedButtons++; 
        
        if(PressedButtons == Count) OpenAudit();
        else if(FlappyDore)OpenDore(false);
    }

    public void OpenDore(bool _state = true)
    {
        curentArt.sprite = ChengedSprite(_state);
        curentArt.sortingOrder = _state? 10:sp.clousedOrder;
        
        coll.isTrigger =  _state;

        curentState = _state;
        End = !Prime && _state;
        if(EndDore && _state) PrimeDore.AllDone = true;
        
    }

    public void RemoveDore()
    {
        if(!Prime && !Closed && !Opened) OpenDore(state);
        else if(!Prime && Closed && !Opened) OpenDore(false);
        else if(!Prime && !Closed && Opened) OpenDore(true);

        else if(Prime && !Closed) OpenDore(true);
        else OpenDore(false);
    }

    public void OpenAudit()
    {
        if(!Closed && !Opened) OpenDore(true);
        else if (Closed || Opened) return;
    }

    private Sprite ChengedSprite(bool open)
    {
        if(open) return Vertical? sp.OpenArt_Ver: sp.OpenArt_Hor;
        return Vertical? sp.ClousedArt_Ver : sp.ClousedArt_Hor;
    }
    private Vector3 ChengedColliderOffset()
    {
        Vector3 newOffset = new();
        newOffset.x = Vertical? 1f:0;
        newOffset.y = Vertical? 0:1f;
        return Bihaend? -newOffset : newOffset;
    }
    public void Restartlavel(bool newGame = false, bool justButtons = false, bool restLavel = false)
    {
        foreach (var b in buttons) b.ReturnState(); 
        foreach (var b in buttonsState) b.button.RemoveState(); 
        foreach (var b in PressButtons) b.IsPressed = false; 
        foreach (var b in NotNidButtons) b.IsPressed = false; 
        foreach (var b in NotNidButtonsState) b.RemoveState(); 

        if(restLavel && memoryAtRock.Count != 0)
        {
            AllDone = false;
            OpenDore(false);
            foreach (var r in memoryAtRock) r.ReturnPos();
            foreach (var d in ChildDore) d.RemoveDore();
            return;
        }
        if(!justButtons) foreach (var o in DinamicObj) o.Restart(); 

        if(AllDone && !newGame) return;
        
        if(newGame) AllDone = false;
        
        foreach (var d in ChildDore) d.RemoveDore();
    }
    
    public Vector3 size;
    public Vector3 offset;
    public MovementMemory memory;

    void Trigger()
    {
        if(sp.pl.curentDore == PrimeDore) return;
        
        if(Physics2D.OverlapBox(transform.position + offset, size, 0, LayerMask.GetMask("Player")))
        {
            sp.pl.lavelMode(PrimeDore);
            if(Prime && !AllDone) 
            {
                OpenDore(false);
                memory.ReturnAllRockInLavel(startPos.transform.position ,inMemory: true);
            }
        }
    }



    
    


    void OnDrawGizmos()
    {
        if(!curentState) return;
        if(sp.pl.curentDore == PrimeDore) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position + offset, size);
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
public class MemoriAtRock
{
    Collider2D rock;
    Vector3 pos;
    public MemoriAtRock(Collider2D _rock, Vector3 _pos)
    {
        rock = _rock;
        pos = _pos;
    }
    public void ReturnPos() => rock.transform.position = pos;
    
}
