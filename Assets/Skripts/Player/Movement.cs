using System.Collections.Generic;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Random = UnityEngine.Random;
using System.Collections;
using NUnit.Framework;

public class Movement : MonoBehaviour
{

    //Добавити сохраніння інЛавел

    [Header("States")] 
    public bool StopGame = false;
    public bool inLavel;
    public bool isMove;
    public bool PhotoOpen => !s.UIEmpty();
    public bool noInteractive;
    public bool OptionsOpen = false;
    [Header("Movement")] 
    [SerializeField] float spd;
    [SerializeField] float curentSpd;
    [Space]

    public Vector3 moveDir;
    [SerializeField] Vector3 targetPoint;

    [Header("Components")] 
    [SerializeField] PlayerSprite spr = new PlayerSprite();
    [SerializeField] FollowCamera Cra;
    [HideInInspector] public P_SoundAndPhoto s;
    [HideInInspector] public MovementMemory memory;
    [HideInInspector] public LavelInfo curentLavel;
    private MusicThemeControler music;
    [HideInInspector] public GameOptions meny;


    [Header("LavelStates")] 
    public Dore curentDore;
    [HideInInspector] public Dore abuseDore;
    [HideInInspector] public Vector3 StartPos;
    
    private readonly Vector3 zero = Vector3.zero;
    public void SetStop(bool _stop) => StopGame = _stop;
    public void SetNoInteractiv(bool noInter) => noInteractive = noInter;
    public bool isThisLavel(LavelInfo l)
    {
        if(curentLavel != l) {curentLavel = l; return false;}
        return true;
    }
    public bool _inSnow => spr.inSnow;


    public void CentralizedCamera() => Cra.CentralizedCamera();
    public void Awake_movement(FollowCamera _c)
    {
        s = GetComponent<P_SoundAndPhoto>();
        Cra = _c;
        Cra.CentralizedCamera();
        curentSpd = spd;
        preLavels.Add(curentDore);
    }
    public void Awake_servis(MovementMemory _memory, GameOptions _meny, 
        Dore _curentDoor, Dore _abuseDoor, MusicThemeControler _music)
    {
        memory = _memory;

        abuseDore = _abuseDoor;
        curentDore = _curentDoor == null? curentDore : _curentDoor;
        
        StartPos = curentDore.startPos.position;
        memory.NewDore(curentDore);
        
        meny = _meny;
        music = _music;
    }
    
    void Start()
    {
        spr.LStart();
    }
    private bool Run => Input.GetKey(KeyCode.LeftShift);
    public bool MotionMod;
    private void Update() {
        
        if(PhotoOpen || noInteractive) return;

        if ( Input.GetKeyDown(KeyCode.X)) memory.FlipCounter();
        //зробити щоб немона було робити сейв в войді
        if ( Input.GetKeyDown(KeyCode.Y)) meny.Save();
        if ( Input.GetKeyDown(KeyCode.L)) meny.ReloadScene();
        if(inLavel)
        {
            if ( Input.GetKeyDown(KeyCode.Escape)) meny.InOptions(!OptionsOpen);
            if ( Input.GetKeyDown(KeyCode.Q)) meny.BlackFon(() => memory.RestartLavel());
            else if ( Input.GetKeyDown(KeyCode.R)) memory.StepBihaind();
        }
        if ( Input.GetKeyDown(KeyCode.P)) SetStop(!StopGame);
        if ( Input.GetKeyDown(KeyCode.Space)) spr.ChengSprite(GetDirect());
        
            
        
        if(dirTimer > 0) { dirTimer -= Time.deltaTime; return; }
        if(isMove || StopGame) return;
        NewDirection();
    }
    public void SetMoveDir(Vector3 dir) => spr.ChengSprite(dir);
    

    private void FixedUpdate() {
        
        if(isMove)  UncontrolMove(targetPoint, curentSpd); 
        if(!inLavel) Move(transform.position + moveDir,curentSpd);
    }


