using System.Collections;
using NUnit.Framework;
using UnityEditor.Tilemaps;
using UnityEngine;

public class Rock : MonoBehaviour 
{
    public Vector2 StartPos;
    [SerializeField] Collider2D Hcoll;
    [SerializeField] Collider2D Acoll;
    public LayerMask IceArea;
    public LayerMask DipthArea;
    public LayerMask PlatformLayer;
    public LayerMask BaricadeArea;
    SpriteRenderer sp;
    [SerializeField] Sprite PlatformArt;
    [SerializeField] Sprite RockArt;

    [SerializeField] float spd;
    [SerializeField]Movement pl;
    [HideInInspector] public MovementMemory memory;


    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        sp.sortingOrder = 80;

        StartPos = transform.position;
    }
    void FixedUpdate()
    {
        if(isMove) UncontrolMove(targetPoint);
    }
    public void Restart(bool timeLaps = false)
    {
        isMove = false;
        State(ChageToFallen(transform.position) == 1);
        if(!timeLaps) transform.position = StartPos;
    }


    [SerializeField] Vector3 targetPoint;
    [SerializeField] Vector3 moveDir;
    public void MoveTo(Vector3 dir, bool inToMove = false)
    {
        if(isMove) return;
        var hit = Physics2D.OverlapCircle(transform.position + dir, 0.3f,BaricadeArea);

        if(hit != null && !hit.isTrigger)
        {
            Rock r;
            if(inToMove && hit.TryGetComponent<Rock>(out r))
            {
                r.MoveTo(dir,inToMove);
            }
            isMove = false;
            return;

        }
        else
        {
            moveDir = dir;
            FindLostPoint(moveDir);
            pl.SetStop(true);
            isMove = true;
        }
    }
    public void FindLostPoint(Vector3 dir)
    {

        targetPoint = transform.position + dir;
        while(true)
        {
            int tipe = ChageToFallen(targetPoint);
            if(tipe == 1) break;
            
            else if(tipe == 2)
            {
                targetPoint += dir;
                continue;
            }
            else
            {
                if(!CheckEmpty(targetPoint))
                {
                    targetPoint -= dir;
                    break;
                }
                else break;
            }
        }
        memory.RegistPoint(this, transform.position);
    }
public void UncontrolMove(Vector3 _point)
    {
        Move(_point);
        if(isPlace(_point))
        {
            PlaceControle(moveDir);
        }
            
    }
    private bool isPlace(Vector2 place) => place.x == transform.position.x && place.y == transform.position.y;

    void Move(Vector3 target)
    {
        float newspd = Time.fixedDeltaTime * spd;
        Vector3 newPos = Vector2.MoveTowards(transform.position, target, newspd);
        transform.position = newPos;
    }    
    public int ChageToFallen(Vector3 _point)
    {
        bool _isIce = Physics2D.OverlapCircle(_point,0.2f, IceArea);
        bool _isDipth = Physics2D.OverlapCircle(_point, 0.2f, DipthArea) 
                && !Physics2D.OverlapPoint(_point, PlatformLayer);
        if(_isDipth) return 1;
        else if (_isIce) return 2;
        else return 0; 
    }


    public void PlaceControle(Vector3 dir)
    {
        Vector3 myPoint = transform.position;

        int tipe = ChageToFallen(myPoint);
        
        if(tipe == 1) State(true);
        else if (tipe == 2) 
        {
            targetPoint = transform.position + dir;
            isMove = CheckEmpty(targetPoint);
        }
            
        else {isMove = false;pl.SetStop(false);}

    }

    private void State(bool _plane)
    {
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
    public void RestPos(Vector3 _point)
    {
        transform.position = _point;
        State(ChageToFallen(transform.position) == 1);
    }
    public bool CheckEmpty(Vector3 _point)
    {
        var coll = CheckCollider(_point);
            if(coll == null || coll.isTrigger) return true;
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
    private Collider2D CheckCollider(Vector3 point) => Physics2D.OverlapCircle(point, 0.3f,BaricadeArea);
    
    
    public Vector3 oldPos;
    Vector3 MoveDir;
    [SerializeField] bool isMove;
    public bool _isIce;
    public bool _isDipth;
    
    bool MoveBreack;





    [SerializeField] float castRadius;

    


}