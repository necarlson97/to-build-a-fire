using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Flame : MonoBehaviour {
    
    // How long will this flame last, spreading new fires
    internal float lifeMax = 100;
    public float life;

    // An individual flame that gets spawned by this fire source
    public GameObject flamePrefab;

    internal static int maxFlames = 100;
    internal float maxFlameSpread = 1f; // How far flames will want to spread to
    
    void Start() {
        if (life == 0) life = lifeMax;
        FindFuel();
        InvokeRepeating("Next", Random.Range(0, 1f), 1f);
    }

    void FindFuel() {
        // Once we spawn in, look for a fuel to join to
        var f = ClosestFuel(maxFlameSpread);

        // TODO REMOVE
        if (f == null) {
            Debug.DrawLine(transform.position,
            ClosestFuel().transform.position,
            Color.yellow, 5f);
        } else {
            Debug.DrawLine(transform.position,
            f.transform.position,
            Color.green, 5f);
        }

        if (f == null) return;

        // Attach flame to closest fuel
        transform.position = Helper.ClosestPoint(f.gameObject, transform.position);
        transform.parent = f.transform;
    }

    void Update() {
        // Find flame particles and scale them to vitality
        var f = transform.Find("Fire");
        if (f == null ) return;
        var vitalityVector = new Vector3(1, 1, 1) * Vitality();
        f.localScale = Vector3.Lerp(f.localScale, vitalityVector, Time.deltaTime * 2);

        transform.rotation = Quaternion.identity;

        // Remove, but give a bit of time for particles to flame out
        if (life < -2) DieOut();
    }

    void Next() {
        // Called every second, allow us to decide how often to make a flame

        // All fires are constantly burnuing out,
        // and must reproduce to survive
        life --;

        // Flames with no fuel are very short lived
        if (MyFuel() == null) {
            life -= 50;
            return;
        } 

        // Subract from fuels remaining life
        MyFuel().fuel--;
        // Sometimes, depending on this flames health, it will spread
        if (Random.Range(0, 1f) < 0.3f) SpawnFlame();
    }

    void DieOut() {
        // Remove self after a time
        Destroy(gameObject);
    }

    int NearbyFlames() {
        // Retuns the number of flames naerby
        return Helper.FindNearby<Flame>(gameObject, 1f).Count;
    }

    void SpawnFlame() {
        // Spawn another flame in a random direction
        // if it cannot survite there, it will die out quickly
        if (Helper.FindAllScripts<Flame>().Count > maxFlames) return;

        // If this fuel is already lit
        if (MyFuel().transform.childCount > maxFlames / 2) return;

        var pos = transform.position + Helper.RandomPoint(maxFlameSpread);
        var go = GameObject.Instantiate(flamePrefab, pos, Quaternion.identity);
        go.GetComponent<Flame>().life = lifeMax;

        // Orpahns are short lived flames with no fuel,
        // it is the default - but likely it will find purchase
        go.transform.parent = GameObject.Find("Orphan Flames").transform;
    
        // TODO do we need this?
        life *= 0.8f;
    }

    float Vitality() {
        // How much life left this flame has
        return Mathf.Clamp(life / lifeMax, 0, 1);
    }

    Fuel ClosestFuel(float max=0) {
        // Must be quite close to fuel to survive
        if (max==0) max = Mathf.Infinity;
        Fuel closestFuel = null;
        float distance = Mathf.Infinity;
        var pos = transform.position; // Shorthand
        foreach (var f in Helper.FindNearby<Fuel>(gameObject, max, false, f => f.HasFuel())) {
            var p = Helper.ClosestPoint(f, pos);
            if (Vector3.Distance(p, transform.position) < distance) {
                closestFuel = f.GetComponent<Fuel>();
                distance = Vector3.Distance(p, transform.position);
            }
        }
        if (distance < max) return closestFuel;
        else return null;
    }

    Vector3 ClosestFuelPoint(float max=0) {
        // Actual vector which would attach us to the closest fuel
        var f = ClosestFuel(max);
        if (f == null) return transform.position;
        return Helper.ClosestPoint(f.gameObject, transform.position);
    }

    Fuel MyFuel() {
        // The fuel this flame is feeding from
        return transform.parent.GetComponent<Fuel>();
    }
}
