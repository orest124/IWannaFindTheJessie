using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MovementMemory : MonoBehaviour
{
    public List<Dore> dores;
    private Dore abusDore;

    private Dore curentDore;
    public void NewDore(Dore d) => curentDore = d;
    public Movement pl;
    [SerializeField] Button HardModButton;
    public List<PersonStepInfo> Steps = new();
    [SerializeField] int stepCount = 0;
    [SerializeField] int maxStepMember;
    [SerializeField] bool maxStep;

    ContactFilter2D fl;
    void Awake()
    {
        fl.SetLayerMask(LayerMask.GetMask("Rook"));
        fl.useTriggers = true;

        Dore[] _dores = FindObjectsByType<Dore>(FindObjectsSortMode.None);
        dores = _dores.Where(d => d.Prime == true).ToList();
        foreach (var d in dores) { if(d.name == "AbuseDoor") abusDore = d; }
        
        pl = FindAnyObjectByType<Movement>();
        pl.memory = this;
        curentDore = pl.curentDore;
        
        RemoveMemory();
    }

    
        void W() => AbsolutRestart();
        void R() => RestartLavel();
        void E() => LocalClining();
        void A() => RemoveMemory();

        void D() => OnDrawGizmos();

        void U() => ReturnAllRockInLavel();

        void Y() => RegistPoint(null,false);
    

    /////////////////////////
    // ------------------- //
    //    MEMORY METHOD    //
    // ------------------- //
    /////////////////////////
    
    public void IncrementMove()
    {
        stepCount += 1;
        if(Steps.Count > maxStepMember) 
        {
            maxStep  = true;
        }
        else maxStep = false;
        
        if(maxStep) RemoveLastStep();
    }
    private void RemoveLastStep()
    {
        int nomb = Steps[0].step;
        int f = 0;
        while (true)
        {
            f++;
            if(f>50) {print("Stac Owerflow Exeption in Rmovelavel");return;}
            if(Steps.Count == 0) break;
            if(Steps[0].step == nomb)
            Steps.Remove(Steps[0]);
            else return;
        }
    }



    public void StepBihaind()
    {
        if(Steps.Count == 0) return;
        int nomb = Steps[^1].step;
        int f = 0;
        while (true)
        {
            f++;
            if(f>50) 
            {
                print("Stac Owerflow Exeption in Remove");return;
            }
            if(Steps.Count == 0) break;
            if(Steps[^1].step == nomb)
            {
                Steps[^1].PrewStep();
                Steps.Remove(Steps[^1]);
            }
            else break;
        }
        pl.isMove = false;
        pl.SetStop(false);
    }


    /////////////////////////
    // ------------------- //
    //    LAVEL METHOD     //
    // ------------------- //
    /////////////////////////

    public void AbsolutRestart()
    {
        RemoveMemory();
        pl.Idle(newGame: true, notFromLavel: true); // Якщо ню гейм в 2 зоні. Змінити на фолс
        foreach (var d in dores) d.LavelMod(newGame: true);
    }
    public void RestartLavel()
    {
        if(curentDore == abusDore) return;
        
        LocalClining();
        curentDore.LavelMod(restLavel:true);
        HardModButton.IsPressed = false;
    }
    public void LocalClining( bool forMemory = false)
    {
        RemoveMemory();
        pl.Idle();
        if(forMemory) { curentDore.LavelMod(forMemory:true); return; }
        ReturnAllRockInLavel();
    }
    public void RemoveMemory()
    {
        stepCount = 0;
        Steps.Clear();
        maxStep = false;
    }


    /////////////////////////
    // ------------------- //
    //        GIZMO        //
    // ------------------- //
    /////////////////////////
    public bool _gizmo = false;
        void OnDrawGizmos()
    {
        if (!_gizmo) return;
        foreach (var p in CLOUSED)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere (p, 0.3f);

            Gizmos.color = Color.red;
            Gizmos.DrawLine (p + new Vector3(0.15f,0f),p + new Vector3(-0.15f,0f));
            Gizmos.DrawLine (p + new Vector3(0,0.15f), p+ new Vector3(0,-0.15f));
            
        }
    }


    /////////////////////////
    // ------------------- //
    //     SCAN METHOD     //
    // ------------------- //
    /////////////////////////
    List<Collider2D> tempRock = new List<Collider2D>();
    List<Collider2D> temp = new List<Collider2D>();
    private List<Vector3> OPEN = new();
    private List<Vector3> CLOUSED = new();

    public void ReturnAllRockInLavel(bool inMemory = false)
    {
        OPEN.Clear();
        CLOUSED.Clear();
        OPEN.Add(curentDore.startPos.transform.position);
        Vector3 curentPoint;

        int i = 0;
        while(OPEN.Count > 0)
        { 
            i ++;
            if(i>10000) break;
            curentPoint = OPEN[0];
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    //
                    if(x != 0 && y != 0) continue;
                    int xv = (int)curentPoint.x + x; 
                    int yv = (int)curentPoint.y + y; 
                    Vector3 point = new Vector3(xv,yv);

                    if (CLOUSED.Contains(point) || OPEN.Contains(point)) continue;
                    else if (CheckWall(point)) CLOUSED.Add(point);
                    else OPEN.Add(point);
                    
                }
            }
            CheckAllRock(curentPoint);
            CLOUSED.Add(curentPoint);
            OPEN.Remove(curentPoint);
        }
        if(inMemory) 
        { 
            foreach (var R in tempRock) 
            {
                Rock r = R.GetComponent<Rock>();
                curentDore.memoryAtRock.Add(new MemoriAtRock(r, R.transform.position)); 
            }
        }
        
        else ClearRock();

    }
    private bool CheckWall(Vector3 point) => Physics2D.OverlapCircle(point,0.3f, LayerMask.GetMask("Wall"));
    private void ClearRock()
    {
        Rock rc;
        int i = 0;
        while(tempRock.Count > 0)
        {
            i++;
            if(i > 1000) break;
            
            if(tempRock[0].TryGetComponent<Rock>(out rc))
            {
                CheckAllRock(rc.StartPos);
                tempRock[0].transform.position = rc.StartPos;
                tempRock.Remove(tempRock[0]);
            }
        }
        
    }
    private void CheckAllRock(Vector3 point)
    {
        Physics2D.OverlapCircle(point, 0.2f, fl,temp);
        foreach (var i in temp)
        {
            if(!tempRock.Contains(i))
                tempRock.Add(i);
        }
    }



    /////////////////////////
    // ------------------- //
    // REGISTRATION METHOD //
    // ------------------- //
    /////////////////////////
    public void RegistPoint(Rock rock, Vector3 point, bool _state)
    {
        Steps.Add(new PersonStepInfo(stepCount, rock, point, _state));
    }
    public void RegistPoint(Movement p, Vector3 point)
    {
        IncrementMove();
        Steps.Add(new PersonStepInfo(stepCount, p, point, p.curentDore));
    }
    public void RegistPoint(Dore d, bool _state)
    {
        Steps.Add(new PersonStepInfo(stepCount, d, _state, d.AllDone? Vector3.one: Vector3.zero));
    }

}


public struct PersonStepInfo
{
    public int step;
    private int tipe;
    public Movement person; public Dore dore; public Rock rock;
    public Vector3 point;
    public bool state;
    public PersonStepInfo(int _s, Movement ps, Vector3 pos, Dore _dore)
    {
        step = _s;
        tipe = 1;
        person = ps;
        point = pos;
        dore = _dore;
        state = false; rock = null;
    }
    public PersonStepInfo(int _s, Rock _r, Vector3 pos, bool _state)
    {
        step = _s;
        tipe = 2;
        point = pos;
        rock = _r;
        state = _state;
        person = null; dore = null;
    }
    public PersonStepInfo(int _s, Dore _d, bool _state, Vector3 _allDone)
    {
        step = _s;
        tipe = 3;
        dore = _d;
        state = _state;
        point = _allDone; person = null; rock = null;
    }
    public void PrewStep()
    {
        if(tipe == 1) 
        {
            person.transform.position = point;
            person.lavelMode(dore);
        }
        else if(tipe == 2)
        {
            rock.transform.position = point;
            rock.State(state);
        }
        else 
        {

            dore.AllDone = point == Vector3.one;
            dore.OpenDore(state, true);
        }
        
    } 
    
    
}