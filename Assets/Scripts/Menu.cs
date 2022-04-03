using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Linq;

public class Menu : MonoBehaviour {
    internal bool paused;
    internal TimeSpan? startTime = null;

    List<Camera> cams = new List<Camera>();

    float showVignette = 0;

    public void Start() {
        Pause();
        cams = FindObjectsOfType<Camera>().ToList();
    }

    void Update() {
        if (Input.GetKeyDown("escape")) {
            if (paused) Unpause();
            else Pause();
        }

        if (Input.GetKey("0")) ToggleCamera();

        // fade out the vignette
        if (showVignette > 0) {
            showVignette--;
            var vign = GameObject.Find("Vignette").GetComponent<RawImage>();
            var c = vign.color;
            c.a = showVignette / 100;
            vign.color = c;
        }
    }

    public void ShowVignette(bool cold) {
        // Show the vigette for a second as either hot or cold
        var vign = GameObject.Find("Vignette").GetComponent<RawImage>();
        if (cold) vign.color = Color.white;
        else vign.color = Color.red;
        showVignette = 100f;
    }

    public void ToggleCamera() {
        // Switch between views, for devs only really
        cams.First().enabled = false;
        cams.Add(cams.First());
        cams.RemoveAt(0);
        cams.First().enabled = true;
    }

    public void Pause() {
        paused = true;
        Time.timeScale = 0.01f;
        transform.Find("Canvas").gameObject.SetActive(true);
        FindObjectOfType<CassettePlayer>().PauseTape();
    }

    public void Unpause() {
        paused = false;
        Time.timeScale = 1;
        transform.Find("Canvas").gameObject.SetActive(false);
        FindObjectOfType<CassettePlayer>().ResumeTape();
        if (startTime == null) startTime = Helper.Now();
    }

    public void SetSounds() {
        var sfxVolume = transform.Find("Canvas/SFX Slider").GetComponent<Slider>().value;
        var cassetteVolume = transform.Find("Canvas/Cassette Slider").GetComponent<Slider>().value;

        var sfx = GameObject.Find("SFX").GetComponentsInChildren<AudioSource>();
        foreach (var c in sfx)  {
            c.volume = sfxVolume;
        }

        var cassette = GameObject.Find("Cassette Player").GetComponentsInChildren<AudioSource>();
        foreach (var c in cassette)  {
            c.volume = cassetteVolume;
        }
    }

    public void HelpToggle() {
        var help = transform.Find("Canvas/Help").gameObject;
        help.SetActive(!help.activeInHierarchy);
    }

    public void Die() {
        var survived = Helper.RoundTime((Helper.Now() - startTime));
        var fuel = Fuel.FuelPrecent();
        var cassette = FindObjectOfType<CassettePlayer>().CurrentTime();
        
        var t = String.Format("You died. That's ok, we all will.\n"
            +"You survived {0}.\n"
            +"You burned {1} of the fuel.\n"
            +"You listened to {2} of the cassette.",
            survived, fuel, cassette);
        transform.Find("Canvas/Description").GetComponent<Text>().text = t;

        transform.Find("Canvas/Help").gameObject.SetActive(false);
        transform.Find("Canvas/Play Btn").gameObject.SetActive(false);
        transform.Find("Canvas/Replay Btn").gameObject.SetActive(true);
        Pause();
    }

    public void Replay() {
        // Player has died - reload everything
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
