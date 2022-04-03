using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoadFlare : MonoBehaviour {
    // TODO could probs just use life
    bool lit = false;
    float life = 1000;

    // How fast do we need to slam to light it?
    float lightVelocity = 15f;

    public GameObject flamePrefab;

    // Workaroud to stop them popping on spawn
    // TODO sloppy
    bool justStarted = true;
    void Start() { Invoke("Started", 1f); }
    void Started() { justStarted = false; }

    void Update() {
        // For velocity calulation
        lastLastPos = lastPos;
        lastPos = transform.Find("Striker").position;
        
        if (!lit || Dead()) return;
        if (lit) life --;
        GetComponentInChildren<Light>().intensity = (life/100) * 10;
    }

    void Light() {
        // Rodflare lights, emitting a bunch of flames
        lit = true;

        var pos = transform.Find("Emitter").position + Helper.RandomPoint(0.6f);
        var go = GameObject.Instantiate(flamePrefab, pos, Quaternion.identity);

        // Orpahns are short lived flames with no fuel,
        // it is the default - but likely it will find purchase
        go.transform.parent = GameObject.Find("Orphan Flames").transform;
    }

    void OnTriggerEnter(Collider other) {
        if (Dead()) return;
        if (Velocity() > lightVelocity) Light();
    }

    Vector3 lastPos;
    Vector3 lastLastPos;
    float Velocity() {
        // How fast is this flare moving
        // TODO rigidbody was giving weird answers,
        // so just doing lastFrame - thisOne
        var pos = transform.Find("Striker").position;
        if (lastLastPos == Vector3.zero) lastLastPos = pos;
        if (justStarted) return 0;
        return (lastLastPos - pos).magnitude / Time.deltaTime;
    }

    bool Dead() {
        return life < 0;
    }

    // void OnDrawGizmos()  {
    //     Handles.Label(transform.position, "Vel: "+Velocity());
    // }
}
