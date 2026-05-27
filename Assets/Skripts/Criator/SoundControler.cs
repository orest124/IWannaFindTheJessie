using UnityEngine;

public class SoundControler : Sounds
{
    // написати виключатель щоб звуки включались при початку гри а не в загрузці
    [SerializeField] AudioClip doreSound;
    [SerializeField] AudioClip buttonSound;
    [SerializeField] float buttonVolume = 0.5f;
    [SerializeField] float doorVolume = 0.01f;
    private bool online;
    public void SetOnline(bool state = true) => online = state;
    public void DoreSound() 
    {
        if(online) PlaySound(doreSound, doorVolume);
    }
    public void ButtonSound() 
    {
        if(online) PlaySound(buttonSound, buttonVolume);

    }
}
