using System;
using System.Collections.Generic;
public class GameState
{
    public int LastId {get;set;}
    public List<Entyty> Entytys {get;set;}
    public int GetNewId()
    {
        return ++LastId;
    }
}


public class Entyty
{
    public int ID {get;set;}
    public EntytyType Type {get; set;}
}
public enum EntytyType
{
    Door,
    Rock,
    Character,
    Photo,
    Config
}

public class DoorEntyty : Entyty
{
    public bool IsOpen {get;set;}
    public bool IsLoked {get;set;}
}
public class PickupableEntyty : Entyty
{
    public string ItemId {get;set;}
    public int Amount {get;set;}
}
public class GreadEntyty : Entyty
{
    public PickupableEntyty[,] gread;
}

