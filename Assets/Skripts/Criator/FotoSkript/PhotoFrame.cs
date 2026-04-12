
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotoFrame : MonoBehaviour
{
    [SerializeField] GameObject beck;
    [SerializeField] GameObject front;
    [SerializeField] TextMeshProUGUI text;
    public GameObject TabHelper;
    [SerializeField] Transform t;
    [SerializeField] Movement pl;
    public Image img;
    [SerializeField] float swapSpd;
    public Vector3 lokalSkale;
    void Start()
    {
        lokalSkale = text.gameObject.transform.localScale;
    }
    public void SetImagine(Sprite _image, bool blecText, string massag, bool inInventory = false) 
    {
        if (!inInventory) t.eulerAngles += new Vector3(0,180,0);
        img.sprite = _image;
        text.color = blecText? Color.black:Color.white;
        text.text = massag;
        Vector3 a = t.eulerAngles;
        AngleControl(a);
        
        
    }
    
    void Update()
    {
        Vector3 a = t.eulerAngles;
        AngleControl(a);
        if(Input.GetKey(KeyCode.A)) t.eulerAngles += new Vector3(0, Time.deltaTime * swapSpd);
        if(Input.GetKey(KeyCode.D)) t.eulerAngles += new Vector3(0, -Time.deltaTime * swapSpd);
        if(Input.GetKey(KeyCode.S)) t.eulerAngles = new Vector3(a.x, 0,a.z);
        
    }
    public void ClousedPhoto()
    {
        t.eulerAngles = new Vector3(t.eulerAngles.x, 0,t.eulerAngles.z);;
        front.SetActive(false);
        gameObject.SetActive(false);
    }
    private void AngleControl(Vector3 a)
    {
        if (a.y < -180) a.y = 180 + (a.y + 180);
        if (a.y > 180)  a.y = -180 + (a.y - 180); 

        bool isBeack;
        isBeack = a.y > -90 && a.y < 90;
        if(isBeack) 
        {
            beck.SetActive(false);
            front.SetActive(true);
            text.enabled = true;

        }
        else
        {
            // front.SetActive(false);
            beck.SetActive(true);
            text.enabled = false;
        }
    }

}
