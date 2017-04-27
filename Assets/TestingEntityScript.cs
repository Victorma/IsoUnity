using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IsoUnity.Entities;
using System;
using IsoUnity;

public class TestingEntityScript : EventedEntityScript
{

    IGameEvent ge;

    [GameEvent(false)]
    public void Test(Cell cell, string mensaje)
    {
        Debug.Log("WHOHOOOOO");
        Debug.Log(mensaje);

        ge = Current;
    }

    public override void Update()
    {
        if(ge != null)
        {
            Game.main.eventFinished(ge);
            ge = null;
        }
    }
}
