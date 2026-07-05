using TMPro;
using UnityEngine;

public class PlateView : MonoBehaviour
{
    [TextArea]
    [SerializeField] string masseng;
    [SerializeField] TextMeshProUGUI LostSeetextArea;
    [SerializeField] TextMeshProUGUI ClousedtextArea;
    [SerializeField] GameObject LostSeePlate;
    [SerializeField] GameObject ClousedPlate;

    public void OpenPlate(bool state, bool mod = true)
    {
        if(mod)
        {
            ClousedtextArea.text = masseng;
            ClousedPlate.SetActive(state);   
            return; 
        }
        LostSeetextArea.text = masseng;
        LostSeePlate.SetActive(state);
    }
}
