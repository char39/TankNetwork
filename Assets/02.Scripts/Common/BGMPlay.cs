using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMPlay : MonoBehaviour
{
    private Transform tr;
    public AudioClip bgm;

    void Start()
    {
        tr = GetComponent<Transform>();
        SoundManager.instance.SetBGM(tr.position, bgm, true);
    }
}
