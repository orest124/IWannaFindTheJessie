using System;
using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Random = UnityEngine.Random;

public class Movement : MonoBehaviour
{

    //Добавити сохраніння інЛавел

    [Header("States")] 
    public bool StopGame = false;
    public bool inLavel;
    public bool isMove;
    public bool PhotoOpen => !s.notFromPhoto;
    public bool OptionsOpen = false;
    [Header("Movement")] 
    [SerializeField] float spd;
    private float curentSpd;
    [Space]

    public Vector3 moveDir;
    [SerializeField] Vector3 targetPoint;

    [Header("Components")] 
    [SerializeField] PlayerSprite spr = new PlayerSprite();
    [SerializeField] Camera Cra;
    [HideInInspector] public P_SoundAndPhoto s;
    [HideInInspector] public MovementMemory memory;
    private MusicThemeControler music;
    private FollowCamera cMove;
    [HideInInspector] public GameOptions meny;
    private Collider2D coll;


    [Header("Abuse")] 
    public Dore abuseDore;
    private bool ReversArtefact;

    [Header("LavelStates")] 
    public Dore curentDore;
    [HideInInspector] public Vector3 StartPos;


    


    public void SetStop(bool _stop) => StopGame = _stop;

    public bool _inSnow => spr.inSnow;



    void Awake()
    {
        s = GetComponent<P_SoundAndPhoto>();
        coll = GetComponent<Collider2D>();
        cMove = FindAnyObjectByType<FollowCamera>();
        StartPos = curentDore.startPos.position;
        CentralizedCamera();
        curentSpd = spd;
        preLavels.Add(curentDore);
    }
    void Start()
    {
        spr.LStart();
    }
    private bool Run => Input.GetKey(KeyCode.LeftShift);
    private void Update() {
        
        if(PhotoOpen) return;

        if ( Input.GetKeyDown(Esc)) meny.InOptions(!OptionsOpen);
        if ( Input.GetKeyDown(Restart))
        {
            if(curentDore.AllDone) memory.LocalClining( forMemory:true );
            else memory.RestartLavel();
        }

        else if ( Input.GetKeyDown(Return)) memory.StepBihaind();
        if ( Input.GetKeyDown(Save)) meny.Save();

        
        if(isMove || StopGame) return;
        NewDirection();
    }

    private void FixedUpdate() {

        if(isMove)  UncontrolMove(targetPoint, curentSpd); 
        if(!inLavel && !CinemaMod) Move(transform.position + moveDir,curentSpd);
    }


    public void UncontrolMove(Vector3 _point, float _spd)
    {
        Move(_point, _spd);
        if(isPlace(_point))
        {
            if(CinemaMod) {
                
                coll.enabled = true; 
                curentSpd = spd;
                moveDir = zero;
                spr.StendUp();
                CinemaMod = false;
            }
            else
            {
                
            PlaceControle(moveDir);
            memory.stopAll = false;
            }

        }
            
    }
  

    private Vector3 prePos;
    void Move(Vector3 target, float _spd)
    {
        if(StopGame) return;
        if(inLavel)
        {
            if(fall)
            {
                if(IdealPos() == FallPos)
                {
                    spr.Falling();
                }
            }
            if(prePos == transform.position )
            {
                targetPoint = IdealPos();
            }
            prePos = transform.position;
            prePos.z = 0;
        }
        float newspd = Time.fixedDeltaTime * _spd;
        Vector3 newPos = Vector2.MoveTowards(transform.position, target, newspd);
        
        spr.ChengSprite(moveDir);
        transform.position = newPos;
        CheckButton(newPos);
    }   

    private void CheckButton(Vector3 point)
    {
        Collider2D coll = Physics2D.OverlapPoint(point, LayerMask.GetMask("Button"));
        if(coll != null && prewColl != coll)
        {
            prewColl = coll;
            curentButton?.ChengPresed(false, true);
            curentButton = coll.GetComponent<Button>();
            curentButton.ChengPresed(true, true);
        }
        else if(coll == null && prewColl != null)
        {
            curentButton.ChengPresed(false, true);
            curentButton = null;
            prewColl = null;
        }
    }
    
    private Collider2D prewColl;
    private Button curentButton;

    private readonly Vector3 zero = Vector3.zero;
    
    public void ReturnToaMap(Vector3 target)
    {
        CinemaMod = true;
        curentSpd = 10;
        coll.enabled = false;
        spr.Falling();
        targetPoint = target;
        isMove = true;
    }

    /////////////////////////
    // ------------------- //
    //    AUDIT METHOD     //
    // ------------------- //
    /////////////////////////

