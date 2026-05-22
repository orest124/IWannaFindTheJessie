using System;
using System.Collections.Generic;
using UnityEngine;

public class Dore : MonoBehaviour {
    [SerializeField] private int StepCountToGold1;
    [SerializeField] private int StepCountToGold2;
    [HideInInspector] public LavelInfo myLavel;
    public bool DontNidStatistic;

    bool MoveTipe => memory.pl.MotionMod;
    public int GetDifference() => (MoveTipe? StepCountToGold2 : StepCountToGold1) - StepCount -1;
    public int GetCountToGold() => MoveTipe? StepCountToGold2 : StepCountToGold1;
    private Collider2D coll;
    private SpriteRenderer curentArt;
    [HideInInspector] public MovementMemory memory;
    [HideInInspector] public Dore PrimeDore;
    public Transform startPos;
    [Space] [Space]
    [Header("Cild Dores")]
    public List<Dore> ChildDore = new();
    [Space] [Space]

    [Header("Buttons")]
    [Tooltip("Тут можна назначити кнопки")]
    [SerializeField] private ImportantButtonsCollection Buttons;
    [Tooltip("Тут можна назначити неважливі кнопки")]


    public bool Prime;
    [Tooltip("Якщо забрати з кнопки камінь двері закриються. \n Навіть коли рівень пройдено")]
    [SerializeField] bool FlappyDore;
    [Space]

    public bool Vertical;
    public bool Bihaend;
    [Space]


    public bool AllDone = false;
    public void SetComplite(bool state, bool Load = false)
    {
        AllDone = state;
        if(DontNidStatistic || Load) return; 
        if(Prime && AllDone)myLavel.GetRating();
    }

    public int StepCount = 0;
    
    [SerializeField] bool curentState;
    [Space]


    public List <MemoriAtRock> memoryAtRock = new();

    public DooreSprites sp = new();
    private Vector3 size;
    private Vector3 offset;
    private SoundControler sound;
    private void Awake() 
    {
        coll = GetComponent<Collider2D>();
        curentArt = GetComponent<SpriteRenderer>();
        float y = curentArt.bounds.max.y;
        int order = -Mathf.RoundToInt(y * 10) + 5;
        curentArt.sortingOrder = order;

        Buttons.Preparation(this);
        

        if(Prime || FlappyDore)
        {
            sound = FindAnyObjectByType<SoundControler>();
            memory = FindAnyObjectByType<MovementMemory>();
            GetName();
            memory.IsSaveReady(this);
        }    
            
        if(Prime)
        {
            PrimeDore = this;
            foreach (var b in ChildDore) 
            {
                if(select)
        {
            
        }
                b.PrimeDore = this;
                b.sound = sound;
                b.memory = memory;
                b.GetName();
                memory.IsSaveReady(b);
            }
        }
        size = Vertical? new Vector2(0.1f,3): new Vector2(3,0.1f);
        offset = ChengedColliderOffset();
        if(!AllDone) NewPreparation();
        
    }

    void Update() {
        if(curentState == true) Trigger();
    }


                            /////////////////////////
                            // ------------------- //
                            //   Службові Мктоди   //
                            // ------------------- //
                            /////////////////////////

public bool select = false;

    /// <summary> /// Load метод дверей /// </summary>
    public void LoadPreparation(JsonDoor i)
    {
        remember = i;
        OpenDore(i.State);
        SetComplite(i.Complit, true);
        ReturnCount(i.C_M453,i.moveTipe);
    }
    public void ReturnCount(int _count, bool tipe)
    {
        if(tipe == MoveTipe)
        StepCount = _count;
        else
        {

            int n = (tipe? StepCountToGold2 : StepCountToGold1) - (_count - 1);
            int t = GetCountToGold();
            StepCount = n + t;
        }
    }

    /// <summary> /// NewGame метод дверей + restart /// </summary>
    public void NewPreparation(bool rest = false)
    {
        if(Prime) OpenDore(rest? false:true);
        else OpenDore(Buttons.count == 0? true:false);
        SetComplite(false);
        ButtonsRemove();
        if(FlappyDore) Check();
    }
    public void ButtonsRemove() => Buttons.RemoveState();
    
    /// <summary> /// Фізичний метод дверей. Виклик лише в методах /// </summary>
    public void OpenDore(bool _state = true)
    {
        curentState = _state;
        coll.isTrigger =  curentState;
        curentArt.sprite = ChengedSprite(curentState);
    }
    public void Check() {
        if(AllDone && FlappyDore == false) return;
        if(Buttons.Check()) 
        {
            OpenDore(true);
            SetComplite(true);
            
            if(!Prime) return;
            sound.DoreSound();
            memory.RemoveMemory(true);
        }
        else if(FlappyDore)
        {
            OpenDore(false);
            SetComplite(false);
        }

    }


