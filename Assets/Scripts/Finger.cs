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

    public string matName = "Skin Mat";
    Material mat; // Material specific to this object

    public void Start() {
        InvokeRepeating("Next", Random.Range(0, 1f)+20f, 1f);
        // Create a new unite material to tint
        mat = Instantiate(Resources.Load(matName) as Material);;
        var t = transform.parent.parent;
        foreach (var mr in t.GetComponentsInChildren<SkinnedMeshRenderer>()) {
            mr.material = mat;
        }
        SetColor();
    }

    public void Next() {
        // Called every second, allow us to decide if we are cold/hot
        // TODO burning / freezing / etc
        if (!Operable()) return;
        if (status == "cold" && Warm()) status = "ok";
        if (status == "burned" && Frosty()) status = "ok";

        if (Random.Range(0, 1f) < 0.05f) {
            if (Burning() && status == "burned") status = "charred";
            if (Hot() && status == "ok") status = "burned";
            if (Warm() && status == "cold") {
                status = "ok";
                Helper.Tip("warm");
            }
            
            if (Freezing() && status == "cold") status = "frostbitten";
            if (Frosty() && status == "ok") status = "cold";

            if (status != "ok") Helper.Tip(status);
        }

        SetColor();
    }

    // TODO lazy with the divides lol
    Dictionary<string, Color> statusColors = new Dictionary<string, Color>(){
        {"ok", new Color(255f/255, 190f/255, 129f/255)},
        {"cold", new Color(222f/255, 255f/255, 241f/255)},
        {"frostbitten", new Color(32f/255, 28f/255, 28f/255)},
        {"burned", new Color(224f/255, 123f/255, 145f/255)},
        {"charred", new Color(20f/255, 20f/255, 20f/255)},
    };
    public void SetColor() {
        mat.color = statusColors[status];
    }

    public void Update() {
        // Move finger to ik target
        MoveFinger();
    }

    void MoveFinger() {
        // Move this individual finger to its open / closed pos
        if (!Operable()) {
            GetComponent<SphereCollider>().center = new Vector3(0, 1000, 0);
            return;
        }

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


    // If there are flames nearby, or too close
    bool Warm() { return NearFlames(4f); }
    bool Hot() { return NearFlames(2f); }
    bool Burning() { return NearFlames(1f); }
    // If we are holding onto something cold, and no fire around
    bool Frosty() {
        // Can get cold just from the wind
        if (Random.Range(0, 1f) < 0.1f) return true;
        var hand = FindObjectOfType<Grabber>();
        return hand.held?.GetComponent<Fuel>()?.frost > 0;
    }
    bool Freezing() { return Frosty() & !NearFlames(6f); }

    bool Operable() {
        // can the finger be used at all
        return status != "frostbitten" && status != "charred";
    }
    bool Slowed() {
        // If finger is injuried by flame or cold, it is slower
        return status == "cold" || status == "burned";
    }

    bool NearFlames(float range) {
        return Helper.FindNearby<Flame>(gameObject, range).Count > 0;
    }

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

    // void OnDrawGizmos()  {
    //     Handles.Label(transform.position, "flames: "+NearFlames(4f));
    // }
}
