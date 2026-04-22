using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Dore : MonoBehaviour {
    private Collider2D coll;
    private SpriteRenderer curentArt;
    [HideInInspector] public MovementMemory memory;
    [HideInInspector] public Dore PrimeDore;
    public Transform startPos;
    [SerializeField] ZoneOptimizatoin myZone;
    [Space] [Space]

    [Header("Buttons")]
    [Tooltip("Тут можна назначити кнопки")]
    [SerializeField] private ImportantButtonsCollection Buttons;
    [Tooltip("Тут можна назначити неважливі кнопки")]
    [SerializeField] private NotImportantButtonsCollection Buttons_Extra;
    [Space] [Space]

    public bool Prime;
    [Tooltip("Якщо забрати з кнопки камінь двері закриються. \n Навіть коли рівень пройдено")]
    [SerializeField] bool FlappyDore;
    [HideInInspector] public List<PersonStepInfo> steps = new();
    [HideInInspector] public int stepCount = 0;
    [Space]

    public bool Vertical;
    public bool Bihaend;
    [Space]

    public bool Closed;
    public bool Opened;
    [Space]

    public bool AllDone = false;
    [Space]


    [Tooltip("Стан дверей на початку гри")]
    [SerializeField] bool startState;
    [SerializeField] bool curentState;





    public List<PhotoPictures> CurentPhoto = new();
    public List <MemoriAtRock> memoryAtRock = new();

    public List<Dore> ChildDore = new();


    
    public DooreSprites sp = new();
    private int _count;
    private Vector3 size;
    private Vector3 offset;
    
    private void Awake() 
    {
        curentArt = GetComponent<SpriteRenderer>();
        coll = GetComponent<Collider2D>();
        memory = FindAnyObjectByType<MovementMemory>();
        _count = Buttons.Count;
        Buttons.Preparation(this);
        // на початку гри має оприділятись ордер енд леєр
            
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
        RemoveDore(newGame: true);
        
    }
    void Update()
    {
        if(curentState == true) Trigger();
    }

    public void Check() {

        if(Closed || Opened) return;
        if(Buttons.Check() == _count) 
        {
            if(!FlappyDore) memory.RegistPoint(this, curentState);
            OpenDore(true);
            AllDone = true;
        }
        else if(FlappyDore) OpenDore(false);

    }

    public void OpenDore(bool _state = true, bool newGame = false)
    {
        curentArt.sprite = ChengedSprite(_state);
        curentArt.sortingOrder = _state? 10:sp.clousedOrder;
        
        coll.isTrigger =  _state;

        curentState = _state;
        if(Prime && !_state) AllDone = false;
        if(newGame) AllDone = false;
    }

    public void RemoveDore(bool newGame = false)
    {
        if(!Prime && !Closed && !Opened && newGame) OpenDore(startState);
        else if(!Prime && Closed && !Opened) OpenDore(false);
        else if(!Prime && !Closed && Opened) OpenDore();

        else if(Prime && !Closed && !newGame) OpenDore(false);
        else if(Prime && !Closed && newGame) OpenDore(newGame:true);
        else OpenDore(false);
    }

    private Sprite ChengedSprite(bool open)
    {
        if(open) return Vertical? sp.OpenArt_Ver: sp.OpenArt_Hor;
        return Vertical? sp.ClousedArt_Ver : sp.ClousedArt_Hor;
    }
    private Vector3 ChengedColliderOffset()
    {
        Vector3 newOffset = new Vector3(Vertical? 1f:0,Vertical? 0:1f);
        return Bihaend? -newOffset : newOffset;
    }
    public void Restartlavel(bool newGame = false, bool restLavel = false)
    {
        RemoveDore(newGame: true);
        if(restLavel)
        {
            foreach (var r in memoryAtRock) r.ReturnPos();
            foreach (var d in ChildDore) d.RemoveDore();
            return;
        }
        else if(newGame)
        {
            foreach (var r in memoryAtRock) r.NewGame();
            foreach (var d in ChildDore) d.RemoveDore(newGame: true);
        }
        Buttons.RemoveState();
        Buttons_Extra.RemoveState();
            

    }
    


    void Trigger()
    {
        if(sp.pl.curentDore == PrimeDore) return;
        
        if(Physics2D.OverlapBox(transform.position + offset, size, 0, LayerMask.GetMask("Player")))
        {
            sp.pl.lavelMode(PrimeDore);
            if(Prime && AllDone == false) 
            {
                memory.RegistPoint(this, curentState);
                OpenDore(false, true);
                if(memoryAtRock.Count == 0) memory.ReturnAllRockInLavel(inMemory: true);
            }
        }
    }



    
    

    [SerializeField] bool _gizmos = false;
    void OnDrawGizmos()
    {
        if(!curentState || !_gizmos || sp.pl.curentDore == PrimeDore) return;

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
    public void Preparation(Dore d) => button.dore.Add(d);

}
[System.Serializable]
public class ButtonState_State
{
    public ButtonStates button;
    public int state;
    public void ReturnState() => button.RemoveState();
    public bool isValid() => button.stateNomb == state;
    public void Preparation(Dore d) => button.dore.Add(d);
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
[System.Serializable]
public class ImportantButtonsCollection
{

    [Tooltip("Відкриття при нажатті")]
    public List<Button> Buttons = new();
    [Tooltip("Відкриття на останній стан")]
    public List<ButtonStates> SwitchButton = new();
    [Tooltip("Задати стан")]
    public List<ButtonState_Pres> Set_StateButtons = new();
    [Tooltip("Задати стан")]
    public List<ButtonState_State> Set_SwitchButton = new();
    public int Count => Buttons.Count + SwitchButton.Count + Set_StateButtons.Count + Set_SwitchButton.Count;
    public void Preparation(Dore d)
    {
        foreach (var b in Buttons) b.dore.Add(d);
        foreach (var b in SwitchButton) b.dore.Add(d);
        foreach (var b in Set_StateButtons) b.Preparation(d);
        foreach (var b in Set_SwitchButton) b.Preparation(d);
    }
    public int Check()
    {
        int _count = 0;
        foreach (var b in Buttons) if(b.IsPressed) _count++;
        foreach (var b in SwitchButton) if(b.isLost) _count++;;
        foreach (var b in Set_StateButtons) if(b.isValid()) _count++;
        foreach (var b in Set_SwitchButton) if(b.isValid()) _count++;
        return _count;
    }
    public void RemoveState()
    {
        foreach (var b in Buttons) b.IsPressed = false; 
        foreach (var b in SwitchButton) b.RemoveState(); 
        foreach (var b in Set_StateButtons) b.ReturnState(); 
        foreach (var b in Set_SwitchButton) b.ReturnState(); 
    }
}
[System.Serializable]
public class NotImportantButtonsCollection
{
    public List<Button> StandartButtons = new();
    public List<ButtonStates> SwitchButtons = new();
    public void RemoveState()
    {
        foreach (var b in StandartButtons) b.IsPressed = false; 
        foreach (var b in SwitchButtons) b.RemoveState(); 
    }

    
}
public class MemoriAtRock
{
    public Rock rock;
    public Vector3 pos;
    public MemoriAtRock(Rock _rock, Vector3 _pos)
    {
        rock = _rock;
        pos = _pos;
    }
    public void ReturnPos() => rock.transform.position = pos;
    public void NewGame() => rock.RemovePos();
    
}
