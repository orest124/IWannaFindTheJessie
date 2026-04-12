
using System.Collections.Generic;
using System.IO;
using System;
using Newtonsoft.Json;
using UnityEngine;
using Random = UnityEngine.Random;

public class Example : MonoBehaviour 
{
    private const string STATE = nameof(STATE);
    private const string path = "/temp/memory.json";

    private GameState _gameState;

    void Start()
    {
        JsonProgectSeting.ApplayProgectSerializeationSeting();

        _gameState = LoadState();
    }
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Alpha1))
        {
            CreateRandomDoor();
            return;
        }
        if(Input.GetKeyUp(KeyCode.Alpha2))
        {
            CreateRandomItem();
            return;
        }
        if(Input.GetKeyUp(KeyCode.Alpha3))
        {
            CreateRandomCell();
            return;
        }
    }

    private GameState LoadState()
    {
        
        // var stateJson = PlayerPrefs.GetString(STATE);
        if(!File.Exists(Application.dataPath + "/" + path))
        {
            Directory.CreateDirectory(Application.dataPath + "/temp");
            File.WriteAllText(Application.dataPath + "/" + path, string.Empty);
        }
        var stateJson = File.ReadAllText(Application.dataPath + "/" + path);
        print("Load state" + stateJson);
        
        var state = string.IsNullOrEmpty(stateJson)
        ? new GameState { Entytys = new List<Entyty>() }
        : JsonConvert.DeserializeObject<GameState>(stateJson);

        print("Finaly load state" + JsonConvert.SerializeObject(state));

        return state;

    }
    public void SaveState()
    {
        var stateJson = JsonConvert.SerializeObject(_gameState);
        print("State to save" + stateJson);

        // PlayerPrefs.SetString(STATE, stateJson);
        File.WriteAllText(Application.dataPath + "/" + path, stateJson);
    }

    public void CreateRandomDoor()
    {
        var door = new DoorEntyty
        {
            ID = _gameState.GetNewId(),
            Type = EntytyType.Door,
            IsLoked = Random.Range(0,2) == 0,
            IsOpen = Random.Range(0,2) == 0,
        };
        _gameState.Entytys.Add(door);
        SaveState();
    }
    private string[] items = {"banana", "arrow", "sword", "pokemon"};
    public void CreateRandomItem()
    {
        var item = new PickupableEntyty
        {
            ID = _gameState.GetNewId(),
            Type = EntytyType.PickupableItem,
            ItemId = items[Random.Range(0, items.Length)],
            Amount = Random.Range(0, 999)
        };
        _gameState.Entytys.Add(item);

        SaveState();
    }
    public void CreateRandomCell()
    {
        
        var g = new GreadEntyty()
        {
            ID = _gameState.GetNewId(),
            Type = EntytyType.Character,

            gread = new PickupableEntyty[2,2]
        };
            
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    var item = new PickupableEntyty
                    {
                        ID = g.ID,
                        Type = EntytyType.PickupableItem,
                        ItemId = items[Random.Range(0, items.Length)],
                        Amount = Random.Range(0, 999)
                    };
                    g.gread[i,j] = item;
                }
            }

            
        _gameState.Entytys.Add(g);

        SaveState();
    }
    
}