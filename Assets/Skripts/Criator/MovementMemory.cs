using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class MovementMemory : MonoBehaviour
{
    public List<Dore> dores;
    private Dore curentDore;
    public Movement pl;
    public List<PersonStepInfo> Steps => curentDore.steps;
    private int stepCount { get => curentDore.stepCount; set => curentDore.stepCount = value;}
    [SerializeField] private TextMeshProUGUI _score;

    ContactFilter2D fl;
    void Awake()
    {
        fl.SetLayerMask(LayerMask.GetMask("Rook"));
        fl.useTriggers = true;
        
        Dore[] _dores = FindObjectsByType<Dore>(FindObjectsSortMode.None);
        dores = _dores.Where(d => d.Prime == true).ToList();
        foreach (var d in dores) {
            foreach (var r in d.DinamicObj) r.memory = this;
            d.memory = this;
        }

        pl = FindAnyObjectByType<Movement>();
        pl.memory = this;
        curentDore = pl.curentDore;
        
        IncrementMove(0);
        
        
    }
    public Vector3 mousePos ;
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CheckAllRock(mousePos);
            
            if(tempRock.Count != 0) print(tempRock[^1].name);
            
        }
        if(Input.GetMouseButtonDown(2))
        {
            ClearRock();
        }
    }
    public void NewDore(Dore d)
    {
        curentDore = d;
        IncrementMove(0);
    }


    public void RegistPoint(Rock r, Vector3 point)
    {
        Steps.Add(new PersonStepInfo(stepCount, r.transform, point));
    }
    public void RegistPoint(Movement p, Vector3 point)
    {
        IncrementMove();
        Steps.Add(new PersonStepInfo(stepCount, p.transform, point, false));
    }

        public void IncrementMove(int point = 1)
    {
        stepCount+= point;
        _score.text = $"{stepCount}";
    }

    public void StepBihaind()
    {
        if(Steps.Count == 0) return;
        PersonStepInfo i = Steps[Steps.Count -1];
        int nomb = i.step;
        while (i.step == nomb)
        {
            Steps.Remove(i);
            i.RemovPos();
            if(Steps.Count == 0) break;

            i = Steps[Steps.Count-1];
            if(nomb != i.step) break;
        }
        foreach (var r in curentDore.DinamicObj) r.Restart(timeLaps: true); 
        PlayerState();
    }
    public void PlayerState()
    {
        pl.isMove = false;
        pl.SetStop(false);
    }



    
    public void AbsolutRestart()
    {
        foreach (var d in dores)
        {
            d.Restartlavel(true);
            d.stepCount = 0;
            d.steps.Clear();
        }
        IncrementMove(0);
        pl.Idle(newGame: true, true);
    }
    public void LocalClining()
    {
        if(curentDore.AllDone == false) return;
        
        pl.Idle();
        ReturnAllRockInLavel(curentDore.startPos.transform.position);
        
        
    }
    public void RestartLavel()
    {
        curentDore.steps.Clear();
        curentDore.stepCount = 0;
        IncrementMove(0);

        pl.Idle();
        ReturnAllRockInLavel(curentDore.startPos.transform.position);
        
        curentDore.Restartlavel(restLavel:true);
    }

    public void DoMemory()
    {
        
    }
    List<Vector3> OPEN = new();
    List<Vector3> CLOUSED = new();

    public void ReturnAllRockInLavel(Vector3 startPoint, bool inMemory = false)
    {
        OPEN.Clear();
        CLOUSED.Clear();
        OPEN.Add(startPoint);
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
        if(inMemory) { foreach (var R in tempRock) curentDore.memoryAtRock.Add(new MemoriAtRock(R, R.transform.position)); }
        
        else ClearRock();

    }
    List<Collider2D> tempRock = new List<Collider2D>();
    List<Collider2D> temp = new List<Collider2D>();
    private bool CheckWall(Vector3 point) => Physics2D.OverlapCircle(point,0.3f, LayerMask.GetMask("Wall"));
    private void CheckAllRock(Vector3 point)
    {
        Physics2D.OverlapCircle(point, 0.2f, fl,temp);
        foreach (var i in temp)
        {
            if(!tempRock.Contains(i))
                tempRock.Add(i);
        }
    }
    private void ClearRock()
    {
        Rock rc;
        int i = 0;
        while(tempRock.Count > 0)
        {
            i++;
            if(i > 10000) break;
            
            if(tempRock[0].TryGetComponent<Rock>(out rc))
            {
                CheckAllRock(rc.StartPos);
                tempRock[0].transform.position = rc.StartPos;
                tempRock.Remove(tempRock[0]);
            }
        }
        
    }
    
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
}


public struct PersonStepInfo
{
    public bool isRock;
    public int step;
    public Transform person;
    public Vector3 point;
    public PersonStepInfo(int _s, Transform ps, Vector3 pos, bool isRock = true)
    {
        step = _s;
        person = ps;
        point = pos;
        this.isRock = isRock;
    }
    public void RemovPos() => person.position = point;
    
    
}