using UnityEngine;
using System.Collections;

// TODO redoc and whatnot all
public class Hand: MonoBehaviour {
   internal float moveSpeed = 10;
   internal float rotSpeed = 4;
   internal float fingerSpeed = 10;
   internal Vector3 targetPos;
   internal Vector3 startingPos;

   // TODO move elsewhere
   bool paused = false;

   void Start() {
        targetPos = transform.position;
        startingPos = transform.position;

        // Ignore collision between palm and held objects
        Physics.IgnoreLayerCollision(
            LayerMask.NameToLayer("Grabbed"), LayerMask.NameToLayer("Palm"));
   }

   void Update() {
        if (Input.GetKey("escape")) paused = true;
        if (paused) {
            if (Input.GetMouseButton(0)) paused = false;
            else return;
        }

        SetTargetPosition();
        // can, like a claw machine, move down to grab
        Lower(Input.GetMouseButton(0));
       
        // Are we moveing hand or rotating it
        if (Input.GetMouseButton(1)) Rotate();
        else Move();

        MoveFingers();
   }

    void SetTargetPosition() {
        // Find the target position - 
        // a point on a plane above the ground,
        // where the users mouse 'is'
        Plane plane = new Plane(Vector3.up, targetPos);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        float point = 0f;
        if(plane.Raycast(ray, out point)) targetPos = ray.GetPoint(point);
    }

    void Move() {
        // Lerp hand pos to where the target position (Where the mouse is)
        Cursor.lockState = CursorLockMode.None;
        transform.position = Vector3.Lerp(transform.position, targetPos, moveSpeed * Time.deltaTime);
        Debug.DrawLine(transform.position,targetPos,Color.red);
    }

    void Rotate() {
        // Rotate hand based on relative mouse movement,
        // amnesia style
        Cursor.lockState = CursorLockMode.Locked;
        var rotX = Input.GetAxis("Mouse X") * rotSpeed;
        var rotY = Input.GetAxis("Mouse Y") * rotSpeed;
        
        transform.RotateAround(transform.position, Vector3.up, rotX);
        transform.RotateAround(transform.position, Vector3.right, rotY);

        // TODO limit rotation
    }

    void Lower(bool lower) {
        // Move hand down, grabbing something or dropping it on the ground

        
        var y = startingPos.y;
        // If hand is up, leave it at default height

        // If it is down, cast ray, and put hand a bit above it
        RaycastHit hit;
        // Ignore everything in palm & finger
        int layerMask = 1 << LayerMask.NameToLayer("Finger");
        layerMask |= 1 << LayerMask.NameToLayer("Palm");
        layerMask |= 1 << LayerMask.NameToLayer("Grabbed");
        layerMask = ~layerMask;
        // Want to fire the ray from higher up, in case we need to move above
        var pos = transform.position + Vector3.up;
        bool didHit = Physics.Raycast(pos, -Vector3.up, out hit, 10f, layerMask);
        if (lower && didHit) {
            Debug.DrawRay(transform.position, -Vector3.up * hit.distance, Color.yellow);
            y = hit.point.y + .6f;
        }
        targetPos = new Vector3(targetPos.x, y, targetPos.z);
    }

    void MoveFingers() {
        // Open / close fingers
        MoveFinger("thumb", Input.GetKey("space"));
        MoveFinger("index", Input.GetKey("f"));
        MoveFinger("middle", Input.GetKey("d"));
        MoveFinger("ring", Input.GetKey("s"));
        MoveFinger("pinkie", Input.GetKey("a"));
    }

    void MoveFinger(string name, bool closed) {
        // Move an individual finger, by name

        // TODO why is it reversed?
        var t = transform.Find("Parts/"+name+"/Rig");
        var to = closed ? t.Find("closed") : t.Find("open");

        // TODO pink / black finggers from cold
        LerpTransform(t.Find("target"), to);

        // If the finger is closed, it can grab stuff
        // note - we do not use enable/disable, because
        // it does not fire 'OnTriggerExit' - so we
        // use this location hack
        // TODO likely not best place for it
        if (closed) t.Find("closed").GetComponent<SphereCollider>().center = Vector3.zero;
        else t.Find("closed").GetComponent<SphereCollider>().center = new Vector3(0, 1000, 0);
    }

    void LerpTransform(Transform from, Transform to, float speedMod = 1) {
        // Lerp a transform, using finger speed, with an optional speed modifier
        var speed = fingerSpeed * Time.deltaTime * speedMod;
        from.position = Vector3.Lerp(from.position, to.position, speed);
        from.rotation = Quaternion.Lerp (from.rotation, to.rotation, speed);
    }
}