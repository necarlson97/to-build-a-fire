using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class Fuel : MonoBehaviour {
    // TOOD have fuel that runs out,
    // and turns parent object black
    public int fuel = 1000;
    private int startingFuel;

    public string matName = "Wood Mat"; // The resource mat name that we will tint 
    Material mat; // Material specific to this object

    // TODO lazy with the divides lol
    Color woodColor = new Color(112f/255, 32f/255, 0f/255);

    Color frostColor = new Color(67f/255, 105f/255, 105f/255);
    Color ashColor = new Color(28f/255, 28f/255, 28f/255);

    // Touching frosted fuel will hurt the player,
    // and 
    public int frost = 0;
    private int startingFrost;

    void Start() {
        InvokeRepeating("Next", Random.Range(0, 1f), 1f);
        // Create a new unite material to tint
        mat = Instantiate(Resources.Load(matName) as Material);;
        foreach (var mr in GetComponentsInChildren<MeshRenderer>()) {
            mr.material = mat;
        }        
        // Minimum of 1 to avoid /0 errors
        startingFuel = Mathf.Max(fuel, 1);
        startingFrost = Mathf.Max(frost, 100);
        SetColor();
    }
    void Next() {
        if (frost > 0 && NearbyFlames() > 0) {
            // Defrost particles
            GetComponentInChildren<ParticleSystem>().Play();    
            frost -= Mathf.Min(10, NearbyFlames());
        }
        SetColor();
    }
    void SetColor() {
        // Combine frost level and burnt level to get color
        var c = Color.Lerp(woodColor, frostColor, (float)frost/startingFrost);
        mat.color = Color.Lerp(ashColor, c, (float)fuel/startingFuel);
    }
    int NearbyFlames() {
        // Retuns the number of flames naerby
        return Helper.FindNearby<Flame>(gameObject, 5f).Count;
    }
    public bool HasFuel() {
        return fuel > 0 && frost <= 0;
    }

    private void OnCollisionEnter(Collision collision) {
        // If this is a wooden fuel source, play stick clank
        // when it hits something
        var t = transform.Find("Wood Fuel");
        if (t == null) return;
        // Dont ear-smash on startup
        if (Helper.JustStarted()) return;
        var audio = t.GetComponent<AudioSource>();
        var clips = t.GetComponent<WoodClips>().clips;
        audio.pitch = 1 + Random.Range(-.1f, .1f);
        audio.clip = clips.First();
        clips.Add(clips.First());
        clips.RemoveAt(0);
        audio.Play();
    }

    public static string FuelPrecent() {
        // What % of all fuel sources were used
        var used = 0;
        var total = 0;
        foreach (var f in Helper.FindAllScripts<Fuel>()) {
            used += f.startingFuel - f.fuel;
            total += f.startingFuel;
        }
        return Mathf.Round(((float)used / total) * 100) + "%";
    }
}
