using UnityEngine;

public class Bootstrap : MonoBehaviour 
{
    [Header("Objects")]
    [SerializeField] Dore CurentDore;
    [SerializeField] FollowCamera camera;
    [SerializeField] Movement pl;
    [SerializeField] RockModern[] rocks;
    [SerializeField] Dore[] dores;
    [SerializeField] Button[] butons;
    [SerializeField] Dore AbuseDoor;

    [Header("Servis")]
    [SerializeField] SaveSystem save;
    [SerializeField] bool inMeny;
    [SerializeField] MovementMemory memory;
    [SerializeField] GameOptions meny;

    [Header("Sound")]
    [SerializeField] SoundControler sound;
    [SerializeField] MusicThemeControler music;





    private void Awake() 
    {
        camera.Awake_Camera(pl);
        pl.Awake_movement(camera);
        save.inMeny = inMeny;
        save.Awake_saves(pl);
        memory.Awake_memory(pl, AbuseDoor);
        meny.Awake_many(pl, save, music, sound);
        
        foreach (Button b in butons)
        {
            b.Awake_buttons(sound);
        }
        foreach (Dore d in dores)
        {
            d.Awake_door(sound, memory);
            save.AddDoor(d);
        }

        pl.Awake_servis(memory, meny, CurentDore, AbuseDoor, music);
        
        foreach (RockModern r in rocks)
        {
            r.Awake_rock(sound, memory);
            save.AddRock(r);
            r.CheckButton(r.transform.position);
        }

        pl.CheckButton(pl.transform.position);
    }





    private void Start() {

        Destroy(gameObject);
    }
}