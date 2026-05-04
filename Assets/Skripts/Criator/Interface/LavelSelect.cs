
using System.Collections.Generic;
using UnityEngine;

public class LavelSelect : MonoBehaviour
{
    public List<Dore> l;
    Movement pl;
    private void Awake() {
        pl = FindAnyObjectByType<Movement>();
    }
    public void TeleportInLavel(int nomb, bool Restart)
    {
        pl.lavelMode(l[nomb]);
        pl.Idle();
        pl.CentralizedCamera();
        if(Restart) l[nomb].LavelMod(restLavel: true);
    }
    public void RestartInLavel(int nomb) => TeleportInLavel(nomb,true);
    public void TeleportInLavel(int nomb) => TeleportInLavel(nomb,false);
    
}

