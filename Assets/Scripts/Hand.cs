using UnityEngine;
using System.Collections;

// TODO redoc and whatnot all
public class Hand: MonoBehaviour {
   internal float moveSpeed = 10;
   internal float rotSpeed = 4;
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

        SetFingers();
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

    void SetFingers() {
        // Open / close fingers using keyboard inputs
        SetFinger("thumb", Input.GetKey("space"));
        SetFinger("index", Input.GetKey("f"));
        SetFinger("middle", Input.GetKey("d"));
        SetFinger("ring", Input.GetKey("s"));
        SetFinger("pinkie", Input.GetKey("a"));
    }

    void SetFinger(string name, bool closed) {
        // set a finger to be opened or closed
        transform.Find("Parts/"+name).GetComponentInChildren<Finger>().closed = closed;
    }

    
}