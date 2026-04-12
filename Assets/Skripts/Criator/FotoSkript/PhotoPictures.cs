
using UnityEngine;
using UnityEngine.Search;

public class PhotoPictures : MonoBehaviour
{
    [SerializeField] PhotoFrame Photo;
    [SerializeField] Sprite image;
    [SerializeField] bool blecText;
    [TextArea]
    public string massag;

    // [SerializeField] GameObject Mybutton;
    public bool inInventory;
    
    public void OpenFhoto()
    {
        Photo.SetImagine(image, blecText, massag, inInventory);
        Photo.gameObject.SetActive(true);
    }
    public void ClousedPhoto() => Photo.ClousedPhoto();
    
}
