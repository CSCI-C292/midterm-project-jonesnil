using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
