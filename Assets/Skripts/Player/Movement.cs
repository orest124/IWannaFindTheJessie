
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
    [HideInInspector] public MovementMemory memory;


    [Header("Abuse")] 
    public Dore abuseDore;
    public bool ReversArtefact;

    [Header("LavelStates")] 
    public Dore curentDore;
    public Dore StartDore;
    public Vector3 StartPos;
    public Vector3 NawGamePos;

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
        FollowCamera();
        curentSpd = spd;
        
    
    }
    private void Update() {
        if(Input.GetKeyDown(KeyCode.R) && ReversArtefact) memory.StepBihaind();
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R)) memory.RestartLavel();
        if(Input.GetKey(KeyCode.Space) && Input.GetKeyUp(KeyCode.R)) memory.AbsolutRestart();
        if(Input.GetKeyUp(KeyCode.G)) memory._gizmo = !memory._gizmo;

        if(isMove || StopGame) return;
        NewDirection();
    }

    private void FixedUpdate() {

        FollowCamera(transform.position);
        if(isMove)  UncontrolMove(targetPoint, curentSpd); 
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
    private Vector3 targetPoint;
  


    void Move(Vector3 target, float _spd)
    {
        if(StopGame) return;
        float newspd = Time.fixedDeltaTime * _spd;
        Vector3 newPos = Vector2.MoveTowards(transform.position, target, newspd);
        
        spr.ChengSprite(moveDir);
        // FollowCamera();
        transform.position = newPos;
    }   

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

    
    



    // Audit Method

    bool CinemaMod = false;
    private void NewDirection()
    {
        moveDir = GetDirect();
        spr.ChengSprite(moveDir);

        if(moveDir != zero )
        {
            if(!spr.RedyToMove()) {

                moveDir = zero;
                spr.StendUp(); return;
            }

            if(!_inLavel) return;

            if(moveDir.x != 0 && moveDir.y != 0) moveDir.y = 0;
            targetPoint = IdealPos(mod: false) + moveDir;
            isMove = CheckEmpty(targetPoint);

            if(isMove == false) return;

            memory.RegistPoint(this, transform.position);

            int nomb = ChageToFallen(targetPoint);
            if(nomb == 2) 
            {
                //Падіння небуде бо він не робе скан кожен крок
                curentSpd = spd * 1.6f;
                FindLostPoint(moveDir);
            }
            else if(nomb == 1) {moveDir = zero; spr.ChengSprite(moveDir);return;}
            else curentSpd = spd;

        }    
    }
    public Vector3 GetDirect()
    {
        float Bup = 0; float Bdown = 0; float Bleft = 0; float Bright = 0;
        
        if (Input.GetKey(up)) Bup = 1;
        if (Input.GetKey(down)) Bdown = 1;
        if (Input.GetKey(right)) Bright = 1;
        if (Input.GetKey(left)) Bleft = 1;

        float x = Bright - Bleft;
        float y = Bup - Bdown;
        return new Vector3(x,y);
    }
    public void FindLostPoint(Vector3 dir)
    {

        targetPoint = IdealPos(mod: false) + dir;
        while(true)
        {
            int tipe = ChageToFallen(targetPoint);
            if(tipe == 1) break;
            
            else if(tipe == 2)
            {
                if(!CheckEmpty(targetPoint + dir, true)) break;
                
                targetPoint += dir;
                continue;
            }
            else break;
        }
    }

    public int ChageToFallen(Vector3 _point)
    {
        if (Physics2D.OverlapCircle(_point, 0.2f, s.DipthArea) 
        && !Physics2D.OverlapPoint(_point, s.PlatformMask)) return 1;
        if (Physics2D.OverlapCircle(_point,0.2f, s.IceArea)) return 2;
        else return 0; 
    }

    public bool CheckEmpty(Vector3 _point, bool JustCheck = false)
    {
        var coll = CheckCollider(_point);
        if(coll == null || coll.isTrigger) return true;
        else if (JustCheck) return false;
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
        int tipe = ChageToFallen(transform.position);

        if (tipe == 1) {memory.RestartLavel();spr.Falling();}
        else isMove = false;
    }


    private bool isPlace(Vector2 place) => place.x == transform.position.x && place.y == transform.position.y;
    private Vector3 IdealPos(bool mod = true)
    {
        float _mod = mod? 0.5f : 0;
        return new Vector2((int)(transform.position.x + _mod), (int)(transform.position.y + _mod));
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
        if(!inLavel)
        {
            _inLavel = false;
            curentDore = abuseDore;
            StartPos = curentDore.startPos.position;
            optim.ChengedMusic();

        }
        else
        {
            if(curentDore == _dore) return;
            curentDore = _dore;
            StartPos = curentDore.startPos.position;
            memory.NewDore(curentDore);
            
            if(!_inLavel)
            {
                int x = curentDore.Vertical? 1:0;
                int y = !curentDore.Vertical? 1:0;
                Vector3 cor = new Vector3(x,y) * (curentDore.Bihaend? -1:1);
                
                _inLavel = true;
                optim.ChengedMusic();
                targetPoint = IdealPos() + cor;
                isMove = true;
            }
        }
    }
    public void Idle(bool newGame = false, bool notFromLavel = false)
    {
        if(newGame) curentDore = StartDore;
        StartPos = curentDore.startPos.transform.position;
        _inLavel = !notFromLavel;

        isMove = false;
        StopGame = false;
        moveDir = Vector2.zero;
        transform.position = StartPos;
        
    }
    public float minModSpd;
    public float maxModSpd;
    public float dist;
    public void FollowCamera(Vector3 point)
    {
        float _spd = Time.fixedDeltaTime;
        dist = Vector3.SqrMagnitude(Cra.transform.position - new Vector3(transform.position.x,transform.position.y,Cra.transform.position.z));
        if(dist > 9) _spd = _spd * maxModSpd;
        Vector3 newpoint = Vector3.MoveTowards(Cra.transform.position, new Vector3(point.x,point.y, Cra.transform.position.z), _spd);
        Cra.transform.position = newpoint;
    }
    public void FollowCamera() => Cra.transform.position = new Vector3(transform.position.x,transform.position.y, Cra.transform.position.z);
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(targetPoint, new Vector3(0.3f,0.3f));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(targetPoint + new Vector3(0.5f,0f), targetPoint + new Vector3(-0.5f,0f));
        Gizmos.DrawLine(targetPoint + new Vector3(0,0.5f), targetPoint + new Vector3(0,-0.5f));
    }
}
