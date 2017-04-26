using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using IsoUnity.Entities;
using System;
using IsoUnity;

public class TestingEntityScript : EventedEntityScript
{
    [GameEvent]
    public IEnumerator Test(Cell cell)
    {
        Debug.Log("WHOHOOOOO");
        yield return new WaitForSeconds(2);
        Debug.Log("Whojiii");
    }

    public override void Update()
    {

    }
}
