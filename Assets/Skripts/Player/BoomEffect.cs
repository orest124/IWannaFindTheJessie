using System.Collections;
using UnityEngine;

public class BoomEffect : MonoBehaviour {
    
    [SerializeField] SpriteRenderer sp;
    bool white;
    public float whiteSpd;
    bool blekc;
    public float bleckSpd;
    float t = 0;
    void OnEnable()
    {
        sp.color = new Color(1,1,1,0);
        blekc = false;
        white = true;
        t = 0;
    }
    private void FixedUpdate() {
        Color c = sp.color;
        if(white)
        {
            t += Time.fixedDeltaTime * whiteSpd;
            Color newColor = Color.Lerp(sp.color, new Color(c.r, c.g, c.b, 1), t);
            sp.color = newColor;
            if(c.a > 0.9)
            {
                blekc = true;
                white = false;
                t = 0;
                sp.color = Color.black;
            } 
        }
        else if(blekc)
        {
            t += Time.fixedDeltaTime * bleckSpd;
            Color newColor = Color.Lerp(sp.color, new Color(c.r, c.g, c.b, 0), t);
            sp.color = newColor;
            if(c.a < 0.1)
            {
                gameObject.SetActive(false);
            } 
            
        }
    }
}