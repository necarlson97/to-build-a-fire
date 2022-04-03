using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public abstract class FireStarter : MonoBehaviour {
    // TODO could probs just use life
    bool lit = false;
    internal float life = 1000;

    // How fast do we need to slam to light it?
    float lightVelocity = 5f;

    public GameObject flamePrefab;

    void Update() {
        // For velocity calulation
        lastLastPos = lastPos;
        lastPos = transform.Find("Striker").position;
        
        if (!lit || Dead()) return;
        if (lit) life --;
    }

    internal abstract void Visuals();

    void Light() {
        // Rodflare lights, emitting a bunch of flames
        lit = true;

        var pos = transform.Find("Emitter").position + Helper.RandomPoint(0.6f);
        var go = GameObject.Instantiate(flamePrefab, pos, Quaternion.identity);
        go.transform.parent = transform;
        GetComponent<AudioSource>().Play();
    }

    void OnTriggerEnter(Collider other) {
        if (Dead() || lit) return;
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
        // Workaroud to stop them popping on spawn
        if (Helper.JustStarted()) return 0;
        return (lastLastPos - pos).magnitude / Time.deltaTime;
    }

    bool Dead() {
        return life < 0;
    }

    void OnDrawGizmos()  {
        Handles.Label(transform.position, "Vel: "+Velocity());
    }
}
