using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Dore : MonoBehaviour {
    private Collider2D coll;
    private SpriteRenderer curentArt;
    public JsonDoor remember;
    [HideInInspector] public MovementMemory memory;
    [HideInInspector] public Dore PrimeDore;
    public Transform startPos;
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
    [Space]

    public bool Vertical;
    public bool Bihaend;
    [Space]

    public bool Opened;
    [Space]


    public bool AllDone = false;
    public void SetComplite(bool state)
    {
        AllDone = state;
        remember.opened = AllDone;
        remember.complited = AllDone;
    }
    
    [SerializeField] bool curentState;
    [Space]


    public List <MemoriAtRock> memoryAtRock = new();

    public DooreSprites sp = new();
    private Vector3 size;
    private Vector3 offset;
    private SoundControler sound;
    private void Awake() 
    {
        curentArt = GetComponent<SpriteRenderer>();
        float y = curentArt.bounds.max.y;
        int order = -Mathf.RoundToInt(y * 10);
        curentArt.sortingOrder = order;

    
        GetName();
        memory = FindAnyObjectByType<MovementMemory>();
        memory.IsSaveReady(this);
        

        
        coll = GetComponent<Collider2D>();
        sound = FindAnyObjectByType<SoundControler>();
        Buttons.Preparation(this);
        
        remember = new JsonDoor(ID,Prime?AllDone:curentState);
            
        if(Prime)
        {
            PrimeDore = this;
            foreach (var b in ChildDore) 
            {
                b.PrimeDore = this; 
            }
        }
        size = Vertical? new Vector2(0.1f,3): new Vector2(3,0.1f);
        offset = ChengedColliderOffset();
        if(!AllDone) RemoveDore(newGame: true);
        
    }

    void Update() {
        if(curentState == true) Trigger();
    }

    public int ID;
    private void GetName()
    {
        Vector3 pos = transform.position;
        int x = Mathf.RoundToInt(Mathf.Abs(pos.x));
        int y = Mathf.RoundToInt(Mathf.Abs(pos.y));
        int z = 0;
        if(pos.x < 0 && pos.y >=0) z = 1;
        else if(pos.x >= 0 && pos.y < 0) z = 2;
        else if(pos.x < 0 && pos.y < 0) z = 3;
        ID = int.Parse(x + "" +y +"" +z);

        gameObject.name = $"Door {ID}";;
    }




    public void Check() {

        if(Opened) return;
        if(Buttons.Check()) 
        {
            if(!FlappyDore) memory.RegistPoint(this, curentState);
            OpenDore(true);
            if(AllDone) return;
            SetComplite(true);
            sound.DoreSound();
        }
        else if(FlappyDore) OpenDore(false);

    }

    public void OpenDore(bool _state = true, bool newGame = false)
    {
        curentArt.sprite = ChengedSprite(_state);
        coll.isTrigger =  _state;
        curentState = _state;

        if(!Prime) remember.opened = curentState;
        else if(Prime && (_state == false || newGame)) SetComplite(false);

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
        if(!Prime && Opened) OpenDore();
        else if(Prime &&  newGame) OpenDore(newGame:true);

        else OpenDore(false);
        Buttons.RemoveState();
        Buttons_Extra.RemoveState();
        
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
            ButtonsRemove();
            foreach (var r in memoryAtRock) r.ReturnPos();
            return;
        }

        LavelRemove(newGame);
        if(restLavel) foreach (var r in memoryAtRock) r.ReturnPos();
        else if(newGame) 
        {
            foreach (var r in memoryAtRock) r.NewGame();
            RemoveMemoryAtRook();
        }

    }
    public void LavelRemove(bool newGame = false)
    {
        ButtonsRemove();
        OpenDore(newGame, newGame);
        foreach (var d in ChildDore) d.RemoveDore();
    }
    public void ButtonsRemove()
    {
        Buttons.RemoveState();
        Buttons_Extra.RemoveState();
    }
    


    void Trigger()
    {
        if(sp.pl.curentDore == PrimeDore || memory.stopAll) return;
        
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
    public void RemoveMemoryAtRook() => memoryAtRock.Clear();
    



    
    

    [SerializeField] bool _gizmos = false;
    void OnDrawGizmos()
    {
        if(!_gizmos) return;
        if(!curentState || sp.pl.curentDore == PrimeDore) return;

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
    public AudioClip musicTheme;
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
        foreach (var b in Buttons) b.CheckPoint(); 
        foreach (var b in SwitchButton) b.RemoveState(); 
        foreach (var b in Set_StateButtons) b.button.CheckPoint(); 
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
        foreach (var b in StandartButtons) b.CheckPoint(); 
        foreach (var b in SwitchButtons) b.RemoveState(); 
    }

    
}
[Serializable]
public class MemoriAtRock
{
    public Rock rock;
    public Vector3 pos;
    public MemoriAtRock(Rock _rock, Vector3 _pos)
    {
        rock = _rock;
        pos = _pos;
    }
    public void ReturnPos() => rock.SetPos(pos);  
    public void NewGame()   => rock.SetPos();
    
}
public class JsonRock : Entyty
{
    public int name;
    public float x;
    public float y;
    public JsonRock(int name, Vector3 pos)
    {
        this.name = name;
        x = pos.x;
        y = pos.y;
        Type = EntytyType.Rock;
    }
    public Vector3 GetPos() => new Vector3(x,y);
    
}


public class JsonDoor : Entyty
{
    public int name;
    public bool opened;
    public bool complited;
    public List<JsonRock> memory;
    public JsonDoor(int n, bool end)
    {
        name = n;
        opened = end;
        memory = new();
        Type = EntytyType.Door;
    }
    
}
