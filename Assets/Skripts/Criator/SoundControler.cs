using UnityEngine;

public class SoundControler : Sounds
{
    // написати виключатель щоб звуки включались при початку гри а не в загрузці
    [SerializeField] AudioClip doreSound;
    [SerializeField] AudioClip[] buttonSound;
    [SerializeField] AudioClip rockBong;
    [SerializeField] AudioClip rockPush;
    [SerializeField] float buttonVolume = 0.5f;
    [SerializeField] float rockVolume = 0.5f;
    [SerializeField] float doorVolume = 0.01f;
    private bool online;
    public void SetOnline(bool state = true) => online = state;
    public void DoreSound() 
    {
        if(online) PlaySound(doreSound, doorVolume);
    }
    public void ButtonSound(float mod = 1, int tipe = 0) 
    {
        if(online) PlaySound(buttonSound[tipe], buttonVolume * mod);

    }
        public void RockSound(int v) 
    {
        if(online) PlaySound(v == 1? rockBong : rockPush, rockVolume);

    }
}
