using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Tips : MonoBehaviour {
    
    Dictionary<string, List<string>> d = new Dictionary<string, List<string>>(){
        {"grip", new List<string>() {
            "Oh, that's heavy.\nI'll have to grip it with more fingers...",
            "Arg, I have to get a better grip.",
        }}, {"frost", new List<string>() {
            "Hm, it's frozen, I'll need to warm it first.",
            "Yeah, this is wet, I'll need to dry it.",
            "Oh man that's cold. Going to loose a finger.",
            "Ouch, this hurts just to hold",
        }}, {"burn", new List<string>() {
            "Ow! I'm going to burn my hands!",
            "Wow, thats hot!",
        }}, {"warm", new List<string>() {
            "Ohh, yeah, that feels nice and warm",
            "The warm air will keep my fingers moving.",
        }}, {"cold", new List<string>() {
            "Ah, getting colder. If I don't keep my hands warm, they will freeze up.",
            "Need to keep my fingers warm, else they'll blacken...",
            "Losing circulation here...",
        }}, {"frostbite", new List<string>() {
            "Frostbite! That fingers gone...",
            "Oh no... The beginning of the end...",
        }}, {"flare", new List<string>() {
            "A roadflare! I need to strike this to light it.",
            "Flares, gotta find something hard to slam it aginst.",
        }}, {"match", new List<string>() {
            "A few matches.. Going to be hard to swipe these with my cold hands.",
            "Flares, gotta find something hard to slam it aginst.",
        }}, {"fuel", new List<string>() {
            "Ok, I can light this to buy myself a bit more time.",
            "Yes.. This should burn nicley.",
        }}
    };

    // Start a timer to make sure we don't repeat ourselves in quick succesion
    Dictionary<string, double> timer = new Dictionary<string, double>();
    float interval = 10f;
    

    public void Tip(string topic) {
        // For a certian topic, load the tips for that one,
        // and put it on the screen
        // TODO for now, just run out of tips if they go by

        // Don't re-inform us if it was very recent
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        double now = (double)(System.DateTime.UtcNow - epochStart).TotalSeconds;
        if (timer.ContainsKey(topic) && now - timer[topic] < interval) return;

        List<string> list = d[topic];
        if (list.Count == 0) return;
        GetComponentInChildren<Text>().text = list.First();
        list.RemoveAt(0);
        timer[topic] = now;
    }
}
