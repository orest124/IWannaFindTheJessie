using UnityEngine;

public class Trace : MonoBehaviour {
    [SerializeField]ParticleSystem _particle;
    Movement pl;
//     void Start()
//     {
//         pl = GetComponent<Movement>();
//         _particle.Stop();
//     }
//     void Update()
//     {
//         if(pl.moveDir != Vector3.zero) _particle.Play();
//         else _particle.Stop();
//     }
}