    bool CinemaMod = false;
    private void NewDirection()
    {
        prePos = Vector3.zero; 
        moveDir = GetDirect();
        spr.ChengSprite(moveDir);

        if(moveDir != zero )
        {
            if(!spr.RedyToMove()) {

                moveDir = zero;
                spr.StendUp(); return;
            }

            if(!inLavel) return;

            targetPoint = IdealPos(mod: false) + moveDir;

            if(Run) {
                Vector3 pos = GetTargetRockInMove(targetPoint);
                if(pos.z == 1) return;
                else if(pos.z == -1) isMove = true;
                else isMove = targetPoint != pos; 
            }
            else  isMove = CheckEmpty(targetPoint);
             
            if(isMove == false) return;   
            
            memory.RegistPoint(this, transform.position);

            int nomb = CheckFallen(targetPoint);
            if(nomb == 2) 
            {
                curentSpd = spd * 1.5f;
                FindLostPoint(moveDir);
            }
            else if(nomb == 1) {moveDir = zero; spr.ChengSprite(moveDir);return;}
            else curentSpd = spd;

        }    
    }
    [HideInInspector] public bool JoysticMod;
    public Vector3 GetDirect()
    {
        float x, y;
        
        if(JoysticMod)
        {
            x = Mathf.RoundToInt(Input.GetAxis("Horizontal"));
            y = Mathf.RoundToInt(Input.GetAxis("Vertical"));
        }
        else
        {
            float Bup = 0, Bdown = 0, Bleft = 0, Bright = 0;
            if (Input.GetKey(up) || Input.GetKey(KeyCode.UpArrow)) Bup = 1;
            if (Input.GetKey(down) || Input.GetKey(KeyCode.DownArrow)) Bdown = 1;
            if (Input.GetKey(right) || Input.GetKey(KeyCode.RightArrow)) Bright = 1;
            if (Input.GetKey(left) || Input.GetKey(KeyCode.LeftArrow)) Bleft = 1;
            x = Bright - Bleft;
            y = Bup - Bdown;
        }
        if(inLavel && x != 0) y = 0;
            
        return new Vector3(x,y);

    }


    public void FindLostPoint(Vector3 dir)
    {
        fall = false;
        int rockForvard = 0;
        Random.InitState(Random.Range(0, 12545));

        targetPoint = IdealPos(mod: false) + dir;
        while(true)
        {
            int tipe = CheckFallen(targetPoint);
            if(tipe == 1) break;
            
            else if(tipe == 2)
            {
                int p = CheckRockInMove(targetPoint + dir);
                if(p == 2) break;
                else if(p == 1) rockForvard++;
            
                if(!fall)
                {
                    int n = Random.Range(0, Run? 14:30);
                    fall = n == 13;
                    if(fall) FallPos = targetPoint;
                }
                targetPoint += dir;
                continue;
            }
            else break;
        }
        targetPoint -= moveDir * rockForvard;
    }
                private bool fall;
                private Vector3 FallPos;


    public int CheckFallen(Vector3 _point)
    {
        if (Physics2D.OverlapCircle(_point, 0.2f, s.DipthArea) 
        && !Physics2D.OverlapPoint(_point, s.PlatformMask)) return 1;
        if (Physics2D.OverlapCircle(_point,0.2f, s.IceArea)) return 2;
        else return 0; 
    }
    public bool CheckEmpty(Vector3 _point, bool JustCheck = false)
    {
        var coll = CheckCollider(_point);
        if(coll == null) return true;
        else if (JustCheck) return false;
        else
        {
            Rock r;
            if(coll.TryGetComponent<Rock>(out r))
            {
                r.MoveTo(moveDir);
                cMove.shake = true;
                return false;
            }
            
            return false;
        }
    }
    public int CheckRockInMove(Vector3 _point)
    {
        var coll = CheckCollider(_point);
        if(coll == null) return 0;
        else
        {
            Rock r;
            if(coll.TryGetComponent<Rock>(out r))
            {
                if (r.isMove) return 1;
                else return 2;
            }
            return 2;
        }
    }
    public Vector3 GetTargetRockInMove(Vector3 _point)
    {
        var coll = CheckCollider(_point);
        if(coll == null) return new Vector3(0,0,-1);
        Rock r;
        if(coll.TryGetComponent<Rock>(out r))
        {
            if (r.isMove) return r.GetTarget();
            else
            {
                cMove.shake = true;
                r.MoveTo(moveDir);
                return new Vector3(0,0,1);
            } 
        }
        return new Vector3(0,0,1);
        
    }
    private Collider2D CheckCollider(Vector3 point)
    {
        Collider2D coll = Physics2D.OverlapPoint(point, LayerMask.GetMask("Wall"));
        if(coll != null && !coll.isTrigger) return coll;
        
        coll = Physics2D.OverlapPoint(point, LayerMask.GetMask("Rook"));
        if(coll != null && !coll.isTrigger) return coll;

        return null;
        
    } 
    

    public void PlaceControle(Vector3 dir)
    {
        int tipe = CheckFallen(transform.position);

        if (tipe == 1) {memory.RestartLavel();spr.Falling();}
        else isMove = false;
    }


    private bool isPlace(Vector2 place) => place.x == transform.position.x && place.y == transform.position.y;
    private Vector3 IdealPos(bool mod = true)
    {
        float _mod = mod? 0.5f : 0;
        return new Vector2(Mathf.RoundToInt(transform.position.x + _mod), Mathf.RoundToInt(transform.position.y + _mod));
    }

