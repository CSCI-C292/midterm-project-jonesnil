using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenButton : MonoBehaviour
{
    [SerializeField] Sprite cropOut;
    [SerializeField] Sprite cropIn;
    Image buttonImage;

    public void Start()
    {
        buttonImage = this.GetComponent<Image>();
    }
    public void ChangeScreenSize() 
    {
        Screen.fullScreen = !Screen.fullScreen;

        if (Screen.fullScreen)
        {
            buttonImage.sprite = cropOut;
        }
        else 
        {
            buttonImage.sprite = cropIn;
        }
    }


}
