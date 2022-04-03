using System; // TODO I hate that I can't just import System.Func
using Convert = System.Convert;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

// Helper script
public class Helper : MonoBehaviour {

    public static List<T> FindAllScripts<T>(Func<T, bool> filter = null) where T: MonoBehaviour {
        // A helper method to return every instance of a given script type,
        // and filter based on a predicate

        // Get the scripts that match our query type
        List<Object> matchingObjs = FindObjectsOfType(typeof(T)).ToList();
        // Cast object type to the given script type
        List<T> matchingScripts = matchingObjs.Select(o => o as T).ToList();
        // Apply given predicate filter, if there is one, otherwise use this trivial always true predicate
        return matchingScripts.Where(filter ?? (s => true)).ToList();
    }

    public static List<GameObject> FindNearby<T>(GameObject go, float radius, bool ignoreSelf=true, Func<T, bool> filter = null) where T: MonoBehaviour {
        // Find all scripts of a certian type that are phyiscally
        // nearby, within a certian radius
        List<T> nearby = Helper.FindAllScripts<T>(script =>
            (filter == null || filter(script))
            && !(ignoreSelf && script.gameObject == go) // Ignore the calling object
            && Vector3.Distance(script.transform.position, go.transform.position) < radius
        );
        return nearby.Select(s => s.gameObject).ToList();
    }

    public static Vector3 RandomPoint(float radius) {
        //Return a random point in a radius
        return new Vector3(
            Random.Range(-radius, radius),
            Random.Range(-radius, radius),
            Random.Range(-radius, radius));
    }

    public static Vector3 ClosestPoint(GameObject go, Vector3 pos) {
        // Return the closest point on any of the target gameObjects colliders
        Vector3 closest = new Vector3(1000, 1000, 1000);
        foreach (var c in go.GetComponentsInChildren<Collider>()) {
            var p = c.ClosestPoint(pos);
            if (Vector3.Distance(pos, p) < Vector3.Distance(pos, closest)) {
                closest = p;
            }
        }
        return closest;
    }

    public static void LerpTransform(Transform from, Transform to, float speed) {
        // Lerp a transform, using finger speed, with an optional speed modifier
        from.position = Vector3.Lerp(from.position, to.position, speed * Time.deltaTime);
        from.rotation = Quaternion.Lerp (from.rotation, to.rotation, speed * Time.deltaTime);
    }

    public static void Tip(string topic) {
        FindObjectOfType<Tips>().Tip(topic);
    }

    public static TimeSpan Now() {
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return System.DateTime.UtcNow - epochStart;
    }

    public static TimeSpan RoundTime(TimeSpan? span) {
        if (span == null) return new TimeSpan();
        return TimeSpan.FromSeconds(Math.Round(((TimeSpan) span).TotalSeconds));
    }

    public static bool JustStarted() {
        var st = FindObjectOfType<Menu>().startTime;
        if (st == null) return true;
        return (Helper.Now() - (TimeSpan) st).TotalSeconds < 3f;
    }


    // Just for easy copying:

    // void OnDrawGizmos()  {
    //     Handles.Label(transform.position, "Text: "+v);
    // }
}
