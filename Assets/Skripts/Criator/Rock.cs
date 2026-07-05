using System.Collections;
using UnityEngine;

public class Rock : MonoBehaviour 
{
    void W() => UncontrolMove(new());
    void R() => Move(new());
    void E() => FindLostPoint(new());
    void A() => State(false);

    void D() => CheckButton(new());
    void Y() => PlaceControle();
    void U() => CheckEmpty(new());
    void T() => CheckCollider(new());

    [SerializeField] bool NOAlpfa;
    SpriteRenderer sp;
    [SerializeField] Collider2D Hcoll;
    [SerializeField] Collider2D Acoll;

    [SerializeField] float spd;
    [SerializeField] float curentSpd;

    public Vector2 StartPos;
    public bool isMove;
    [SerializeField] bool PlatformState;
    [SerializeField] Sprite PlatformArt;
    [SerializeField] Sprite ActiveArt;
    [SerializeField] Sprite RockArt;


    [HideInInspector] public MovementMemory memory;
    public LayerMask PlatformLayer;
    private SoundControler sound;


    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        sp.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10);
        GetName();
        StartPos = transform.position;
        memory = FindAnyObjectByType<MovementMemory>();
        // memory.IsSaveReady(this);
        // memory.GetSoundControler += () => {sound = memory.sound;};
        curentSpd = spd;
    }
    void Start()
    {
        CheckButton(transform.position);
    }
    void FixedUpdate()
    {
        if(isMove) UncontrolMove(targetPoint);
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
        ID = int.Parse($"{x}{y}{z}");

        gameObject.name = $"Rock {ID}";;
    }
    /////////////////////////
    // ------------------- //
    //   Movement METHOD   //
    // ------------------- //
    /////////////////////////
    public void SetPos(Vector3 point = new(), bool noTimer = false)
    {
        if(transform.position == point) return;
        isMove = false;
        point = point == Vector3.zero? StartPos : point;
        transform.position = point;
        bool state = CheckFallen(transform.position) == 1;
        if(sp.sprite == PlatformArt && !state || sp.sprite != PlatformArt && state) State(state);
        CheckButton(transform.position);
    }



    public Vector3 GetTarget() => targetPoint;
    [SerializeField] Vector3 targetPoint;
    [SerializeField] Vector3 moveDir;
    public bool MoveTo(Vector3 dir, bool inToMove = false)
    {
        if(isMove) return false;
        moveDir = dir;  
        if(CheckEmpty(transform.position + dir,!inToMove))
        {
            _rice = false;
            isMove = true;

            // memory.RegistPoint(this, transform.position, PlatformState,inToMove);
            FindLostPoint(moveDir);
            return true;
        }
        return false;

    }
    private bool _rice;
    public void FindLostPoint(Vector3 dir)
    {

        int rockForvard = 0;
        targetPoint = transform.position + dir;
        bool ice = false;
        bool dipth = false;
        // _bonck = false;
        if( CheckRockInMove(targetPoint) == 1) rockForvard++;
        while(true)
        {
            int tipe = CheckFallen(targetPoint);

            if(tipe == 1) {
                if(rockForvard > 0)
                {
                    rockForvard--;
                    targetPoint += dir;
                    continue;
                } 
                else {
                    dipth = true; break; 
                }
            }
            
            else if(tipe == 2)
            {
                ice = true;
                int p = CheckRockInMove(targetPoint + dir); // на випередження. зберігаючи попередній таргет як перевірений
                // якщо сканувати поточний то всі куби зіллються в один
                if(p == 2) 
                {
                    // _bonck = true; _rice = true;
                    break;
                }
                else if(p == 1) rockForvard++;

                targetPoint += dir;
                continue;
            } 
            else break;      
        }
        // перерахунок якщо глибина
        if(ice) curentSpd = spd * 1.5f;
        else curentSpd = spd;

        if(dipth) return;
        else targetPoint -= moveDir * rockForvard;
        // if(rockForvard > 0) _bonck = false;

    }
