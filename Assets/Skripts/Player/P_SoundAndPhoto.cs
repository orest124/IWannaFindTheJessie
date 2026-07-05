
using Unity.VisualScripting;
using UnityEngine;

public class P_SoundAndPhoto : Sounds {
    private Movement mc;

    [Header("Photo Value")]
    public PhotoColection pc;

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
        if(mc.OptionsOpen || mc.noInteractive) return;
        
        if(UIEmpty()) {

            PhotoAudit(); 

            if(Input.GetKeyDown(KeyCode.Tab)  && pc.PhotoCount() > 0) PhotoTime( true );
            else if(Input.GetKeyDown(KeyCode.E)) ReadPlate();
            
            return;   
        }
        else {

            if(Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.E)) ClousedUI();
             

            if(Input.GetKey(KeyCode.Space) && Input.GetKeyDown(KeyCode.A)) pc.PrewPhoto();
            if(Input.GetKey(KeyCode.Space) && Input.GetKeyDown(KeyCode.D)) pc.NextPhoto();
            
        }
        
    }
    public bool UIEmpty() => State == UIState.Empty;
    public void PhotoTime(bool state)
    {
        State = state? UIState.Photo : UIState.Empty;   
        mc.SetStop(state);  
        if(state) pc.OpenPhoto(); else pc.ClousedPhoto();

    }
    UIState State;
    private void ClousedUI()
    {
        if(State == UIState.Photo)PhotoTime(false);
        else if(State == UIState.Plate) ClousedPlate();
        State = UIState.Empty;
        mc.SetStop(false);  


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



    [SerializeField] private Collider2D curentPlateColl;
    [SerializeField] private PlateView curentPlate;
    public void ReadPlate()
    {
        //Память, коли колл == нулл то відобразити курент палет но з іншим дизайном
        bool notHavePlate = false;
        if( curentPlate == null && curentPlateColl == null) return;
        else if(curentPlate == null && curentPlateColl != null) notHavePlate = true;
        else if(curentPlate != null && curentPlateColl != null && curentPlate.gameObject != curentPlateColl.gameObject) notHavePlate = true;
        
        if(notHavePlate) curentPlate = curentPlateColl.GetComponent<PlateView>();
       
        curentPlate.OpenPlate(true, curentPlateColl != null);
        State = UIState.Plate;
        mc.SetStop(true);  

    }
    public void ClousedPlate() => curentPlate.OpenPlate(false, curentPlateColl != null);
    
    private void PhotoAudit()
    {
        Collider2D coll = Physics2D.OverlapCircle(transform.position,0.3f,LayerMask.GetMask("Photo"));
        PhotoPictures ph;
        if(coll != null && coll.TryGetComponent<PhotoPictures>(out ph))
        { 
            ph.gameObject.SetActive(false);
            
            pc.AddPhoto(ph);
            PhotoTime(true);
            ph.ShowBeckSide = false;
        }
        coll = Physics2D.OverlapCircle(transform.position,0.3f,LayerMask.GetMask("Prise"));
        if(coll != null)
        { 
            coll.GetComponent<PrizeSercher>().prize.StartPresentation();
        }
        if(mc.moveDir == Vector3.zero) return;
        coll = Physics2D.OverlapCircle(transform.position + mc.moveDir,0.3f,LayerMask.GetMask("Info"));
        curentPlateColl = coll;
        
    }



    [Header("Layers")]
    public LayerMask IceArea;
    public LayerMask DipthArea;
    public LayerMask PlatformMask;
    public LayerMask BaricadeArea;
    public LayerMask EndArea;
    public LayerMask FishTrigger;
}
public enum UIState
{
    Empty,
    Photo,
    Meny,
    Plate,

}