    public void UncontrolMove(Vector3 _point, float _spd)
    {
        Move(_point, _spd);
        if(Methods.isPlace(_point, transform.position))
        {   
            PlaceControle(moveDir);
            memory.stopAll = false;
        }
    }
    public void PlaceControle(Vector3 dir)
    {
        isMove = false;
        int tipe = Methods.CheckFallen(transform.position);
        if(tipe != 2) toTheMove = false;

        if (tipe == 1) {spr.Falling(); meny.BlackFon(() => memory.RestartLavel());}
        else if (tipe == 2) 
        {
            ToMove(moveDir);
            toTheMove = true;
        }
    }
    void Move(Vector3 target, float _spd)
    {
        if( DiferentSituationInMove() ) return;

        float newspd = Time.fixedDeltaTime * _spd;
        Vector3 newPos = Vector2.MoveTowards(transform.position, target, newspd);
        
        spr.ChengSprite(moveDir);
        SetPos(newPos);
        
    }   


    public bool DiferentSituationInMove()
    {
        if(StopGame) {  spr.ChengSprite(zero); return true;   }
        if(inLavel)
        {
            if(fall) {      if(Methods.IdealPos(transform.position) == FallPos)spr.Falling();      }
            if(prePos == transform.position )   targetPoint = targetPoint - moveDir;

            prePos = transform.position; prePos.z = 0;
        }
        return false;
    }
    private Vector3 prePos;


    public void CheckButton(Vector3 point) => curentButton = Methods.CheckButton(point, curentButton, isPlayer:true);
    private Button curentButton;


    

    /////////////////////////
    // ------------------- //
    //    AUDIT METHOD     //
    // ------------------- //
    /////////////////////////

    bool PushRock = false;

    private float dirTimer;
    private float dirTime = 0.05f;
    private void NewDirection()
    {
        prePos = Vector3.zero; 
        if(dirTimer > 0) { return; }

        bool FollowMod = MotionMod? !Run : Run;
        PushRock = false;
        moveDir = GetDirect();
        spr.ChengSprite(moveDir);
        spr.ChengSprite(isMove? moveDir : zero);

        if(moveDir != zero )
        {
            if(!FollowMod) dirTimer = dirTime;

            if(!spr.RedyToMove()) {

                moveDir = zero;
                spr.StendUp(); return;
            }

            if(!inLavel) return;

            ToMove(moveDir);

            if(isMove == false) return;   
            
            int nomb = Methods.CheckFallen(targetPoint);
            if(nomb == 2) curentSpd = spd * 1.4f;
            else if(nomb == 1) {    isMove = false; return;     }
            else curentSpd = spd;
        }    
    }
    //Перевірити падіння вводу

    private bool toTheMove;
    private RockModern rockForward;
    public void ToMove(Vector3 dir)
    {
        if(isMove) return;
        moveDir = dir;  
        Vector3 myPos = Methods.IdealPos(transform.position);
        targetPoint = myPos + dir;

        bool FollowMod = MotionMod? !Run : Run;

        int n = CheckRockInMove(targetPoint);
        if((n == 3 && toTheMove) || n == 2) 
        {
            toTheMove = false;
            isMove = false;
            return;
        }
        if(n == 0) isMove = true;
        else if(n == 3 || n == 1) 
        {
            if(n == 3 && !toTheMove) PushTheRock(); 
            if(MotionMod)
            {
                n = CheckEmpty(myPos, dir);
                isMove = n == 0;
            }
        }
        if (isMove && !toTheMove) 
        {
            memory.RegistPoint(this, transform.position, false);
        }
    }
    private int CheckEmpty(Vector3 myPos, Vector3 dir) => Methods.CheckEmpty(myPos, dir, rockForward);
    private int CheckRockInMove(Vector3 targetPoint) => Methods.CheckRockInMove(targetPoint, rockForward, (RockModern rv) => {rockForward = rv;});

