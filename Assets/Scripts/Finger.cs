using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Animations.Rigging;

using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

public class Finger : MonoBehaviour {

    internal bool closed = false;
    string status = "ok";
    internal float fingerSpeed = 10;

    // Add / remove from what fingers are holding this object
    private void OnTriggerEnter(Collider other) {

        var fingerName = transform.parent.parent.name;
        var g = other.transform.parent.GetComponent<Grabbable>();
        if (!g) return;
        
        var hand = FindObjectOfType<Grabber>();
        hand.holding[fingerName].Remove(g);
        hand.holding[fingerName].Add(g);
    }
    private void OnTriggerExit(Collider other) {
        var fingerName = transform.parent.parent.name;
        var g = other.transform.parent.GetComponent<Grabbable>();
        if (!g) return;
        
        var hand = FindObjectOfType<Grabber>();
        hand.holding[fingerName].Remove(g);
    }

    public void Update() {
        // Move finger to ik target
        MoveFinger();
    }

    void MoveFinger() {
        // Move this individual finger to its open / closed pos
        if (!Operable()) return;

        var t = transform.parent;
        var to = closed ? t.Find("closed") : t.Find("open");
        
        var speed = Slowed() ? fingerSpeed/5 : fingerSpeed;
        Helper.LerpTransform(t.Find("target"), to, speed);

        // If the finger is closed, it can grab stuff
        // note - we do not use enable/disable, because
        // it does not fire 'OnTriggerExit' - so we
        // use this location hack
        // TODO likely not best place for it
        if (closed) GetComponent<SphereCollider>().center = Vector3.zero;
        else GetComponent<SphereCollider>().center = new Vector3(0, 1000, 0);
    }

    public void Start() {
        InvokeRepeating("Next", Random.Range(0, 1f), 1f);
    }

    public void Next() {
        // Called every second, allow us to decide if we are cold/hot
        // TODO burning / freezing / etc
    }


    // If there are flames nearby, or too close
    bool Warm() { return Helper.FindNearby<Flame>(gameObject, 1f).Count > 0; }
    bool Hot() { return Helper.FindNearby<Flame>(gameObject, .4f).Count > 0; }
    // If we are holding onto something cold, and no fire around
    bool Frosty() {
        var hand = FindObjectOfType<Grabber>();
        return hand.held?.GetComponent<Fuel>()?.frost > 0;
    }
    bool Freezing() { return Frosty() & Helper.FindNearby<Flame>(gameObject, 3f).Count > 0; }

    bool Operable() {
        // can the finger be used at all
        return status != "frostbitten" && status != "charred";
    }
    bool Slowed() {
        // If finger is injuried by flame or cold, it is slower
        return status == "cold" || status == "burned";
    }

    void OnDrawGizmos()  {
        Handles.Label(transform.position, "status: "+status);
    }
}
