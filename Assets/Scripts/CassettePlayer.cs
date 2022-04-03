using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CassettePlayer : MonoBehaviour {

    AudioClip startClip;
    public TimeSpan? startTime= null;
    
    private void OnTriggerEnter(Collider other) {
        var c = other.transform.parent.GetComponent<Cassette>();
        if (!c) return;

        c.GetComponent<Rigidbody>().isKinematic = true;
        c.GetComponent<Rigidbody>().detectCollisions = false;
        c.transform.parent = transform;
        c.transform.position = transform.Find("Slot").position;
        c.transform.rotation = transform.Find("Slot").rotation;

        transform.Find("Sound Effects").GetComponent<AudioSource>().Play();
        transform.Find("Tape Player").GetComponent<AudioSource>().clip = c.clip;
        Invoke("StartTape", 2f);
    }

    private void StartTape() {
        startTime = Helper.Now();
        transform.Find("Tape Player").GetComponent<AudioSource>().Play();
    }

    public void PauseTape() {
        transform.Find("Tape Player").GetComponent<AudioSource>().Pause();
    }
    public void ResumeTape() {
        transform.Find("Tape Player").GetComponent<AudioSource>().UnPause();
    }

    public string CurrentTime() {
        // How long as the cassette been playing?
        if (startTime == null) return "none";
        return Helper.RoundTime((Helper.Now() - startTime)) + "%";
    }
}
