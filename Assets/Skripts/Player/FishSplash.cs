using UnityEngine;

public class FishSplash : MonoBehaviour 
{
    [SerializeField] Optimization pl;
    public bool imortal = true;
    [SerializeField] GameObject Bloom;

    [SerializeField] float curentSpd;
    [SerializeField] float lifeTime;

    public Transform target;
    Vector3 dir = new();
    void Start()
    {
        dir = target.position -transform.position ;
        transform.up = dir.normalized;
        
    }
    float t;
    void Update()
    {
        if(imortal) return;
        t += Time.fixedDeltaTime;
        if(t> lifeTime) Destroy(gameObject);
        Move();
    }
    public void Move()
    {
        float newspd = Time.deltaTime * curentSpd;
        transform.position += transform.up * newspd;
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject == pl.gameObject)
        {
            Bloom.SetActive(true);
            pl.FishAttak();
            Destroy(gameObject);
        }
    }
}