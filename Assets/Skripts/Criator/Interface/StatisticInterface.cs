using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatisticInterface : MonoBehaviour {
    public float spd = 1;
    [Header("Counter")]
    public LavelInfo[] lavels;
    [SerializeField] Image Imagine;
    [SerializeField] TextMeshProUGUI countText;
    public Color Gold;
    public Color Silver;
    public Color Coper;
    public Color Platinum;

    [Space] [Space]
    [Header("Coints")]
    [SerializeField] Transform CointOrigin;
    [SerializeField] TextMeshProUGUI CoperText;
    [SerializeField] TextMeshProUGUI SilverText;
    [SerializeField] TextMeshProUGUI GoldText;
    [SerializeField] Transform[] Coints;
    [Space] [Space]

    [Header("Beck")]
    public GameObject Statistic;
    [SerializeField] Transform P1;
    [SerializeField] Transform P2;
    bool state = false;
    bool isMove = false;
    void Awake()
    {
        fix = new WaitForFixedUpdate();
        foreach (var item in lavels) item.Preparation(this);
    }
    public void Flip(int t = 0)
    {
        if(isMove) return;
        isMove = true;
        state = !state;
        StartCoroutine(FlipProces(t == 0?state : false));
    }
    WaitForFixedUpdate fix;
    IEnumerator FlipProces(bool st)
    {
        if(st == true)
        {
            foreach (var c in Coints)
            {
                float cor = Random.Range(-15,16);
                c.position = new Vector3(CointOrigin.position.x + cor, c.position.y, c.position.z);
            }
        }
        Transform t = Statistic.transform;
        while (t.position != (st? P1.position : P2.position))
        {
            Vector3 newPos = Vector3.MoveTowards(t.position,st? P1.position : P2.position, Time.fixedDeltaTime * spd);
            t.position = newPos;
            yield return fix;
        }
        isMove = false;

    }






    public void CountPreparation(int value = -1)
    {
        if(value == -1)
        {
            CoperText.text = ">1";
            SilverText.text = "<0";
            GoldText.text = "-4";
            //Зробити нагороду якщо гравець добється числа -4
            return;
        }
            CoperText.text = (value + 10).ToString();
            SilverText.text = (value + 3).ToString();
            GoldText.text = value.ToString();
            
    }

    public void SetStatistic(Dore d) 
    {
        if(d == null)
        {
            countText.text = "X";
            Imagine.color = Platinum;
            CountPreparation(-1);
            return;
        }
        SetCount(d);
        CountPreparation(d.DontNidStatistic? -1 : d.GetCountToGold());

        
    }
    public void SetCount(Dore d)
    {

        int nomb = d.DontNidStatistic? -1 : d.StepCount;
        countText.text = nomb > 0? nomb.ToString() : "X";
        if(_sake == false) StartCoroutine(Shake());
        
        int dif = d.GetDifference();
        if(nomb == -1) Imagine.color = Platinum;
        else if(dif > -3 && dif <=0) Imagine.color = Gold;
        else if(dif > 0) Imagine.color = Platinum;
        else if(dif < -9) Imagine.color = Coper;
        else if(dif < -2) Imagine.color = Silver;
    }
    bool _sake = false;

    [Header("Shake")]
    [SerializeField] public bool shake;
    [SerializeField] int shaceCount;
    [SerializeField]float shaceSpd;
    [SerializeField] float IntensiwShake;

    private void ShakeProces()
    {
        float polar = ShakeState %2 == 0? 1 : -1;
        Vector3 pos = countText.transform.position;
        float x = pos.x + IntensiwShake * polar;
        countText.transform.position = new Vector3(x, pos.y, pos.z);

    }
    private int ShakeState;
    IEnumerator Shake()
    {
        _sake = true;
        ShakeState = 0;
        while (ShakeState <= shaceCount)
        {
            yield return new WaitForSeconds(shaceSpd);
            ShakeProces();
            ShakeState++;
            
        }
        _sake = false;
        

    }
}


[System.Serializable]
public class LavelInfo
{
    public Dore[] dores;
    private StatisticInterface stats;
    public void ClousedScore() => stats.Flip(1);
    [Space]
    [Space]

    public Prize prize;

    public int profit = 404;
    [Space]
    [Space]
    [SerializeField] private Button.PressedEvent SpecialAction = new Button.PressedEvent();
    public void GetRating()
    {
        int n = 0;
        foreach (var d in dores)
        {
            if(!d.AllDone) 
            {
                n = 404;
                break;
            }
            n += d.GetDifference();
        }
        profit = n;
        prize.SetImage(profit);
        
    }
    public void Preparation(StatisticInterface stats)
    {
        foreach (var d in dores) 
        {
            d.myLavel = this;
            this.stats = stats;
        }

        if (prize == null) return;
        prize.myLavel = this;
    }
    public void Activatemetod() => SpecialAction.Invoke(); 
}
