using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

public class Grabber : MonoBehaviour {
    // What fingers are currently holding onto something
    internal Dictionary<string, List<Grabbable>> holding = new Dictionary<string, List<Grabbable>>(){
        {"thumb", new List<Grabbable>()},
        {"index", new List<Grabbable>()},
        {"middle", new List<Grabbable>()},
        {"ring", new List<Grabbable>()},
        {"pinkie", new List<Grabbable>()},
    };
    // What 1 object are we actually grasping
    internal GameObject held = null;

    // If we have not enough fingers, grab only a bit
    int badGrab;
    int maxGrab = 100;

    void Update() {
        CheckGrab();
    }

    void OnHold(GameObject go) {
        // When we hold, it moves with us,
        // and has no gravity physics
        go.transform.parent = transform;
        go.GetComponent<Rigidbody>().isKinematic = true;
        held = go;
        CheckTips();
    }

    void OnDrop(GameObject go) {
        // When we drop, return it to normal physics
        go.transform.parent = transform.root;
        go.GetComponent<Rigidbody>().isKinematic = false;
        held = null;
    }

    void CheckGrab() {
        // See what are fingers are near
        var grabbable = held?.GetComponent<Grabbable>();
        // Flatten to get list of everything in fingers
        var inFingers = InFingers();
        
        // If we are still holding on, good
        if (held != null && inFingers.Contains(grabbable) && Gripping()) return;
        // If we are not holding on anymore, let go
        // If there is something in our fingers, pick it up
        if (held != null) OnDrop(held);
        else if (inFingers.Count > 0) OnHold(inFingers.First().gameObject);
    }

    List<Grabbable> InFingers() {
        // Return a flat list of everying in any fingers
        return holding.Values.ToList().SelectMany(x => x).ToList();
    }

    private bool Gripping() {
        // Grip is adequite if:
        // we have required # of fingers on it
        // OR we have only a few fingers, so we can hold for a short bit
        if (!held || GoodGrab()) badGrab = Mathf.Min(badGrab+1, maxGrab);

        if (!held) return false;
        else if (GoodGrab()) return true;
        else if (BadGrab()) {
            badGrab = Mathf.Max(badGrab-1, 0); // Decrement to 0
            if (badGrab < maxGrab/2) GripTip();
            return true;
        }
        return false;
    }
    private void GripTip() {
        // If we are not holding on well,
        // let them know 
        if (held.GetComponent<Grabbable>().required > 2) Helper.Tip("heavy");
        else Helper.Tip("grip");
    }

    private bool GoodGrab() {
        // For a solid grasp that holds indefinitly,
        // we must have required # of fingers on
        var g = held.GetComponent<Grabbable>();
        // (+1 because thumb doesn't count)
        return holding["thumb"].Contains(g) && HeldFingers() >= g.required+1;
    }

    private bool BadGrab() {
        // For a light grab that only holds a bit,
        // we only need 1 finger
        return HeldFingers() > 0 && badGrab > 0;
    }

    private int HeldFingers() {
        // Total number of fingers touching held object
        if (!held) return 0;
        var touching = 0;
        foreach (var item in holding) {
            if (item.Value.Contains(held.GetComponent<Grabbable>())) touching++;
        }
        return touching;
    }

    private void CheckTips() {
        // See if there are any tips we should share with the player
        // based on what we are holding
        if (held == null) return;
        if (held.GetComponent<RoadFlare>()) Helper.Tip("flare");
        if (held.GetComponent<Match>()) Helper.Tip("match");
        if (held.GetComponent<Fuel>()){
            if (held.GetComponent<Fuel>().frost > 0) Helper.Tip("frost");
            else Helper.Tip("fuel");
        }
    }

    // void OnDrawGizmos()  {
    //     var s = ""; 
    //     if (held != null) s += "Held: "+held.name+" "+HeldFingers()+"\n";
    //     if (held != null) s += "Grip: "+badGrab+"/"+maxGrab+"\n";
    //     // foreach (var item in holding) {
    //     //     s += item.Key+": "+item.Value.Count+"\n";
    //     // }
    //     Handles.Label(transform.position, s);
    // }
}
