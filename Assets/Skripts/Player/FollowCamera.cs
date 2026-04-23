using UnityEngine;

public class FollowCamera{
    float dist;
    public Transform _c;


    public void MoveTo(Vector3 point)
    {
        Vector3 newpoint = Vector3.Lerp(_c.position, new Vector3(point.x,point.y, _c.position.z), 2);
    }
}