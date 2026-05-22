using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Prize : MonoBehaviour
{
    public LavelInfo myLavel;
    public Animator anim;
    public Image origin;
    [SerializeField]TextMeshProUGUI textProfit;


    public int ID = 0;
    public int Profit;
    [HideInInspector]public bool Present = false;

    PrizeControl pc;
    [SerializeField]private Sprite[] PrizeImage;
    private Image Prize_I;
    [SerializeField] private SpriteRenderer Prize_R;

    void Awake()
    {
        pc = FindFirstObjectByType<PrizeControl>();
        pc.RegistPrize(this);
        Prize_I = GetComponent<Image>();
        if(Prize_R != null)
        {
            Prize_R.gameObject.AddComponent<PrizeSercher>().prize = this;
        }
        SetImage(404);
        gameObject.SetActive(false);
    }
    public void SetImage(int profit)
    {
        int tipe = 0;
        if(profit < -21 || profit > 403) tipe = 0;
        else if(profit <= -10  ) tipe = 1;
        else if(profit > -10 && profit < 10 ) tipe = 2;
        else if(profit >= 10 ) tipe = 3;

        SetImage(PrizeImage[tipe]);
        int n = profit== 404? 0:profit;
        Profit = profit;
        if(Profit != 404) pc.SavePrize();
        if(textProfit == null) return;
        textProfit.text = profit == 404? "X" : profit.ToString();
    }
    private void SetImage(Sprite s)
    {
        Prize_I.sprite = s;
        if(Prize_R != null) Prize_R.sprite = s;
    }
    private void SetState(bool s)
    {
        present = s;
        Prize_I?.gameObject.SetActive(s);
        if(Prize_R != null) Prize_R?.gameObject.SetActive(!s);
    }
    public void Preparation(char[] key)
    {
        getAllInfoWithKey(key);
    }
    public bool present;
    public char[] GetKey()
    {
        int n = Profit < 0? 1:0;
        string ne = $"{ID}{Profit}{n}{(present? 7:4 )}";
        char[] nne = ne.ToCharArray();
        nne.Reverse();
        return nne; 
    }
    private void getAllInfoWithKey(char[] c)
    {
        c.Reverse();
        string key = new string(c);
        Profit = c[c.Length -2] == '1'? -Profit : Profit;
        Profit = int.Parse(key[(Profit<0?3:2)..(c.Length -2)]);
        SetState(c[c.Length -1] == '7'? true : false);
        SetImage(Profit);
    }
    



    public void StartPresentation() 
    {
        myLavel.ClousedScore();
        SetState(true);
        origin.sprite = Prize_I.sprite;
        anim.SetTrigger("Fanfar");
    }
}
public class PrizeSercher : MonoBehaviour
{
    public Prize prize;
}
