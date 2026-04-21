using System.Collections;
using UnityEngine;

public class Optimization : MonoBehaviour {
    Movement mc;
    [SerializeField]FishSplash fish;

    [SerializeField] Transform Lavel0;
    [SerializeField] Transform Lavel1;
    [SerializeField] Transform Lavel2;
    [SerializeField] Transform Lavel3;
    [SerializeField] MusicThemeControler music;

    public bool inLavel;
    [SerializeField] bool DontNidMatematic;

    private void Start() {
        mc = GetComponent<Movement>();
        ClosestLavel = Lavel0;
        if(!DontNidMatematic)
        {
            StartCoroutine(CheckDist());
            music.in2Lavel();
        }
        else music.in1Lavel();
            
    }
    void Update()
    {
        if(inLavel && Physics2D.OverlapCircle(transform.position,0.3f,mc.s.EndArea))
        {
            mc.lavelMode(inLavel:false);
            music.inAbuse();
        }
        if(NotFindeAbuse &&  Physics2D.OverlapCircle(transform.position,0.3f,mc.s.FishTrigger)) ControledFish();
        

    }


    public bool borderLeands = false;
    public bool TheEnd = false;
    [SerializeField] private float ActivDist = 625;
    [SerializeField] private float dist1;
    [SerializeField] private float dist2;
    [SerializeField] private float dist3;

    bool a = false;
    bool b = false;
    bool c = false;
    bool d = true;
    public Transform ClosestLavel; 
    private void HasBorder()
    {
        float zero =  Vector3.SqrMagnitude(Lavel0.position - transform.position);
        float Closestdist = zero; 
        ClosestLavel = Lavel0;
        
        if(dist1 < Closestdist) 
        {
            Closestdist = dist1; ClosestLavel = Lavel1;
        }
        if(dist2 < Closestdist) 
        {
            Closestdist = dist2; ClosestLavel = Lavel2;
        }
        if(dist3 < Closestdist) 
        {
            Closestdist = dist3; ClosestLavel = Lavel3;
        }

        borderLeands = zero > 3600 && Closestdist > 625;
        if(borderLeands) ChengedMusic();
    }
    public void FishAttak()
    {
        borderLeands = false;
        ChengedMusic();
        mc.ReturnToaMap(ClosestLavel.position);
        ClosestLavel.gameObject.SetActive(true);
    }
    IEnumerator CheckDist()

    {
        while (true)
        {
            yield return timer;

                if(inLavel && d)
                {
                    Lavel0.gameObject.SetActive(false); d = false;
                }
                else if(!inLavel && !d)
                {
                    Lavel0.gameObject.SetActive(true); d = true;
                }

            if(inLavel) continue;

            dist1 =  Vector3.SqrMagnitude(Lavel1.position - transform.position);
            dist2 =  Vector3.SqrMagnitude(Lavel2.position - transform.position);
            dist3 =  Vector3.SqrMagnitude(Lavel3.position - transform.position);
            HasBorder();
            if(borderLeands)
            {
                Vector3 pos = transform.position + (transform.position - ClosestLavel.position).normalized * 10;
                FishSplash f = Instantiate(fish);
                f.imortal = false;
                f.transform.position = pos;
                
                continue;
            }
            
            bool in1Area = dist1 < ActivDist;
            bool in2Area = dist2 < ActivDist;
            bool in3Area = dist3 < ActivDist;

            
            if(in1Area && !a)           { Lavel1.gameObject.SetActive(true); a = true;      }
            else if (!in1Area && a)     { Lavel1.gameObject.SetActive(false); a = false;    }

            if(in2Area && !b)           { Lavel2.gameObject.SetActive(true); b = true;      }
            else if(!in2Area && b)      { Lavel2.gameObject.SetActive(false); b = false;    }

            if(in3Area && !c)           { Lavel3.gameObject.SetActive(true); c = true;      }
            else if(!in3Area && c)      { Lavel3.gameObject.SetActive(false); c = false;    }
            
        }
    }
    WaitForSeconds timer = new WaitForSeconds(1);
    // public bool gizmo = false;
    // void OnDrawGizmos()
    // {
    //     if(!gizmo) return;
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(Lavel0.position, 25);
    //     Gizmos.DrawWireSphere(Lavel1.position, 25);
    //     Gizmos.DrawWireSphere(Lavel2.position, 25);
    //     Gizmos.DrawWireSphere(Lavel3.position, 25);
    //     Gizmos.color = Color.magenta;
    //     Gizmos.DrawWireSphere(Lavel0.position, 60);

    // }
    [Header("NotFindeAbuse")]
    [SerializeField] Dore[] ClosestDore;
    [SerializeField] GameObject TriggerFish;
    [SerializeField] GameObject TriggerBedEnd;
    private bool NotFindeAbuse = true;

private void ControledFish()
{
    foreach (var d in ClosestDore)
    {
        d.Closed = false;
        d.OpenDore(true);
    }
    ClosestLavel = Lavel0;
    Vector3 pos = transform.position + (transform.position - ClosestLavel.position).normalized * 10;
    FishSplash f = Instantiate(fish);
    f.imortal = false;
    f.transform.position = pos;
    NotFindeAbuse = false;
    TriggerFish.SetActive(false);
    TriggerBedEnd.SetActive(false);
}


    internal void ChengedMusic()
    {
        if(DontNidMatematic) return;
        if(TheEnd) music.inNotMenyLavels();
        if(borderLeands) music.inBorderleands();
        else if(!inLavel)music.inAbuse();
        else if (ClosestLavel == Lavel1) music.in1Lavel();
        else if (ClosestLavel == Lavel2) music.in2Lavel();
        else if (ClosestLavel == Lavel3) music.in3Lavel();
    }
}