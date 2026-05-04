using UnityEngine;

public class DipthControl : MonoBehaviour {
    void Awake()
    {
        int order = -((int)transform.position.y * 10);
        GetComponent<SpriteRenderer>().sortingOrder = order;
    }
}