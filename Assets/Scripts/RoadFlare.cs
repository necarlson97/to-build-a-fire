using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RoadFlare : MonoBehaviour {
    // TODO could probs just use life
    bool lit = false;
    float life = 100;

    // How fast do we need to slam to light it?
    float lightVelocity = 10f;

    public GameObject flamePrefab;

    void Update() {
        if (!lit || Dead()) return;
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
        Debug.Log("Striker entered");
        if (Dead()) return;
        if (Velocity() > lightVelocity) Light();
    }

    float Velocity() {
        // How fast is this flare moving
        return transform.Find("Striker").GetComponent<Rigidbody>().velocity.magnitude;
    }

    bool Dead() {
        return life < 0;
    }

    void OnDrawGizmos()  {
        Handles.Label(transform.position, "Vel: "+Velocity());
    }
}
