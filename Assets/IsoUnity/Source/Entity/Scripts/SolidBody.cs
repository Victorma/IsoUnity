using UnityEngine;
using System.Collections;
using IsoUnity;

namespace IsoUnity.Entities
{
    public interface SolidBody
    {

        GameObject gameObject { get; }

        bool LetsPass(SolidBody e);

        bool CanGoThrough(SolidBody e);

    }
}
