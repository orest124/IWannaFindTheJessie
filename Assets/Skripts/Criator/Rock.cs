using System.Collections;
using NUnit.Framework;
using UnityEngine;

public class Rock : MonoBehaviour 
{
    Vector2 StartPos;
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
    public void Restart()
    {
        enabled = true;
        State(false);
        StopAllCoroutines();
        isMove = false;
        transform.position = StartPos;
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
            // StartCoroutine(EMove(dir));
            moveDir = dir;
            targetPoint = transform.position + moveDir;
            isMove = true;
        }
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


    public void PlaceControle(Vector3 dir)
    {
        Vector3 myPoint = transform.position;

        _isIce = Physics2D.OverlapCircle(myPoint,0.3f, IceArea);
        _isDipth = Physics2D.OverlapCircle(myPoint, 0.3f, DipthArea) 
                && !Physics2D.OverlapPoint(myPoint, PlatformLayer);
        
        if(_isDipth) State(true);
        else if (_isIce) 
        {
            targetPoint = transform.position + dir;
            isMove = CheckEmpty(targetPoint);
        }
        else isMove = false;
    }

    private void State(bool _plane)
    {
        sp.sprite = _plane? PlatformArt : RockArt;
        isMove = false;
        Acoll.enabled = _plane;
        Hcoll.enabled = !_plane;
        sp.sortingOrder = _plane? 11:20;
        if(_plane)
        {
            gameObject.transform.localScale = new Vector3(1.1f,1.1f,0);
        }
        else
        {
            gameObject.transform.localScale = new Vector3(1f,1f,0);
        }
        
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

        void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(castRadius,castRadius));
    }


}