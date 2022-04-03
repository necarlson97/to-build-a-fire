using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour {
    internal bool paused;

    public void Start() {
        Pause();
    }

    void Update() {
        if (Input.GetKeyDown("escape")) {
            if (paused) Unpause();
            else Pause();
        }
    }

    public void Pause() {
        paused = true;
        Time.timeScale = 0.01f;
        transform.Find("Canvas").gameObject.SetActive(true);
    }

    public void Unpause() {
        paused = false;
        Time.timeScale = 1;
        transform.Find("Canvas").gameObject.SetActive(false);
    }
}
