using IsoUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using IsoUnity.Entities;

public class AudioManager : EventedEntityScript
{
    [SerializeField]
    GameObject audioPrefab;

    /*[SerializeField]
    List<AudioClip> audios = new List<AudioClip>();

    public override void ReceiveEvent(IGameEvent ev)
    {
        if(ev.Name == "play sound")
        {
            string audioname = (string) ev.getParameter("audio");

            var audioobject = Instantiate(audioPrefab).GetComponent<AudioSource>();
            audioobject.clip = audios.Find(a => a.name == audioname);
            audioobject.Play();

        }
    }

    public override void Tick()
    {
    }*/

    [GameEvent]
    public void PlaySound(AudioClip clip)
    {
        var audioobject = Instantiate(audioPrefab).GetComponent<AudioSource>();
        audioobject.clip = clip;
        audioobject.Play();
    }

    public override void Update()
    {
    }
}
