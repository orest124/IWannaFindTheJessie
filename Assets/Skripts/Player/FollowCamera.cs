
using UnityEngine;

public class FollowCamera : MonoBehaviour{
    
    private Movement pl;
    [Header("Values")]
    [SerializeField] private float dist;
    [SerializeField] private float curentSpd;
    [Space]
    [Space]
    [SerializeField] private float MaxSpd;
    [SerializeField] private float ModSpdDist;
    [SerializeField] private float lookForwardDist;

    const float fix = 0.02f;
    
    [Header("Shake")]
    [SerializeField] public bool shake;
    [SerializeField] int shaceCount;
    [SerializeField]float shaceSpd;
    [SerializeField] float IntensiwShake;


    private void Awake() {
        pl = FindAnyObjectByType<Movement>();
        curentSpd = MaxSpd;
    }


    private void FixedUpdate() {

        if(shake) ShakeTimer();
        FollowC(pl.transform.position);
    }


    public void FollowC(Vector3 point)
    {
        float _spd = Time.fixedDeltaTime;
        dist = Vector3.SqrMagnitude(transform.position - new Vector3(point.x,point.y, transform.position.z));
        if(dist > ModSpdDist) _spd = _spd * MaxSpd;
        if(dist > ModSpdDist + 9) _spd = _spd * (MaxSpd + 4);
        Vector3 newpoint = Vector3.MoveTowards(transform.position, new Vector3(point.x, point.y, transform.position.z), _spd);
        transform.position = newpoint;
    }

    private float ts = 1;
    private void GetSpd(Vector3 point)
    {
        Vector3 Pos = transform.position;

        dist = Vector3.SqrMagnitude(Pos - new Vector3(point.x,point.y, Pos.z));

        if(dist > ModSpdDist) ts += fix;
        else  ts = MaxSpd;
        curentSpd = fix * ts;

    }
    private Vector3 GetPoint()
    {
        return pl.transform.position; 
    }


    float t;
    private int ShakeState;
    public void ShakeTimer()
    {
        t += fix;
        if(t > shaceSpd)
        {
            ShakeProces();
            t = 0;
        }
        
    }
    private void ShakeProces()
    {
        ShakeState++;
        float polar = ShakeState %2 == 0? 1 : -1;
        float x = transform.position.x + IntensiwShake * polar;
        
        transform.position = new Vector3(x, transform.position.y, transform.position.z);

        if(ShakeState >= shaceCount) 
        {
            ShakeState = 0;
            shake = false;
        }
    }
    [SerializeField] bool gizmo = false;

    void OnDrawGizmos()
    {
        if(!gizmo) return;
            Vector3 p = GetPoint();
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere (p, 0.3f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine (p + new Vector3(0.15f,0f),p + new Vector3(-0.15f,0f));
            Gizmos.DrawLine (p + new Vector3(0,0.15f), p+ new Vector3(0,-0.15f));
    }
}