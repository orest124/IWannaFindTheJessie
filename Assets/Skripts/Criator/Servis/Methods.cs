using System;
using Unity.VisualScripting;
using UnityEngine;

public static class Methods{
    public static bool isPlace(Vector3 my, Vector2 place) => place.x == my.x && place.y == my.y;
    public static Vector3 IdealPos(Vector3 my) => new Vector2(Mathf.RoundToInt(my.x), Mathf.RoundToInt(my.y));
    public static Vector3 GetNextStep(Vector3 my, Vector3 target,float _spd)
    {
        float newspd = Time.deltaTime * _spd;
        return Vector2.MoveTowards(my, target, newspd);
    }
/// <summary>
///  0 => normal, 1 => Dipth, 2 => Ice.
/// </summary>
    public static int CheckFallen(Vector3 _point)
    {
        if(Physics2D.OverlapPoint(_point, LayerMask.GetMask("Dipth")) 
            && !Physics2D.OverlapPoint(_point, LayerMask.GetMask("Platform")) 
            && !Physics2D.OverlapPoint(_point, LayerMask.GetMask("Ice"))) return 1;
        else if(Physics2D.OverlapPoint(_point, LayerMask.GetMask("Ice"))) return 2;
        else return 0; 
    }


    public static Collider2D CheckCollider(Vector3 point)
    {
        Collider2D coll = Physics2D.OverlapPoint(point, LayerMask.GetMask("Wall"));
        if(coll != null && !coll.isTrigger) return coll;
        
        coll = Physics2D.OverlapPoint(point, LayerMask.GetMask("Rook"));
        if(coll != null && !coll.isTrigger) return coll;

        return null;
        
    } 
    /// <summary>
    /// 0 => пусто,  1 => в русі,  2 => перегорожа,  3 => камінь.
    /// </summary>
    public static int CheckRockInMove(Vector3 _target, RockModern rv = null, Action<RockModern> act = null)
    {
        
        var coll = CheckCollider(_target);
        if(coll == null) return 0;
        else
        {
            RockModern r;
            if (rv != null && coll.gameObject == rv.gameObject) {     if(rv.isMove) return 1; else return 3;     }
            else if(coll.TryGetComponent<RockModern>(out r))
            {
                if (r.isMove) 
                {
                    act?.Invoke(r);
                    return 1;
                }
                else return 3;
            }
            return 2;
        }
    }
    /// <summary>
    /// 0 => пусто,  1 => в русі,  2 => стіна,  3 => камінь.
    /// </summary>
    public static int CheckEmpty(Vector3 myPos, Vector3 dir, RockModern r = null)
    {
        int _placeTipe = 1;
        Vector3 newpos = myPos;
        int n = 0;
        while (_placeTipe != 2)
        {
            n++; if(n > 50) {Debug.Log("StackOwerFlow"); break;}
            newpos += dir;
            _placeTipe = CheckRockInMove(newpos, r);
            if(_placeTipe == 0) return 0;
            else if(_placeTipe == 3) return 3;
        }
        return 2;
    }
    public static Button CheckButton(Vector3 point, Button curentButton, RockModern r = null, bool isPlayer = false)
    {
        Collider2D coll = Physics2D.OverlapPoint(point, LayerMask.GetMask("Button"));
        Button b = curentButton;
        if( !CompareButton(coll, b) )
        {
            b?.ChengPresed(false, null);
            b = coll.GetComponent<Button>();
            if(isPlayer) b?.ChengPresed(true, true);
            else b?.ChengPresed(true, r);
        }
        else if(coll == null && b != null)
        {
            b.ChengPresed(false, null); 
            b = null;
        }
        return b;
    }
    private static bool CompareButton(Collider2D b, Button cb)
    {
        if(b == null) return true;
        else if(cb == null) return false;
        else if(b.name == cb.name) return true;
        else return false;
    }

    public static int GetName(Vector3 pos)
    {
        int x = Mathf.RoundToInt(Mathf.Abs(pos.x));
        int y = Mathf.RoundToInt(Mathf.Abs(pos.y));
        int z = 0;
        if(pos.x < 0 && pos.y >=0) z = 1;
        else if(pos.x >= 0 && pos.y < 0) z = 2;
        else if(pos.x < 0 && pos.y < 0) z = 3;
        int ID = int.Parse($"{x}{y}{z}");
        return ID;

    }

}