    public void SprintAudit()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift)) 
            {curentSpd = spd + s.pc.PhotoCount() * 0.1f; spr.AnimSpd(true); s.ChengTimeLoop(true);}

        else if (Input.GetKeyUp(KeyCode.LeftShift)) 
            {curentSpd = spd; spr.AnimSpd(false);s.ChengTimeLoop();}
    }

    /////////////////////////
    // ------------------- //
    //    STATE METHOD     //
    // ------------------- //
    /////////////////////////

    public void MenuMod(bool _state)
    {
        StopGame = _state;
    }
    public void ReversOn(bool state)
    {
        ReversArtefact = state;
        memory.RemoveMemory();
    }
    public List<Dore> preLavels = new();
    public void lavelMode(Dore _dore = null)
    {
        if(_dore == curentDore || StopGame) return;
        if(_dore == null)
        {
            
            inLavel = false;
            curentDore = abuseDore;
            preLavels.Add(abuseDore);
            StartPos = curentDore.startPos.position;

        }
        else
        {
            NextDoor(_dore);
            preLavels.Add(curentDore);
            
            if(!inLavel)
            {
                int x = curentDore.Vertical? 1:0;
                int y = !curentDore.Vertical? 1:0;
                Vector3 cor = new Vector3(x,y) * (curentDore.Bihaend? -1:1);
                
                inLavel = true;
                targetPoint = IdealPos() + cor;
                isMove = true;
            }
        }

        AudioClip c = curentDore.sp.musicTheme;
        if(c != null) music.PlayTheme(c);
    }


    public void NextDoor(Dore _dore)
    {
        curentDore = _dore;
        StartPos = curentDore.startPos.position;
        memory.NewDore(curentDore);
    }
    public void SetOldDoor()
    {
        preLavels.Remove(preLavels[^1]);
        NextDoor(preLavels[^1]);
    }
    public bool IsLostDoor() => preLavels.Count <= 1;
    public void Idle(bool FromLavel = true)
    {
        StartPos = curentDore.startPos.transform.position;
        inLavel = FromLavel;

        isMove = false;
        StopGame = false;
        moveDir = Vector2.zero;
        transform.position = StartPos;
        
    }
    public void FiendKey(int tipe)
    {
        switch (tipe)
        {
            case 3:
                up = KeyCode.I; 
                left = KeyCode.J; 
                down = KeyCode.K; 
                right = KeyCode.L;
                Return = KeyCode.U; 
                Restart = KeyCode.Space; 
                Esc = KeyCode.Escape;
                Save = KeyCode.P;
            break;
        }
    }
    [Header("Keyboard")] 
    private KeyCode up = KeyCode.W; private KeyCode left = KeyCode.A; private KeyCode down = KeyCode.S; private KeyCode right = KeyCode.D;
    private KeyCode Return = KeyCode.Space; private KeyCode Restart = KeyCode.Q; private KeyCode Esc = KeyCode.Escape; private KeyCode Save = KeyCode.Y;
    

    public void CentralizedCamera() => Cra.transform.position = new Vector3(transform.position.x,transform.position.y, Cra.transform.position.z);
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(targetPoint, new Vector3(0.3f,0.3f));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(targetPoint + new Vector3(0.5f,0f), targetPoint + new Vector3(-0.5f,0f));
        Gizmos.DrawLine(targetPoint + new Vector3(0,0.5f), targetPoint + new Vector3(0,-0.5f));
    }
    public void SetMusic(MusicThemeControler m) => music = m;
    
    public JsonCharacter GetPersonalmemory() 
    {
        JsonCharacter i = new JsonCharacter();
        i.inLavel = inLavel;
        i.SetVector(transform.position);
        return i.BuldNevMemory(s.pc._photoColection, preLavels);
    }

    void Q() => NewDirection();
    void W() => UncontrolMove(new(), 0);
    void R() => Move(new(), 0);
    void A() => lavelMode();
    void S() => Idle();
    void D() => OnDrawGizmos();


    void E() => CheckButton(new());
    void Y() => CheckFallen(new());
    void U() => CheckEmpty(new());
    void T() => CheckCollider(new());

}

public class JsonCharacter : Entyty
{
    public int x;
    public int y;
    public bool inLavel;
    public List<int> photoIDs;
    public List<int> DoorIDs;
    public Vector3 GetVector() => new Vector3(x,y);
    public void SetVector(Vector3 pos)
    {
        x = Mathf.RoundToInt(pos.x);
        y = Mathf.RoundToInt(pos.y);
    }
    public JsonCharacter BuldNevMemory(List<PhotoPictures> pict, List<Dore> ds)
    {
        ID = 0;
        Type = EntytyType.Character;
        
        photoIDs = new();
        DoorIDs = new();
        foreach (var f in pict) photoIDs.Add(f.ID);
        foreach (var d in ds) DoorIDs.Add(d.ID);
        return this;
    }

}
