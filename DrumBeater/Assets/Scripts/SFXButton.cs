using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXButton : MonoBehaviour
{
    public Audio.AudioType SFXType;

    public void changeButtonSFX()
    {
        AudioManager.instance.playAudio(SFXType);
        GameManager.instance.buttonSFX = SFXType;
    }
}
