using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Tips : MonoBehaviour {
    
    Dictionary<string, List<string>> d = new Dictionary<string, List<string>>(){
        {"heavy", new List<string>() {
            "Oh, that's heavy.\nI'll have to grip it with more fingers...",
            "Arg, I have to get a better grip.",
            "Barley holding on here, need to get a better grip.",
            "Ah, only got a few fingers touching, gonna drop it...",
        }},{"grip", new List<string>() {
            "Ah, this is fiddly... Let me try more fingers...",
            "Hrm, got to reposition my hand.",
            "Slippery...",
        }}, {"frost", new List<string>() {
            "Argh, I hate holding this - so cold!",
            "Hm, it's frozen, I'll need to warm it first.",
            "Yeah, this is wet, I'll need to dry it.",
            "Oh man that's cold. Going to loose a finger.",
            "Ouch, this hurts just to hold",
        }}, {"burned", new List<string>() {
            "Ow! I'm going to burn my hands!",
            "Wow, thats hot!",
        }}, {"charred", new List<string>() {
            "AHH! IT BURNS!",
            "Too hot! Too hot!",
        }}, {"warm", new List<string>() {
            "Ohh, yeah, that feels nice and warm",
            "The warm air will keep my fingers moving.",
        }}, {"cold", new List<string>() {
            "Ah, getting colder. If I don't keep my hands warm, they will freeze up.",
            "Need to keep my fingers warm, else they'll blacken...",
            "Losing circulation here...",
        }}, {"frostbitten", new List<string>() {
            "Frostbite! That fingers gone...",
            "Oh no... The beginning of the end...",
        }}, {"flare", new List<string>() {
            "A roadflare! I think the gray end is the striker...",
            "Seems I need to strike this pretty hard to light it.",
            "Flares, gotta find something hard to slam it aginst.",
        }}, {"match", new List<string>() {
            "A few matches.. Going to be hard to swipe these with my cold hands.",
            "Darn, these little matches are cumbersome...",
        }}, {"fuel", new List<string>() {
            "Ok, I can light this to buy myself a bit more time.",
            "Yes.. This should burn nicley.",
        }}
    };

    // Start a timer to make sure we don't repeat ourselves in quick succesion
    Dictionary<string, double> timer = new Dictionary<string, double>();
    float interval = 10f;
    
    // Also, dont rapid fire anything after anything
    double anythingTimer;
    float anythingInterval = 5f;
    public void Tip(string topic) {
        // For a certian topic, load the tips for that one,
        // and put it on the screen
        // TODO for now, just run out of tips if they go by

        Debug.Log("Tipping with "+topic);

        // Don't re-inform us if it was very recent
        double nowSeconds = Helper.Now().TotalSeconds;
        if (timer.ContainsKey(topic) && nowSeconds - timer[topic] < interval) return;

        // Bypass anything timer if the message is about injury
        bool urgent = topic == "frostbitten" || topic == "charred";
        if (!urgent && nowSeconds - anythingTimer < anythingInterval) return;
        anythingTimer = nowSeconds;

        List<string> list = d[topic];
        if (list.Count == 0) return;
        GetComponentInChildren<Text>().text = list.First();
        list.RemoveAt(0);
        timer[topic] = nowSeconds;

        // Background starts transparent, just need a little readability
        var img = GetComponentInChildren<Image>();
        var c = img.color;
        c.a = .1f;
        img.color = c;
    }
}
