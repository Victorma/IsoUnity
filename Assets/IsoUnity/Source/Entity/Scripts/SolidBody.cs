using UnityEngine;
using System.Collections;

public interface SolidBody {

    GameObject gameObject {get;}

    bool LetsPass(SolidBody e);

    bool CanGoThrough(SolidBody e);

}
