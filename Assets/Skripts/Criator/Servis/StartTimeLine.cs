using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class StartTimeLine : MonoBehaviour
{
    [SerializeField]PlayableDirector director;
    [SerializeField]Movement pl;
    [SerializeField]GameObject cat;
    [SerializeField] Vector3 PlayerMoveDirection;

    public void StartCatScene(bool state)
    {
        if(state == false) 
        {
            Destroy(gameObject);
            return;
        }
        cat.SetActive(true);
        director.Play();
        
        SetDir(PlayerMoveDirection);
        _start = true;

    }
    public void SetDir(Vector3 dir) => pl.SetMoveDir(dir);
    private bool _start;
    void Update()
    {
        if(_start && director.state != PlayState.Playing)
        {
            pl.SetStop(false);
            cat.SetActive(false);
            Destroy(gameObject);
        }   
    }
}
