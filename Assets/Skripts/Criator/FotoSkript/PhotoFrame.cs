
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotoFrame : MonoBehaviour
{
    [SerializeField] GameObject beck;
    [SerializeField] GameObject front;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Transform Rotation_Object;
    public Image Photo;
    [SerializeField] float swapSpd;
    public Vector3 lokalSkale;
    void Start()
    {
        lokalSkale = text.gameObject.transform.localScale;
    }
    public void SetImagine(Sprite _image, bool blecText, string massag, bool inInventory = false) 
    {
        if (!inInventory) Rotation_Object.eulerAngles += new Vector3(0,180,0);
        Photo.sprite = _image;
        text.color = blecText? Color.black:Color.white;
        text.text = massag;
        Vector3 a = Rotation_Object.eulerAngles;
        AngleControl(a);
        
        
    }
    
    void Update()
    {
        Vector3 a = Rotation_Object.eulerAngles;
        AngleControl(a);
        if(Input.GetKey(KeyCode.A)) Rotation_Object.eulerAngles += new Vector3(0, Time.deltaTime * swapSpd);
        if(Input.GetKey(KeyCode.D)) Rotation_Object.eulerAngles += new Vector3(0, -Time.deltaTime * swapSpd);
        if(Input.GetKey(KeyCode.S)) Rotation_Object.eulerAngles = new Vector3(a.x, 0,a.z);
        
    }
    public void ClousedPhoto()
    {
        Rotation_Object.eulerAngles = new Vector3(Rotation_Object.eulerAngles.x, 0,Rotation_Object.eulerAngles.z);;
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
