using UnityEngine;

public class SoundControler : Sounds
{
    
    [SerializeField] AudioClip doreSound;
    [SerializeField] AudioClip buttonSound;
    [SerializeField] float buttonVolume = 0.5f;
    [SerializeField] float doorVolume = 0.01f;
    public void DoreSound() => PlaySound(doreSound, doorVolume);
    public void ButtonSound() => PlaySound(buttonSound, doorVolume);
    
}
