using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class PrizeControl : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textProffit;
    public Dictionary<int,Prize> prizes = new();
    public void RegistPrize(Prize p) => prizes.Add(p.ID,p);
    private const string pathToPrize = "/sens.json";

    private void Start() {
        LoadPrize();
    }

    
    public void LoadPrize()
    {
        List<JsonPrize> ps = GetJson();
        if(ps == null) return;
        foreach (var p in ps)
        {
            if(prizes.ContainsKey(p.ID)) prizes[p.ID].Preparation(p.key);
        }
        if(textProffit != null)
        {
            int n = 0;
            foreach (var p in prizes)
            {
                if(!p.Value.present || p.Value.Profit == 404) continue;
                n += p.Value.Profit;
            }
            textProffit.text = n.ToString();
        }
    }


    public void SavePrize()
    {
        List<JsonPrize> preSaves = GetJson();
        if(preSaves == null) preSaves = new();
        foreach (var p in prizes)
        {
            JsonPrize newJson = new JsonPrize(p.Value.ID,p.Value.GetKey());
            bool fiend = false;
            for (int i = 0; i < preSaves.Count; i++)
            {
                if(preSaves[i].ID == newJson.ID) 
                {
                    preSaves[i] = newJson;
                    fiend = true;
                    break;
                }
            }
            if(!fiend) preSaves.Add(newJson);
        }
        string text = JsonConvert.SerializeObject(preSaves);
        File.WriteAllText(Application.dataPath + pathToPrize, text);
        GC.Collect();
    }


    private List<JsonPrize> GetJson()
    {
        if(!File.Exists(Application.dataPath + pathToPrize)) return null;
        
        string text = File.ReadAllText(Application.dataPath + pathToPrize);
        if(string.IsNullOrEmpty(text)) return null;

        List<JsonPrize> temp;
        try
        {
            temp = JsonConvert.DeserializeObject<List<JsonPrize>>(text);
        }
        catch
        {
            File.WriteAllText(Application.dataPath + pathToPrize,string.Empty);
            return null;
        }
        return temp;
    }
}
public struct JsonPrize
{
    public int ID;
    public char[] key;
    public JsonPrize(int id, char[] k)
    {
        ID = id;
        key = k;
    }
    
}
