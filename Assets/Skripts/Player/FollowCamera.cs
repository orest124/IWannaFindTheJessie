
using UnityEngine;

public class FollowCamera : MonoBehaviour{
    
    private Movement pl;
    [Header("Values")]
    [SerializeField] private float dist;
    [SerializeField] float corSpd;


    const float fix = 0.02f;
    
    [Header("Shake")]
    [SerializeField] public bool shake;
    [SerializeField] int shaceCount;
    [SerializeField]float shaceSpd;
    [SerializeField] float IntensiwShake;


    private void Awake() {
        pl = FindAnyObjectByType<Movement>();
    }


    private void FixedUpdate() {

        if(shake) ShakeTimer();
        FollowC(pl.transform.position);
    }

    
    public void FollowC(Vector3 point)
    {
        if(point == transform.position) return;


        dist = Vector3.SqrMagnitude(transform.position - new Vector3(point.x,point.y, transform.position.z));
        float _spd = GetSpd(dist);
    
        // if(dist > ModSpdDist) _spd = _spd * MaxSpd;
        Vector3 newpoint = Vector3.MoveTowards(transform.position, new Vector3(point.x, point.y, transform.position.z), _spd);
        transform.position = newpoint;
    }

    private float tempspd = 1;
    private float GetSpd(float _dist)
    {
        // if(tempspd >= minSpd) return minSpd;
        // else
        // {
        //     tempspd += minSpd * 0.05f;
        //     return tempspd;
        // }
        float dist = _dist / corSpd;
        return Time.fixedDeltaTime * (dist > 0.01? dist : 0.01f);

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