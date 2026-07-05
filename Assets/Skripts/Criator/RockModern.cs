using UnityEngine;

public class RockModern : MonoBehaviour {

    [SerializeField] RockModernArt spr;
    [HideInInspector] public MovementMemory memory;
    private SoundControler sound;

    [Header("Values")]
    [SerializeField] private float spd;
    private float curentSpd;
    [Space]
    [SerializeField] Vector3 moveDir;
    private Vector3 targetPoint;
    private Coroutine _aCorutin;
    [SerializeField] private Button curentButton;

    [HideInInspector] public int ID;
    [HideInInspector] public Vector3 StartPos;


    [Header("State")]
    public bool isMove = false;
    [SerializeField] private bool Power = false;
    [SerializeField] private bool toTheMove = false;
    public void Awake_rock(SoundControler s, MovementMemory m)
    {
        ID = Methods.GetName(transform.position);
        name = $"Rock {ID}";
        StartPos = transform.position;
        sound = s;
        curentSpd = spd;
        spr.Start_art(this);
        memory = m;
        
    }

    void Update()
    {
        if(isMove) UncontrolMove(targetPoint);   
    }
    public void UncontrolMove(Vector3 _target)
    {
        Move(_target);
        if(Methods.isPlace(transform.position, _target))
        {
            PlaceControle();
        }
    }



    public bool MoveTo(Vector3 dir, bool _notStart = false)
    {
        if(isMove) return false;
        moveDir = dir;  
        Power = _notStart;
        Vector3 myPos = Methods.IdealPos(transform.position);
        targetPoint = myPos + dir;
        int n = Methods.CheckEmpty(myPos, dir);
        isMove = n == 0;
        FirstStepSpd();
        if (isMove && !toTheMove) 
        {
            memory.RegistPoint(this, transform.position, spr.PlatformState, _notStart);

            return true;
        }
        if (!isMove && Power && n > 1) 
            CheckRock(targetPoint);
        
        return false;
    }
    void Move(Vector3 target)
    {
        Vector3 newPos = Methods.GetNextStep(transform.position, target, curentSpd);
        transform.position = newPos; 
        spr.GetLayer(transform.position.y);
        
    }  
    public void FirstStepSpd()

    {
        int _placeTipe = Methods.CheckFallen(transform.position);
        if(_placeTipe == 2) curentSpd = spd * 1.5f;
        else curentSpd = spd;
    }
    public void PlaceControle()
    {
        CheckButton(transform.position);
        isMove = false;
        int _placeTipe = Methods.CheckFallen(transform.position);
        if(spr.PlatformState && _placeTipe != 1)
        {
            if(curentButton != null) spr.State(2);
            else spr.State(0);
        }
        if(_placeTipe == 2)
        {
            toTheMove = true;
            MoveTo(moveDir, toTheMove);
            curentSpd = spd * 1.5f;
        }
        else if(_placeTipe == 1) spr.State(1);
        else 
        {
            toTheMove = false;
            curentSpd = spd;
        }
    }
    public bool CheckRock(Vector3 _point)
    {
        var coll = Methods.CheckCollider(_point);
        if(coll == null) return true;
        else
        {
            if(Power) PushTheRock(coll);
            return false;
        }
    }

    public void PushTheRock(Collider2D rc)
    {
        RockModern r;
        if(rc.TryGetComponent<RockModern>(out r)) 
        { 
            r.MoveTo(moveDir,true);
        }

        if(toTheMove) sound.RockSound(1);
        Power = false;
        toTheMove = false;
    }
    public Vector3 GetTarget() => targetPoint;
    public void SetNoInteractiv(bool s) => memory.pl.SetNoInteractiv(s);




    public void CheckButton(Vector3 point)
    {
        curentButton = Methods.CheckButton(point, curentButton, this);

        if(curentButton = null)_aCorutin = StartActivateAnim(false);
    }
    private bool CheckCoroutine()
    {
        bool newt = _aCorutin == null;
        if(!newt) StopCoroutine(_aCorutin);
        return newt;
    }
    public void ForgiveMyRock(Button b)
    {
        if(curentButton != b) return;
        curentButton = null;
        spr.State(0);
    }
    public void SetPos(Vector3 point = new(), bool noTimer = false)
    {
        isMove = false;
        transform.position = point == Vector3.zero? StartPos : point;

        int _placeTipe = Methods.CheckFallen(transform.position);
        spr.State(_placeTipe == 1? 1:0);
        CheckButton(transform.position);
    }


    public Coroutine StartAlphaAnim(Vector3 nextPos) => StartCoroutine(spr.AlphaAnim(nextPos));
    public Coroutine StartActivateAnim(bool state)
    {
        bool newt = CheckCoroutine();

        if(state == curentActiveState) return null;
        curentActiveState = state;

        return StartCoroutine(spr.ActiveAnim(state, newt));
    }
    private bool curentActiveState = false;
}