using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

public class MusicThemeControler : Sounds
{
    private bool Controle = false;
    private void Start() {
        HasLoop();
        in2Lavel();
    }
    public void inAbuse()
    {
                PlayTheme(3);

    }
    public void in1Lavel()
    {

        PlayTheme(0);

        
    }
    public void in2Lavel()
    {
                PlayTheme(1);
                

        
    }
    public void in3Lavel()
    {

                PlayTheme(2);

        
    }
    
    
    public void inBorderleands()
    {

                PlayTheme(4);

        
    }
    public void inNotMenyLavels()
    {

                PlayTheme(5);

        
    }

}
