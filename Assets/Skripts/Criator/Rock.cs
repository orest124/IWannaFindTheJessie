using System.Collections;
using NUnit.Framework;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Rock : MonoBehaviour 
{
    void W() => UncontrolMove(new());
    void R() => Move(new());
    void E() => FindLostPoint(new());
    void A() => State(false);

    void D() => ButtonCheck(new());
    void Y() => PlaceControle();
    void U() => CheckEmpty(new());
    void T() => CheckCollider(new());

    SpriteRenderer sp;
    [SerializeField]Movement pl;
    [SerializeField] Collider2D Hcoll;
    [SerializeField] Collider2D Acoll;

    [SerializeField] float spd;
    public Vector2 StartPos;
    [SerializeField] bool isMove;
    [SerializeField] bool PlatformState;
    [SerializeField] Sprite PlatformArt;
    [SerializeField] Sprite RockArt;

    [HideInInspector] public MovementMemory memory;
    public LayerMask PlatformLayer;


    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        sp.sortingOrder = -Mathf.RoundToInt(transform.position.y * 10);
        memory = FindAnyObjectByType<MovementMemory>();
        sp.sortingOrder = 80;

        StartPos = transform.position;
    }
    void FixedUpdate()
    {
        if(isMove) UncontrolMove(targetPoint);
    }

    /////////////////////////
    // ------------------- //
    //   Movement METHOD   //
    // ------------------- //
    /////////////////////////
    public void SetPos(Vector3 point = new())
    {
        isMove = false;
        point = point == Vector3.zero? StartPos : point;
        transform.position = point;
        ChengPoint();
    }
    public void ChengPoint()
    {
        State(CheckFallen(transform.position) == 1);
        ButtonCheck(transform.position);
    }




    [SerializeField] Vector3 targetPoint;
    [SerializeField] Vector3 moveDir;
    public void MoveTo(Vector3 dir, bool inToMove = false)
    {
        if(isMove) return;
        moveDir = dir;  
        if(CheckEmpty(transform.position + dir,!inToMove))
        {
            pl.SetStop(true);
            stepCount = 0;
            _rice = false;
            isMove = true;

            memory.RegistPoint(this, transform.position, PlatformState);
            FindLostPoint(moveDir);
        }
    }
    private bool _rice;
    private int stepCount = 0;
    public void FindLostPoint(Vector3 dir)
    {

        targetPoint = transform.position + dir;
        while(true)
        {
            int tipe = CheckFallen(targetPoint);
            if(tipe == 1) break;
            
            else if(tipe == 2)
            {
                if(!CheckEmpty(targetPoint + dir, true)) 
                {
                    _rice = true;
                    break;
                }
                targetPoint += dir;
                continue;
            }
            else break;      
        }
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
        float newspd = Time.fixedDeltaTime * spd;
        Vector3 newPos = Vector2.MoveTowards(transform.position, target, newspd);
        transform.position = newPos;
        sp.sortingOrder = -((int)transform.position.y * 10);
        ButtonCheck(newPos);
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
        State(CheckFallen(transform.position) == 1);
        isMove = false;
        pl.SetStop(false);

    }
    public void State(bool _plane)
    {
        PlatformState = _plane;
        sp.sprite = _plane? PlatformArt : RockArt;
        isMove = false;
        Acoll.enabled = _plane;
        Hcoll.isTrigger = _plane;
        sp.sortingOrder = _plane? 11:20;
        if(_plane)
        {
            gameObject.transform.localScale = new Vector3(1.1f,1.1f,0);
            pl.SetStop(false);
        }
        else
        {
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
                Rock r;
                if(coll.TryGetComponent<Rock>(out r))
                {
                    r.MoveTo(moveDir, true);
                    return false;
                }
                else return false;
            }
    }
    private Collider2D CheckCollider(Vector3 point)
    {
        Collider2D coll = Physics2D.OverlapCircle(point, 0.3f,LayerMask.GetMask("Wall"));
        if(coll != null && !coll.isTrigger) return coll;
        
        coll = Physics2D.OverlapCircle(point, 0.3f,LayerMask.GetMask("Rook"));
        if(coll != null && !coll.isTrigger) return coll;

        return null;
        
    } 
    private Collider2D prewColl;
    private Button curentButton;

    private void ButtonCheck(Vector3 point)
    {
        Collider2D coll = Physics2D.OverlapPoint(point, LayerMask.GetMask("Button"));
        if(coll != null && prewColl != coll)
        {
            curentButton?.ChengPresed(false);
            prewColl = coll;
            curentButton = coll.GetComponent<Button>();
            curentButton.ChengPresed(true);
        }
        else if(coll == null && prewColl != null)
        {
            curentButton.ChengPresed(false);
            curentButton = null;
            prewColl = null;
        }
    }

    


}