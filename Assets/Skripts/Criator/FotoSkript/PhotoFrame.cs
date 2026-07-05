using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhotoFrame : MonoBehaviour
{
    [SerializeField] GameObject beck;
    [SerializeField] GameObject front;
    [SerializeField] TextMeshProUGUI Text;
    [SerializeField] TextMeshProUGUI TextBeck;
    private TextMeshProUGUI text;
    [SerializeField] Transform Rotation_Object;
    public Image Photo;
    [SerializeField] float swapSpd;
    public Vector3 lokalSkale;
    void Start()
    {
        lokalSkale = Text.gameObject.transform.localScale;
    }
    public void SetImagine(Sprite _image, string massag, string hideMassag, bool ShowBeck = false) 
    {
        if (ShowBeck) Rotation_Object.eulerAngles += new Vector3(0,180,0);
        Photo.sprite = _image;

        Text.text = massag;
        TextBeck.text = hideMassag;
        Vector3 a = Rotation_Object.eulerAngles;
        

        AngleControl(a);
        
        
    }
    
    void Update()
    {
        Vector3 a = Rotation_Object.eulerAngles;
        AngleControl(a);
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) Rotation_Object.eulerAngles += new Vector3(0, Time.deltaTime * swapSpd);
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) Rotation_Object.eulerAngles += new Vector3(0, -Time.deltaTime * swapSpd);
        if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) Rotation_Object.eulerAngles = new Vector3(a.x, 0,a.z);
        
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

        bool isBeack = a.y > -90 && a.y < 90;

        front.SetActive(isBeack);
        Text.enabled = isBeack;
        beck.SetActive(!isBeack);
        TextBeck.enabled = !isBeack;
    }

}
