using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Fuel : MonoBehaviour {
    // TOOD have fuel that runs out,
    // and turns parent object black
    public int fuel = 1000;
    private int startingFuel;
    Material mat; // Material specific to this object

    // TODO lazy with the divides lol
    Color woodColor = new Color(112f/255, 32f/255, 0f/255);
    Color frostColor = new Color(105f/255, 255f/255, 250f/255);
    Color ashColor = new Color(28f/255, 28f/255, 28f/255);

    // Touching frosted fuel will hurt the player,
    // and 
    public int frost = 0;
    private int startingFrost;

    void Start() {
        InvokeRepeating("Next", Random.Range(0, 1f), 1f);
        // Create a new unite material to tint
        mat = Instantiate(Resources.Load("Wood Mat") as Material);;
        foreach (var mr in GetComponentsInChildren<MeshRenderer>()) {
            mr.material = mat;
        }        
        // Minimum of 1 to avoid /0 errors
        startingFuel = Mathf.Max(fuel, 1);
        startingFrost = Mathf.Max(frost, 100);
    }
    void Next() {
        if (frost > 0) frost -= NearbyFlames();
        
        // Combine frost level and burnt level to get color
        var c = Color.Lerp(woodColor, frostColor, (float)frost/startingFrost);
        mat.color = Color.Lerp(ashColor, c, (float)fuel/startingFuel);
    }
    int NearbyFlames() {
        // Retuns the number of flames naerby
        return Helper.FindNearby<Flame>(gameObject, 2.5f).Count;
    }
    public bool HasFuel() {
        return fuel > 0 && frost <= 0;
    }

    void OnDrawGizmos()  {
        Handles.Label(transform.position, "Flames: "+NearbyFlames());
    }
}
