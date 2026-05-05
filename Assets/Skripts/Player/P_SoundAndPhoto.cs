
using UnityEngine;

public class P_SoundAndPhoto : Sounds {
    private Movement mc;

    [Header("Photo Value")]
    public PhotoColection pc;
    public bool notFromPhoto = true;

    [Header("Sound Value")]
    float curentTimerTime;
    public float walkLoopTime   = 0.25f;
    public float sprintLoopTime = 0.1f;
    public float stepVolume    = 0.16f;
    public AudioClip[] _walking;
    public AudioClip[] _SnowWalking;

    void Awake()
    {
        pc = new PhotoColection();
        mc = GetComponent<Movement>();
        curentTimerTime = walkLoopTime;

    }


    void Update()
    {
        if(mc.OptionsOpen) return;
        
        if(notFromPhoto) {

            PhotoAudit(); 

            if(Input.GetKeyDown(KeyCode.Tab)  && pc.PhotoCount() > 0) PhotoTime(notFromPhoto);
            
            return;   
        }
        else {

            if(Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape)) PhotoTime(notFromPhoto);
             

            if(Input.GetKey(KeyCode.Space) && Input.GetKeyDown(KeyCode.A)) pc.PrewPhoto();
            if(Input.GetKey(KeyCode.Space) && Input.GetKeyDown(KeyCode.D)) pc.NextPhoto();
            
        }
        
    }
    public void PhotoTime(bool state)
    {
        notFromPhoto = !state;      
        if(state) pc.OpenPhoto(); else pc.ClousedPhoto();

    }


    
    float t = 0;
    public void MoveSoundTimer()
    {
        t += Time.fixedDeltaTime;
        if(t > curentTimerTime)
        {
            t = 0;
            if(mc._inSnow || mc.inLavel) StepSnowNoise(stepVolume);
            else StepNoise(stepVolume);
        }
    }

    private void StepNoise(float volume)
    {
        int i = Random.Range(0, _walking.Length);
        PlaySound(_walking[i], volume,false, pitch: false);
    }
    private void StepSnowNoise(float volume)
    {
        int i = Random.Range(0, _SnowWalking.Length);
        PlaySound(_SnowWalking[i], volume,false, pitch: false);
    }
    public void ChengTimeLoop(bool _isRun = false) {

        curentTimerTime = _isRun? sprintLoopTime : walkLoopTime;
    }




    private void PhotoAudit()
    {
        Collider2D coll = Physics2D.OverlapCircle(transform.position,0.3f,LayerMask.GetMask("Photo"));
        PhotoPictures ph;
        if(coll != null && coll.TryGetComponent<PhotoPictures>(out ph))
        { 
            ph.gameObject.SetActive(false);
            pc.AddPhoto(ph);
            PhotoTime(true);
            ph.inInventory = true;
        }
    }



    [Header("Layers")]
    public LayerMask IceArea;
    public LayerMask DipthArea;
    public LayerMask PlatformMask;
    public LayerMask BaricadeArea;
    public LayerMask EndArea;
    public LayerMask FishTrigger;
}