    public bool PushTheRock(Collider2D rc = null)
    {
        RockModern r;
        if(rc == null) rc = Methods.CheckCollider(targetPoint);
        if(rc.TryGetComponent<RockModern>(out r)) 
        { 
            if(r.MoveTo(moveDir)) return true;
        }
        return false;
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
            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) Bup = 1;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) Bdown = 1;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) Bright = 1;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) Bleft = 1;
            x = Bright - Bleft;
            y = Bup - Bdown;
        }
        if(inLavel && x != 0) y = 0;
        if(x != 0 || y != 0) spr.oldDir = new Vector3(x,y);
        return new Vector3(x,y);

    }
    private bool fall;
    private Vector3 FallPos;



    public bool CheckEmpty(Vector3 _point, bool JustCheck = false)
    {
        var coll = Methods.CheckCollider(_point);
        if(coll == null) return true;
        else if (JustCheck) return false;
        else
        {
            RockModern r;
            if(coll.TryGetComponent<RockModern>(out r))
            {
                spr.AlignedSprite(moveDir);
                r.MoveTo(moveDir);
                return false;
            }
            
            return false;
        }
    }
    public Vector3 GetTargetRockInMove(Vector3 _point)
    {
        var coll = Methods.CheckCollider(_point);
        if(coll == null) return new Vector3(0,0,-1);
        RockModern r;
        if(coll.TryGetComponent<RockModern>(out r))
        {
            if (r.isMove) return r.GetTarget();
            else if(r.MoveTo(moveDir))
            {
                PushRock = true;
                return r.GetTarget();
            }
            
        }
        return new Vector3(0,0,1);
        
    }
    






    /////////////////////////
    // ------------------- //
    //    STATE METHOD     //
    // ------------------- //
    /////////////////////////
    public List<Dore> preLavels = new();
    public void lavelMode(Dore _dore = null,Dore _callDore = null)
    {
        if(_dore == curentDore || StopGame || _dore == null) return;
        if(_dore == abuseDore)
        {
            
            NextDoor(_dore);
            preLavels.Clear();
            preLavels.Add(abuseDore);
            Idle(true, false);

        }
        else
        {
            NextDoor(_dore);
            preLavels.Add(curentDore);
            
            if(!inLavel)
            {
                Vector3 cor = GetDoorDirect(_callDore);
                inLavel = true;
                Vector3 newPos = Methods.IdealPos(_callDore.transform.position + cor);
                float xv = _callDore.Vertical? newPos.x:transform.position.x;
                float yv = !_callDore.Vertical? newPos.y:transform.position.y;

                targetPoint = new Vector3(xv,yv);
                isMove = true;
            }
        }

        AudioClip c = curentDore.sp.musicTheme;
        if(c != null) music.PlayTheme(c);
    }

    private Vector3 GetDoorDirect(Dore _dore = null)
    {
        Dore d = _dore == null? curentDore : _dore;
        int x = d.Vertical? 1:0;
        int y = !d.Vertical? 1:0;
        return new Vector3(x,y) * (d.Bihaend? -1:1);
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
    public void Idle( bool JustStp = false, bool FromLavel = true)
    {
        isMove = false;
        StopGame = false;
        moveDir = Vector2.zero;
        inLavel = FromLavel;
        StartPos = curentDore.startPos.transform.position;

        if(JustStp) return;
        
        SetPos(StartPos);
        spr.AlignedSprite(GetDoorDirect());
        CentralizedCamera();
        
    }

    public void StartAlphaAnim(Vector3 next) => StartCoroutine(spr.AlphaAnim(next));
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(targetPoint, new Vector3(0.3f,0.3f));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(targetPoint + new Vector3(0.5f,0f), targetPoint + new Vector3(-0.5f,0f));
        Gizmos.DrawLine(targetPoint + new Vector3(0,0.5f), targetPoint + new Vector3(0,-0.5f));
    }
    public void SetMusic(MusicThemeControler m) => music = m;
    
    public JsonCharacter GetPersonalMemory() 
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

    void F() => PlaceControle(new());
    void U() => CheckEmpty(new());

    public void SetPos(Vector3 point, bool stop = false)
    {
        transform.position = point;
        CheckButton(point);
        if(stop) Idle(true);
    }
}

