
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Movement : MonoBehaviour
{
    [Header("States")] 
    public bool StopGame = false;
    public bool isMove;
    public bool isAction;
    public bool DialogOpen = false;
    public bool OptionsOpen = false;

    [Header("Components")] 
    [SerializeField] PlayerSprite spr = new PlayerSprite();
    [SerializeField] Camera Cra;
    [HideInInspector] public P_SoundAndPhoto s;
    [HideInInspector] public Optimization optim;
    private Collider2D coll;


    [Header("Abuse")] 
    public Dore abuseDore;

    [Header("LavelStates")] 
    public Dore curentDore;
    private Vector3 StartPos;

    [Header("Keyboard")] 
    KeyCode up = KeyCode.W;
    KeyCode left = KeyCode.A;
    KeyCode down = KeyCode.S;
    KeyCode right = KeyCode.D;

    [Header("Movement")] 
    public Vector3 moveDir;
    [SerializeField] float spd;
    float curentSpd;
    public float castRadius;
    


    public void SetStop(bool _stop) => StopGame = _stop;
    public bool _inLavel {get{return optim.inLavel;} set{optim.inLavel = value;}}
    public bool _inSnow => spr.inSnow;



    void Awake()
    {
        optim = GetComponent<Optimization>();
        s = GetComponent<P_SoundAndPhoto>();
        coll = GetComponent<Collider2D>();
        spr.LStart();
        
        StartPos = curentDore.startPos.position;
        // transform.position = StartPos;
        Cra.transform.position = new Vector3(transform.position.x,transform.position.y, Cra.transform.position.z);
        curentSpd = spd;
        
    
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.R)) Death();

        if(isMove) return;
        NewDirection();
    }

    private void FixedUpdate() {

        if(isMove)  UncontrolMove(_targetPoint, curentSpd); 
        if(!_inLavel && !CinemaMod) Move(transform.position + moveDir,curentSpd);
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
            PlaceControle(moveDir);
        }
            
    }
    private Vector3 _targetPoint;
  


    void Move(Vector3 target, float _spd)
    {
        if(StopGame) return;
        float newspd = Time.fixedDeltaTime * _spd;
        Vector3 newPos = Vector2.MoveTowards(transform.position, target, newspd);
        
        spr.ChengSprite(moveDir);
        Cra.transform.position = new Vector3(transform.position.x,transform.position.y, Cra.transform.position.z);
        transform.position = newPos;
    }   

    private readonly Vector3 zero = Vector3.zero;
    
    public void ReturnToaMap(Vector3 target)
    {
        CinemaMod = true;
        curentSpd = 10;
        coll.enabled = false;
        spr.Falling();
        _targetPoint = target;
        isMove = true;
    }

    
    







    

    // Audit Method

    bool CinemaMod = false;
    private void NewDirection()
    {
        
        float Bup = 0; float Bdown = 0; float Bleft = 0; float Bright = 0;
        
        if (Input.GetKey(up)) Bup = 1;
        if (Input.GetKey(down)) Bdown = 1;
        if (Input.GetKey(right)) Bright = 1;
        if (Input.GetKey(left)) Bleft = 1;
        

        float x = Bright - Bleft;
        float y = Bup - Bdown;
        
        moveDir = new Vector2(x, y);
        spr.ChengSprite(moveDir);


        if(moveDir == zero ) return;
        
        if(!spr.RedyToMove())
        {
            moveDir = zero;
            spr.StendUp();
            return;
        }

        if(!_inLavel) return;

        if(x != 0 && y != 0) moveDir.y = 0;
        _targetPoint = transform.position + moveDir;
        isMove = CheckEmpty(_targetPoint);
        int tipe = CheckToFallen(_targetPoint);
        if(tipe > 0) 
        {
            spr.Falling();
        }
        
    }

    public int CheckToFallen(Vector3 _point)
    {
        bool _isIce = Physics2D.OverlapCircle(_point,0.2f, s.IceArea);
        bool _isDipth = Physics2D.OverlapCircle(_point, 0.2f, s.DipthArea) 
                && !Physics2D.OverlapPoint(_point, s.PlatformMask);
        if(_isDipth) return 1;
        else if (_isIce) return 2;
        else return 0; 
    }

    public bool CheckEmpty(Vector3 _point, bool _action = false)
    {
        var coll = CheckCollider(_point);
        if(coll == null || coll.isTrigger) return true;
        else if(_action) return false;
        else
        {
            Rock r;
            if(coll.TryGetComponent<Rock>(out r))
            {
                r.MoveTo(moveDir);
                return false;
            }
            else return false;
        }
    }
    private Collider2D CheckCollider(Vector3 point) => Physics2D.OverlapCircle(point, 0.3f,s.BaricadeArea);
    

    public void PlaceControle(Vector3 dir)
    {
        Vector3 myPoint = transform.position;
        int tipe = CheckToFallen(myPoint);

        if      (tipe == 1) Death();
        else if (tipe == 2) 
        {
            _targetPoint = moveDir + transform.position;
            isMove = CheckEmpty(_targetPoint, true);
  
        }
        else isMove = false;
    }


    private bool isPlace(Vector2 place)
    {
        return place.x == transform.position.x && place.y == transform.position.y;
    }
    private Vector3 IdealPos()
    {
        int x = (int)(transform.position.x + 0.5f);
        int y = (int)(transform.position.y + 0.5f);
        return new Vector2(x, y);
    }
    public void SprintAudit()
    {

        if (Input.GetKeyDown(KeyCode.LeftShift)) 
            {curentSpd = spd + s.pc.PhotoCount() * 0.1f; spr.AnimSpd(true); s.ChengTimeLoop(true);}

        else if (Input.GetKeyUp(KeyCode.LeftShift)) 
            {curentSpd = spd; spr.AnimSpd(false);s.ChengTimeLoop();}
    }

    

    //State Metods

    
    public void MenuMod(bool _state)
    {
        StopGame = _state;
    }

    public void lavelMode(Dore _dore = null, bool inLavel = true)
    {
        _inLavel = inLavel;
        if(!_inLavel)
        {
            curentDore = abuseDore;
            StartPos = curentDore.startPos.position;
            notFromLavel = true;
        }
        else
        {
            if(curentDore == _dore) return;
            curentDore = _dore;
            StartPos = _dore.startPos.position;
            optim.ChengedMusic();
            if(!notFromLavel)
            {
                _targetPoint = StartPos;
                isMove = true;
            }
        }
    }
    public bool notFromLavel = false;
    public void Death()
    {
        isMove = false;
        moveDir = Vector2.zero;
        if(isAction) spr.StendUp();
        transform.position = StartPos;
        Cra.transform.position = new Vector3(StartPos.x,StartPos.y, Cra.transform.position.z);

        curentDore.Restartlavel();
    }

}
