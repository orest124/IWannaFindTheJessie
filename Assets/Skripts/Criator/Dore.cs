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

    [Header("Cild Dores")]
    public List<Dore> ChildDore = new();
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
    [SerializeField] bool curentState;
    [Space]






    public List<PhotoPictures> CurentPhoto = new();
    public List <MemoriAtRock> memoryAtRock = new();
    
    public DooreSprites sp = new();
    private Vector3 size;
    private Vector3 offset;
    private void Awake() 
    {
        curentArt = GetComponent<SpriteRenderer>();
        float y = curentArt.bounds.max.y;
        int order = -Mathf.RoundToInt(y * 10);
        curentArt.sortingOrder = order;

        
    
        coll = GetComponent<Collider2D>();
        memory = FindAnyObjectByType<MovementMemory>();
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
        if(Buttons.Check()) 
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
        
        coll.isTrigger =  _state;

        curentState = _state;
        if(Prime && !_state) AllDone = false;
        if(newGame) AllDone = false;
    }

    /*
        Звичайні двері відкриваються
        За начальним стейтом
        Просто коли в них алл дон
        Праймові
        Відкриваються з завершенням рівня на алл дон
        Відкриваються без алл дон
        newGame = Стан як на початку гри.
    */
    public void RemoveDore(bool newGame = false)
    {
        if(!Prime && !Closed && !Opened) OpenDore(false);

        else if(Opened && !Prime && !Closed) OpenDore();
        else if(Closed && !Prime && !Opened) OpenDore(false);

        else if(Prime && !newGame && !Closed) OpenDore(false);
        else if(Prime &&  newGame && !Closed) OpenDore(newGame:true);
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
    /*
        В яких випадках викликається?
        1. Локальний перезапуск конкретного рівня
        2. Повний перезапуск карти



    */
    
    public void LavelMod(bool newGame = false, bool restLavel = false, bool forMemory = false)
    {
        if(forMemory)
        {
            foreach (var r in memoryAtRock) r.ReturnPos();
            return;
        }
        Buttons.RemoveState();
        Buttons_Extra.RemoveState();

        if(restLavel)
        {
            RemoveDore();
            foreach (var r in memoryAtRock) r.ReturnPos();
            foreach (var d in ChildDore) d.RemoveDore();
        }
        else if(newGame)
        {
            RemoveDore(newGame: true);
            foreach (var r in memoryAtRock) r.NewGame();
            foreach (var d in ChildDore) d.RemoveDore(newGame: true);
        }
            

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
                OpenDore(false);
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
    
    public int count;

    public void Preparation(Dore d)
    {
        foreach (var b in Buttons) b.dore.Add(d);
        foreach (var b in SwitchButton) b.dore.Add(d);
        foreach (var b in Set_StateButtons) b.Preparation(d);
        foreach (var b in Set_SwitchButton) b.Preparation(d);
        count = Buttons.Count + SwitchButton.Count + Set_StateButtons.Count + Set_SwitchButton.Count;
    }
    public bool Check()
    {
        int _count = 0;
        foreach (var b in Buttons) if(b.IsPressed) _count++;
        foreach (var b in SwitchButton) if(b.isLost) _count++;;
        foreach (var b in Set_StateButtons) if(b.isValid()) _count++;
        foreach (var b in Set_SwitchButton) if(b.isValid()) _count++;
        return _count == count;
    }
    public void RemoveState()
    {
        foreach (var b in Buttons) b.RemoveState(); 
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
        foreach (var b in StandartButtons) b.RemoveState(); 
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
    public void ReturnPos() { rock.SetPos(pos);  } 
    public void NewGame() => rock.SetPos();
    
}
