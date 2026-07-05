using System.Collections.Generic;
using UnityEngine;

public class JsonCharacter
{
    public int x;
    public int y;
    public bool inLavel;
    public List<int> photoIDs;
    public List<int> DoorIDs;
    public Vector3 GetVector() => new Vector3(x,y);
    public void SetVector(Vector3 pos)
    {
        x = Mathf.RoundToInt(pos.x);
        y = Mathf.RoundToInt(pos.y);
    }
    public JsonCharacter BuldNevMemory(List<PhotoPictures> pict, List<Dore> preDors)
    {
        
        photoIDs = new();
        DoorIDs = new();
        foreach (var f in pict) photoIDs.Add(f.ID);
        foreach (var d in preDors) DoorIDs.Add(d.ID);
        return this;
    }

}
