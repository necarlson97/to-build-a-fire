using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoadFlare : FireStarter {
    internal override void Visuals() {
        GetComponentInChildren<Light>().intensity = (life/100) * 10;
    }
}
