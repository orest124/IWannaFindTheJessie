using System.Collections;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class RockModernArt
{
    RockModern mc;
    private SpriteRenderer sp;

    [Header("Values")]
    public bool PlatformState;

    [Header("Sprite")]
    [SerializeField] Sprite RockArt;
    [SerializeField] Sprite ActiveArt;
    [SerializeField] Sprite PlatformArt;

    [Header("ActiveAnim")]
    [SerializeField] SpriteRenderer spActive;
    [SerializeField] float activeTime;
    [SerializeField] float act_t = 0;

    [Header("AlphaAnim")]
    [SerializeField] bool NOAlpfa;
    [SerializeField] float alphaDuration = 0.4f;

    
    [Header("Collider")]
    [SerializeField] Collider2D RockColl;
    [SerializeField] Collider2D PlatformColl;
    public LayerMask PlatformLayer;



    public void Start_art(RockModern _mc)
    {
        mc = _mc;
        sp = _mc.GetComponent<SpriteRenderer>();
        GetLayer(_mc.transform.position.y);
    }

    public void GetLayer(float y) => sp.sortingOrder = -Mathf.RoundToInt(y * 10);

    /// <summary>
    /// 0 => норма, 1 => Затонувший, 2 => Активний.
    /// </summary>
    /// <param name="state"></param>
    public void State(int state = 0)
    {
        if(state != 1 && PlatformState == true || state == 1 && PlatformState == false)
        {
            PlatformState = state == 1;
            CollFlip(state == 1);
            GetLayer(state == 1? 3000 : mc.transform.position.y);
        }
            
        if(state == 0) sp.sprite = RockArt;
        if(state == 2) sp.sprite = ActiveArt;
        if(state == 1) sp.sprite = PlatformArt;

        

    }
    private void CollFlip(bool s = false)
    {
        PlatformColl.enabled = s;
        RockColl.isTrigger = s;
    }



    

    



    WaitForFixedUpdate fix = new WaitForFixedUpdate();
    private bool noInteractive;
    public IEnumerator AlphaAnim(Vector3 nextPoint)
    {
        mc.SetNoInteractiv(true);
        noInteractive = true;

        float t = alphaDuration;
        while (t > 0)
        {
            yield return fix;
            t -= Time.fixedDeltaTime;
            float n = t / alphaDuration;
            
            SetAlpha(a:n);
        }
        mc.SetPos(nextPoint);

        t = 0;
        float alphaMod = NOAlpfa? 1 : 0.65f;
        while (t < alphaDuration * alphaMod)
        {
            yield return fix;
            t += Time.fixedDeltaTime;
            float n = t / alphaDuration;
            
            SetAlpha(a:n);
        }
        noInteractive = false;
        mc.SetNoInteractiv(false);

    }  

    public IEnumerator ActiveAnim(bool state, bool newt = true)
    {
        if(noInteractive == false)
        {
            spActive.sortingOrder = sp.sortingOrder + 1;
            SetAlpha(spActive, state? 0 : 1);
            spActive.enabled = true;
        }
            
        if(state == true)
        {
            if(newt) act_t = 0;
            if(noInteractive == false)
            {
                while (act_t < activeTime)
                {
                    yield return fix;
                    act_t += Time.fixedDeltaTime;
                    float n = act_t / activeTime;
                    
                    SetAlpha(spActive, n);
                }
            }
            sp.sprite = ActiveArt;

        }
        else if(state == false)
        {
            if(newt) act_t = activeTime;

            sp.sprite = RockArt;
            if(noInteractive == false)
            {
                while (act_t > 0)
                {
                    yield return fix;
                    act_t -= Time.fixedDeltaTime;
                    float n = act_t / activeTime;
                    
                    SetAlpha(spActive, n);
                }
            }   
        }
        spActive.enabled = false;
    }  
    public void SetAlpha(SpriteRenderer obj = null, float a = 0)
    {
        if(obj == null) obj = sp;
        obj.color = new Color(obj.color.r,obj.color.g,obj.color.b, a);
    }

}