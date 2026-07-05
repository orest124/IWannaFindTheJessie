using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MovementMemory : MonoBehaviour
{


    // Зробити 0 кнопок. Прайм зразу Алл дон, Звичайна відкриється
    // Перезапустити рівень
    // Засейвити і зробити откат до попереднього
    //зібрати трофей і подивитись чи він сохраняється


    private SaveSystem _save;
    
    private Dore abusDore;

    private Dore curentDore;
    public void NewDore(Dore d, bool Load = false)
    {
        curentDore = d;
        stats.SetStatistic(curentDore);
        if(Load)
        {
            stepCount = curentDore.StepCount;
            return;
        }
        if(curentDore.AllDone) return;
        else RemoveMemory();

    }

    [HideInInspector]public Movement pl;
    public List<PersonStepInfo> Steps = new();
    public int stepCount = 0;
    [SerializeField] int maxStepMember;
    [SerializeField] bool maxStep;
    [SerializeField] TextMeshProUGUI countText;
    private StatisticInterface stats;
    public void FlipCounter() => stats.Flip(0);
    public void SetStats() => stats.SetStatistic(curentDore);
    public Action GetSoundControler;
    

    ContactFilter2D fl;
    public void Awake_memory(Movement _pl,  Dore _abuseDore)
    {
        fl.SetLayerMask(LayerMask.GetMask("Rook"));
        fl.useTriggers = true;
        pl = _pl;
        abusDore = _abuseDore;

        stats = FindAnyObjectByType<StatisticInterface>();
    }

    
    
        void W() => ProtectWithBadDebut();
        void R() => RestartLavel();
        void E() => LocalClining();
        void A() => RemoveMemory();

        void D() => OnDrawGizmos();

        void U() => ReturnAllRockInLavel();
    

    /////////////////////////
    // ------------------- //
    //    MEMORY METHOD    //
    // ------------------- //
    /////////////////////////

    public void IncrementMove()
    {
        stepCount += 1;
        SetCurentDoreScore();
        if(Steps.Count > maxStepMember) 
        {
            maxStep  = true;
        }
        else maxStep = false;
        
        if(maxStep) RemoveLastStep();
    }
    public void SetCurentDoreScore()
    {
        if(curentDore.AllDone) return;
        curentDore.StepCount = stepCount;
        stats.SetCount(curentDore);
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


    // public void StepBihaind()
    // {
    //     if(Steps.Count == 0) return;
    //     PersonStepInfo i = Steps[^1];
    //     i.PrewStep();
    //     Steps.Remove(i);
    //     pl.isMove = false;
    //     pl.SetStop(false);
    // }

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
                PersonStepInfo i = Steps[^1];
                i.PrewStep();
                Steps.Remove(i);
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
public bool stopAll = false;
    public void ProtectWithBadDebut()
    {
        if(pl.IsLostDoor() || curentDore.AllDone)return;
        stopAll = true;
        LocalClining();
        curentDore.Restart(newGame: true);

        pl.SetOldDoor();
        LocalClining();
        stats.SetStatistic(curentDore);


    }
    public void RestartLavel()
    {
        if(pl.curentDore == abusDore)
            { pl.Idle(); return; }
        if(curentDore.AllDone) 
            { Restartlavel_AfterComplit(); return; }

        LocalClining();
        curentDore.Restart();

    }
        

    public void Restartlavel_AfterComplit()
    {
        LocalClining();
        curentDore.PutAllRock();
    }
    public void LocalClining()
    {
        RemoveMemory();
        pl.Idle();
        ReturnAllRockInLavel();
        
    }
    public void RemoveMemory(bool AllDone = false)
    {
        stepCount = 0;
        Steps.Clear();
        maxStep = false;
        if(!AllDone && !curentDore.AllDone) SetCurentDoreScore();
    }
    public int GetStepsCount() => stepCount;

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
        tempRock.Clear();
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
                RockModern r = R.GetComponent<RockModern>();
                MemoriAtRock mr = new MemoriAtRock(r, R.transform.position);
                curentDore.memoryAtRock.Add(mr); 
            }
        }
        
        else ClearRock();

    }
    private bool CheckWall(Vector3 point) => Physics2D.OverlapCircle(point,0.3f, LayerMask.GetMask("Wall"));
    private void ClearRock()
    {
        RockModern rc;
        int i = 0;
        while(tempRock.Count > 0)
        {
            i++;
            if(i > 1000) break;
            
            if(tempRock[0].TryGetComponent<RockModern>(out rc))
            {
                CheckAllRock(rc.StartPos);
                rc.SetPos();
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
    public void RegistPoint(RockModern rock, Vector3 point, bool _state, bool DontNidIncrement = false)
    {
        if(DontNidIncrement == false) IncrementMove();
        Steps.Add(new PersonStepInfo(stepCount, rock, point, _state));

    }
    public void RegistPoint(Movement p, Vector3 point, bool dontNidRegistr = false)
    {
        if(dontNidRegistr == false )IncrementMove();

        Steps.Add(new PersonStepInfo(stepCount, p, point));
    }




    public void IsSaveReady(RockModern r)
    {
        if(_save == null) {_save = GetComponent<SaveSystem>();_save.AddRock(r);}
        else _save.AddRock(r);
    }
    public void IsSaveReady(Dore d)
    {
        if(_save == null) {_save = GetComponent<SaveSystem>();_save.AddDoor(d);}
        else _save.AddDoor(d);
    }
    public void IsSaveReady(PhotoPictures f)
    {
        if(_save == null) {_save = GetComponent<SaveSystem>();_save.AddPict(f);}
        else _save.AddPict(f);
    }

}


public struct PersonStepInfo
{
    public int step;
    private int tipe;
    public Movement person; public RockModern rock;
    public Vector3 point;
    public bool state;
    public PersonStepInfo(int _s, Movement ps, Vector3 pos)
    {
        step = _s;
        tipe = 1;
        person = ps;
        point = pos;
        state = false; rock = null;
    }
    public PersonStepInfo(int _s, RockModern _r, Vector3 pos, bool _state)
    {
        step = _s;
        tipe = 2;
        point = pos;
        rock = _r;
        state = _state;
        person = null;
    }
    public void PrewStep()
    {
        if(tipe == 1) 
        {
            person.StartAlphaAnim(point);
        }
        else if(tipe == 2)
        {
            rock.StartAlphaAnim(point);
        }
        
    } 
    
    
}
