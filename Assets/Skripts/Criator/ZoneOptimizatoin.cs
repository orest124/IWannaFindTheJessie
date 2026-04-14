using UnityEngine;

public class ZoneOptimizatoin : MonoBehaviour
{
    [SerializeField] ZoneOptimizatoin previosliZone;
    [SerializeField] ZoneOptimizatoin nextZone;
    private GameObject pZone;
    private GameObject nZone;
    // "fjdsljlkfljfdf"
    public bool zero;
    private void Awake() {
        if(zero) return;
        pZone = previosliZone.gameObject;
        nZone = nextZone.gameObject;
    }
    private void ClousedNeighbor(GameObject claimant)
    {
        if(zero) return;
        if(pZone != claimant) pZone.SetActive(false);
        else if(nZone != claimant) nZone.SetActive(false);
    }
    public void Clament()
    {
        if(zero) return;
        previosliZone.ClousedNeighbor(gameObject);
        nextZone.ClousedNeighbor(gameObject);
    }
    public void OpenNeighbor()
    {
        if(zero) return;
        pZone.SetActive(true);
        nZone.SetActive(true);
        Clament();
    }

}