                                                /////////////////////////
                                                // ------------------- //
                                                //   Рестарт  Методи   //
                                                // ------------------- //
                                                /////////////////////////

        /// <summary> /// Перезапуск рівня /// </summary>
    public void Restart(bool newGame = false)
    {
        PutAllRock(newGame);
        RemoveLavels(newGame);
        ButtonsRemove();
    }

    private void RemoveLavels(bool newGame = false)
    {
        NewPreparation(!newGame);
        foreach (var d in ChildDore) d.NewPreparation(!newGame);
    }
    /// <summary> /// Робота з камінням + перезапуск дверей /// </summary>
    public void PutAllRock(bool newGame = false)
    {
        if(newGame)
        {
            foreach (var r in memoryAtRock) r.NewGame();
            RemoveMemoryAtRook(); 
            remember = null;
        } 
        else foreach (var r in memoryAtRock) r.ReturnPos();
    }


    
                                                /////////////////////////
                                                // ------------------- //
                                                //  Допоміжні  Методи  //
                                                // ------------------- //
                                                /////////////////////////
    void Trigger()
    {
        if(sp.pl.curentDore == PrimeDore || memory.stopAll) return;
        
        if(Physics2D.OverlapBox(transform.position + offset, size, 0, LayerMask.GetMask("Player")))
        {
            sp.pl.lavelMode(PrimeDore);
            if(Prime && AllDone == false) 
            {
                OpenDore(false);
                if(memoryAtRock.Count == 0) memory.ReturnAllRockInLavel(inMemory: true);
            }
        }
    }
    public void RemoveMemoryAtRook() => memoryAtRock.Clear();
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
    

    public int ID;
    private JsonDoor remember;
    public JsonDoor GetJson()
    {
        if(remember == null || remember.name == 0)
        {
            JsonDoor i = new JsonDoor(ID,AllDone,curentState,StepCount, MoveTipe);
            foreach (var r in memoryAtRock) i.memory.Add(new JsonRock(r.rock.ID, r.pos));
            remember = i;
        }
        else remember.UpdateJson(AllDone,curentState,StepCount,MoveTipe);
        return remember;
    }


    [SerializeField] bool _gizmos = false;
    void OnDrawGizmos()
    {
        if(!_gizmos) return;
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
    public void Normalized() => button.NormalizedState();
    public bool isValid() => button.СheckState(state);
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
        foreach (var b in Buttons) b?.dore.Add(d);
        foreach (var b in SwitchButton) b?.dore.Add(d);
        foreach (var b in Set_StateButtons)
        {
            if(b.button == null) Set_StateButtons.Remove(b);
            b.Preparation(d);
        }
        foreach (var b in Set_SwitchButton) 
        {
            if(b.button == null) Set_SwitchButton.Remove(b);
            b.Preparation(d);
        }
        count = Buttons.Count + SwitchButton.Count + Set_StateButtons.Count + Set_SwitchButton.Count;
    }
    public bool Check()
    {
        if(count == 0) return true;
        int _count = 0;
        foreach (var b in Buttons) if(b.СheckState(true)) _count++;
        foreach (var b in SwitchButton) if(b.isLost) _count++;;
        foreach (var b in Set_StateButtons) if(b.isValid()) _count++;
        foreach (var b in Set_SwitchButton) if(b.isValid()) _count++;
        return _count == count;
    }
    public void RemoveState()
    {
        foreach (var b in Buttons) b.NormalizedState(); 
        foreach (var b in SwitchButton) b.RemoveState(); 
        foreach (var b in Set_StateButtons) b.Normalized(); 
        foreach (var b in Set_SwitchButton) b.ReturnState(); 
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
    public bool Complit;
    public bool State;
    public bool moveTipe;
    public int C_M453;
    public List<JsonRock> memory;
    public JsonDoor (int name, bool AllDone, bool curentState, int stepCount, bool keyT)
    {
        this.name = name;
        Complit = AllDone;
        State = curentState;
        C_M453 = stepCount;
        moveTipe = keyT;
        memory = new();
        Type = EntytyType.Door;
    }
    public void UpdateJson(bool AllDone, bool curentState, int stepCount, bool keyT)
    {
        moveTipe = keyT;
        Complit = AllDone;
        State = curentState;
        C_M453 = stepCount;
    }
    
    
}
