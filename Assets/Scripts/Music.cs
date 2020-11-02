using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is just a singleton that plays the music and doesn't stop on restarting the game.

public class Music : MonoBehaviour
{
    static Music instance;

    void Start()
    {
        DontDestroyOnLoad(this);

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

    }
}
