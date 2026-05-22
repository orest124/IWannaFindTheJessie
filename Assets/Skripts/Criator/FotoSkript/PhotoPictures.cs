
using UnityEngine;
using UnityEngine.Search;

public class PhotoPictures : MonoBehaviour
{
    [SerializeField] PhotoFrame Photo;
    [SerializeField] Sprite image;
    [SerializeField] bool blecText;
    private MovementMemory memory;
    [TextArea]
    public string massag;

    // [SerializeField] GameObject Mybutton;
    public bool inInventory;
    void Awake()
    {
        GetName();
        memory = FindAnyObjectByType<MovementMemory>();
        memory.IsSaveReady(this);
    }

    public void OpenFhoto()
    {
        Photo.SetImagine(image, blecText, massag, inInventory);
        Photo.gameObject.SetActive(true);
    }
    public void ClousedPhoto() => Photo.ClousedPhoto();
        public int ID;

    private void GetName()
    {
        Vector3 pos = transform.position;
        int x = Mathf.RoundToInt(Mathf.Abs(pos.x));
        int y = Mathf.RoundToInt(Mathf.Abs(pos.y));
        int z = 0;
        if(pos.x < 0 && pos.y >=0) z = 1;
        else if(pos.x >= 0 && pos.y < 0) z = 2;
        else if(pos.x < 0 && pos.y < 0) z = 3;
        ID = int.Parse($"{x}{y}{z}");

        gameObject.name = $"Pict {ID}";;
    }
    
}
