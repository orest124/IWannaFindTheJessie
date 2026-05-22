
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class SaveSystem : MonoBehaviour 
{
    public bool inMeny;
    private const string STATE = nameof(STATE);
    private const string pathToDoor = "/Saves/4151518.json";
    private const string pathToRock = "/Saves/18151511.json";
    private const string pathToCharacter = "/Saves/Player Memory.json";

    private GameState _gameState;
    Movement pl;
    void Awake()
    {
        JsonProgectSeting.ApplayProgectSerializeationSeting();
        _gameState = new GameState { Entytys = new List<Entyty>() };
        
        if(!inMeny)pl = FindAnyObjectByType<Movement>();
        
    }
    public void LoadAllData()
    {
        _gameState = LoadState(pathToCharacter);
        ReturnCharacterMemory();
        _gameState.Entytys.Clear();
        
        _gameState = LoadState(pathToDoor);
        ReturnMemoryAtDore();
        _gameState.Entytys.Clear();

        _gameState = LoadState(pathToRock);
        ReturnMemoryAtRock();

        _gameState.Entytys.Clear();
    }


    private GameState LoadState(string path)
    {
        if(!File.Exists(Application.dataPath + "/" + path))
        {
            Directory.CreateDirectory(Application.dataPath + "/Saves");
            File.WriteAllText(Application.dataPath + "/" + path, string.Empty);
        }
        var stateJson = File.ReadAllText(Application.dataPath + "/" + path);
        GameState state;
        try {
            state = string.IsNullOrEmpty(stateJson)
            ? new GameState { Entytys = new List<Entyty>() }
            : JsonConvert.DeserializeObject<GameState>(stateJson);
        }
        catch
        { state = new GameState { Entytys = new List<Entyty>() }; }

        return state;
    }
    public bool CheckEmpty()
    {
        if(!File.Exists(Application.dataPath + "/" + pathToCharacter))
        {
            Directory.CreateDirectory(Application.dataPath + "/Saves");
            File.WriteAllText(Application.dataPath + "/" + pathToCharacter, string.Empty);
            return true;
        }
        var stateJson = File.ReadAllText(Application.dataPath + "/" + pathToCharacter);
        if(stateJson == string.Empty) return true;
        else return false;
    }
    public void SaveState(string path)
    {
        var stateJson = JsonConvert.SerializeObject(_gameState);
        
        File.WriteAllText(Application.dataPath + "/" + path, string.Empty);
        File.WriteAllText(Application.dataPath + "/" + path, stateJson);
    }
    public void SaveState()
    {
        SaveAllDoors();
        SaveAllRocks();
        SaveCharacter();

        pl.meny.SaveSceneIndex();
    }

    public void RemoveAllMemory()
    {
        File.WriteAllText(Application.dataPath + "/" + pathToDoor, string.Empty);
        File.WriteAllText(Application.dataPath + "/" + pathToRock, string.Empty);
        File.WriteAllText(Application.dataPath + "/" + pathToCharacter, string.Empty);
    }


    public void SaveAllDoors()
    {
        _gameState.Entytys.Clear();
        foreach (var door in dores) _gameState.Entytys.Add( door.Value.GetJson() );
        SaveState(pathToDoor);
    }
    private void ReturnMemoryAtDore()
    {
        foreach (JsonDoor i in _gameState.Entytys)
        {
            if(!dores.ContainsKey(i.name)) continue;
            Dore d = dores[i.name];
            d.LoadPreparation(i);
            d.RemoveMemoryAtRook();
            foreach (var r in i.memory)
            {
                Rock rock = rocks[r.name];
                d.memoryAtRock.Add(new MemoriAtRock(rock, r.GetPos()));
            }
        }
    }


    public void SaveAllRocks()
    {
        _gameState.Entytys.Clear();
        foreach (var r in rocks)
        {
            JsonRock jr = new JsonRock(r.Value.ID, r.Value.transform.position);
            _gameState.Entytys.Add(jr);
        }
        SaveState(pathToRock);
    }
    private void ReturnMemoryAtRock()
    {
        foreach (JsonRock i in _gameState.Entytys)
        {
            if(!rocks.ContainsKey(i.name)) continue;
            Rock r = rocks[i.name];
            r.SetPos(i.GetPos());
        }
    }    



    public void SaveCharacter()
    {
       _gameState.Entytys.Clear();
       JsonCharacter i = pl.GetPersonalmemory();
       _gameState.Entytys.Add(i);
        SaveState(pathToCharacter);
    }
    private void ReturnCharacterMemory()
    {
        JsonCharacter pi = (JsonCharacter)_gameState.Entytys[0];
        if(pi == null) return;
        pl.s.pc._photoColection.Clear();
        foreach (int i in pi.photoIDs) // PHOTO
        {
            PhotoPictures p = photos[i];
            pl.s.pc.AddPhoto(p, true);
            p.gameObject.SetActive(false);
        }
        pl.preLavels.Clear();
        foreach (var di in pi.DoorIDs) pl.preLavels.Add(dores[di]);
        pl.curentDore = pl.preLavels[^1];
        pl.StartPos = pl.curentDore.startPos.position;
        pl.memory.NewDore(pl.preLavels[^1], true);

        pl.SetPos(pi.GetVector());
        pl.CentralizedCamera();
        pl.inLavel = pi.inLavel;
    } 


    private Dictionary<int,Dore> dores = new();
    private Dictionary<int,Rock> rocks = new();
    public Dictionary <int,Dore> GetDoorArr() => dores;
    public void AddRock(Rock r) {
        if( !rocks.ContainsKey(r.ID) )  rocks.Add(r.ID,r);
    }
    public void AddDoor(Dore d) {
        if( !dores.ContainsKey(d.ID) )  dores.Add(d.ID, d);
    }
    Dictionary<int, PhotoPictures> photos = new();
    internal void AddPict(PhotoPictures f)
    {
        if( !photos.ContainsKey(f.ID) )  photos.Add(f.ID, f);
    }
    public class Context
    {
        public string text;
    }
}