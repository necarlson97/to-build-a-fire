using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour {
    
    // What # of fingers are required to pickup this object
    public int required = 2;
    // What fingers are currently holding onto this object
    internal List<string> holding = new List<string>();
    // Where will we keep the object relative to the hand
    internal Vector3 offset = Vector3.zero;

    // If we have not enough fingers, grab only a bit
    int badGrab;
    int maxGrab = 100;

    void Update() {
        // If we have the right fingers holding on, we move with hand
        // TODO correct offset
        // TODO ahh fuck, grabbing multiple again...
        var hand = FindObjectOfType<Hand>().gameObject;
        if (Grabbed()) {
            transform.parent = hand.transform;
            GetComponent<Rigidbody>().isKinematic = true;
            CheckTips();  
        } else {
            transform.parent = transform.root;
            GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private void SetGrabbable(bool grabbed) {
        // Change layer of all colliders to make this object
        // not interacting with palm or anything
        foreach(Collider c in GetComponents<Collider> ()) {
            c.gameObject.layer = grabbed ? LayerMask.NameToLayer("Grabbed") : 0;
        }
    }

    private bool Grabbed() {
        // Tell we are grabbed at all, either good or bad
        if (GoodGrab()) return true;
        else if (BadGrab()) {
            badGrab = badGrab > 0 ? badGrab-1 : badGrab; // Decrement to 0
            return true;
        } else if (badGrab < maxGrab) {
            // If not touched at all, let our badGrab refull
            badGrab++;
        }
        return false;
    }

    private bool GoodGrab() {
        // For a solid grasp that holds indefinitly,
        // we must 
        return holding.Contains("thumb") && holding.Count >= 1 + required;
    }

    private bool BadGrab() {
        // For a light grab that only holds a bit,
        // we only need 1 finger
        return holding.Count > 0 && badGrab > 0;
    }

    private void OnCollisionEnter(Collision collision) {
        // TODO play noise
    }

    // Add / remove from what fingers are holding this object
    private void OnTriggerEnter(Collider other) {
        holding.Remove(other.transform.parent.parent.name);
        holding.Add(other.transform.parent.parent.name);
    }
    private void OnTriggerExit(Collider other) {
        holding.Remove(other.transform.parent.parent.name);
    }

    private void CheckTips() {
        // See if there are any tips we should share with the player
        // based on what we are holding
        if (GetComponent<Fuel>()){
            if (GetComponent<Fuel>().frost > 0) Helper.Tip("frost");
            else Helper.Tip("fuel");
        }
        if (GetComponent<RoadFlare>()) Helper.Tip("flare");
    }


}
