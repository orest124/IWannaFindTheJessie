using UnityEngine;

public class DipthControl : MonoBehaviour {
    void Awake()
    {
        int order = -Mathf.RoundToInt(transform.position.y * 10);
        GetComponent<SpriteRenderer>().sortingOrder = order;
    }
}