public void UncontrolMove(Vector3 _point)
    {
        Move(_point);
        if(isPlace(_point))
        {
            PlaceControle();
            if(!PlatformState && _rice) MoveTo(moveDir, true);
        }
        

            
    }
    private bool isPlace(Vector2 place) => place.x == transform.position.x && place.y == transform.position.y;
    void Move(Vector3 target)
    {
        float newspd = Time.fixedDeltaTime * curentSpd;
        Vector3 newPos = Vector2.MoveTowards(transform.position, target, newspd);
        transform.position = newPos;
        sp.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10);
        CheckButton(newPos);
    }  



    
    [SerializeField] float alphaDuration = 0.4f;
    public void StartAlphaAnim(Vector3 next, bool state) => StartCoroutine(AlphaAnim(next, state));
    private bool curentActiveState = false;
    public void StartActivateAnim(bool state)
    {
        if(state == curentActiveState) return;
        curentActiveState = state;

        bool newt = true;
        
        if(activate != null) {StopCoroutine(activate); newt = false;}
        activate = StartCoroutine(ActiveAnim(state, newt));
    }
    WaitForFixedUpdate fix = new WaitForFixedUpdate();
    private bool noInteractive;
    IEnumerator AlphaAnim(Vector3 nextPoint, bool state)
    {
        memory.pl.SetNoInteractiv(true);
        noInteractive = true;

        float t = alphaDuration;
        while (t > 0)
        {
            yield return fix;
            t -= Time.fixedDeltaTime;
            float n = t / alphaDuration;
            
            SetAlpha(a:n);
        }
        SetPos(nextPoint);

        t = 0;
        float alphaMod = NOAlpfa? 1 : 0.65f;
        while (t < alphaDuration * alphaMod)
        {
            yield return fix;
            t += Time.fixedDeltaTime;
            float n = t / alphaDuration;
            
            SetAlpha(a:n);
        }
        noInteractive = false;
        memory.pl.SetNoInteractiv(false);
    }  
        public void SetAlpha(SpriteRenderer obj = null, float a = 0)
    {
        if(obj == null) obj = sp;
        obj.color = new Color(obj.color.r,obj.color.g,obj.color.b, a);
    }


    [SerializeField] SpriteRenderer activeObj;
    [SerializeField] float activeTime;
    [SerializeField] float act_t = 0;
    IEnumerator ActiveAnim(bool state, bool newt = true)
    {
        if(noInteractive == false)
        {
            activeObj.sortingOrder = sp.sortingOrder + 1;
            SetAlpha(activeObj, state? 0 : 1);
            activeObj.enabled = true;
        }
            
        if(state == true)
        {
            if(newt) act_t = 0;
            if(noInteractive == false)
            {
                while (act_t < activeTime)
                {
                    yield return fix;
                    act_t += Time.fixedDeltaTime;
                    float n = act_t / activeTime;
                    
                    SetAlpha(activeObj, n);
                }
            }
            sp.sprite = ActiveArt;

        }
        else if(state == false)
        {
            if(newt) act_t = activeTime;

            sp.sprite = RockArt;
            if(noInteractive == false)
            {
                while (act_t > 0)
                {
                    yield return fix;
                    act_t -= Time.fixedDeltaTime;
                    float n = act_t / activeTime;
                    
                    SetAlpha(activeObj, n);
                }
            }   
        }
        activeObj.enabled = false;
    }  


    /////////////////////////
    // ------------------- //
    //     SCAN METHOD     //
    // ------------------- //
    /////////////////////////

    public int CheckFallen(Vector3 _point)
    {
        if(Physics2D.OverlapPoint(_point, LayerMask.GetMask("Dipth")) 
                && !Physics2D.OverlapPoint(_point, PlatformLayer)) return 1;
        else if(Physics2D.OverlapPoint(_point, LayerMask.GetMask("Ice"))) return 2;
        else return 0; 
    }


    public void PlaceControle()
    {
        bool state = CheckFallen(transform.position) == 1;
        if(sp.sprite == PlatformArt && !state || sp.sprite != PlatformArt && state) State(state);
        isMove = false;
        MoveTo(moveDir, true);

    }
    public void State(bool _plane)
    {
        PlatformState = _plane;
        sp.sprite = _plane? PlatformArt : RockArt;
        isMove = false;
        Acoll.enabled = _plane;
        Hcoll.isTrigger = _plane;
        if(_plane)
        {
            gameObject.transform.localScale = new Vector3(1.1f,1.1f,0);
            sp.sortingOrder = -30000;
        }
        else
        {

            sp.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10);
            gameObject.transform.localScale = new Vector3(1f,1f,0);
        }
        
    }

    public bool CheckEmpty(Vector3 _point, bool JustCheck = false)
    {
        var coll = CheckCollider(_point);
            if(coll == null) return true;
            else if (JustCheck) return false;
            else
            {
                sound.RockSound(1);

                Rock r;
                if(coll.TryGetComponent<Rock>(out r))
                { 
                    r.MoveTo(moveDir, true);
                    return false;
                }
                else return false;
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
    private Collider2D CheckCollider(Vector3 point)
    {
        Collider2D coll = Physics2D.OverlapPoint(point, LayerMask.GetMask("Wall"));
        if(coll != null && !coll.isTrigger) return coll;
        
        coll = Physics2D.OverlapPoint(point, LayerMask.GetMask("Rook"));
        if(coll != null && !coll.isTrigger) return coll;

        return null;
        
    } 
    Coroutine activate;
    private Button curentButton;
    public void CheckButton(Vector3 point)
    {
        Collider2D coll = Physics2D.OverlapPoint(point, LayerMask.GetMask("Button"));
        if(CompareButton(coll))
        {
            curentButton?.ChengPresed(false, null);
            curentButton = coll.GetComponent<Button>();
            curentButton.ChengPresed(true, this);

        }
        else if(coll == null && curentButton != null)
        {
            StartActivateAnim(false);

            curentButton.ChengPresed(false, null);
            curentButton = null;
        }
    }
    public void ForgiveMyRock(Button b)
    {
        if(curentButton != b) return;
        curentButton = null;
        sp.sprite = RockArt;
    }
    private bool CompareButton(Collider2D c)
    {
        if(c == null) return false;
        else if(curentButton == null) return true;
        else if(c.name == curentButton.name) return false;
        else return true;
    }

    


}