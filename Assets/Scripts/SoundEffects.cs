using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffects : MonoBehaviour {

    List<AudioClip> snowHits = new List<AudioClip>();
    List<AudioClip> woodHits = new List<AudioClip>();

    // TODO could do string varaible calling but eh

    public void Play(string sound) {
        // TODO
        Debug.Log("Supposed to play "+sound);
    }

    public void Update() {
        var v = Mathf.Clamp((float) Helper.FindAllScripts<Flame>().Count / 50, 0f, 1f);
        // Awknowledge user-set volume
        v *= transform.Find("Wind").GetComponent<AudioSource>().volume;
        transform.Find("Fire").GetComponent<AudioSource>().volume = v;
    }
}
