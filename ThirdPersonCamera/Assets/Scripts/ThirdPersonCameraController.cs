using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    public Transform Target, Player;
    float mouseX, mouseY;
    public float RotationSpeed = 1;

    //Obstruction
    private Transform Obstruction;
    public float AutoZoomSpeed = 2f;

    int layerMaskToRaycast;

    //Camera Location and Zoom
    public float playerZoomSpeed = 20f;
    public float obstructionProximityLimit = 3.0f;
    public float cameraMinZoom = 1f;
    public float cameraMaxZoom = 10f;
    private float distanceToTarget;
    private float currentZoom;

    void Start() {
        Obstruction = Target;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        layerMaskToRaycast = LayerMask.GetMask("CameraBlockers");
        currentZoom = transform.localPosition.z;
    }

    private void LateUpdate() {
        CamControl();
        ViewObstructed();
    }

    void CamControl() {
        mouseX += Input.GetAxis("Mouse X") * RotationSpeed;
        mouseY -= Input.GetAxis("Mouse Y") * RotationSpeed;
        mouseY = Mathf.Clamp(mouseY, -35, 60);

        transform.LookAt(Target);

        distanceToTarget = Vector3.Distance(Target.position, transform.position);

        //Control Camera independent of character
        if (Input.GetKey(KeyCode.Mouse0)) {
            Target.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        } else {
            Target.rotation = Quaternion.Euler(mouseY, mouseX, 0);
            Player.rotation = Quaternion.Euler(0, mouseX, 0);
        }
        
        //Zoom Controls
        if ((Input.GetAxis("Mouse ScrollWheel") > 0f) && (currentZoom < -cameraMinZoom)) { // Zoom forward 
            transform.Translate(Vector3.forward * playerZoomSpeed * Time.deltaTime);
            currentZoom = transform.localPosition.z;
        }
        else if ((Input.GetAxis("Mouse ScrollWheel") < 0f) && (currentZoom > -cameraMaxZoom)) { // Zoom backward
            transform.Translate(Vector3.back * playerZoomSpeed * Time.deltaTime);
            currentZoom = transform.localPosition.z;
        }
    }

    void ViewObstructed() {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Target.position - transform.position, out hit, distanceToTarget, layerMaskToRaycast)) {
            if (hit.collider.gameObject.tag != "Player") { 
                Obstruction = hit.transform;
                Obstruction.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.ShadowsOnly;

                if (Vector3.Distance(Obstruction.position, transform.position) >= obstructionProximityLimit && Vector3.Distance(transform.position, Target.position) >= currentZoom) {
                    transform.Translate(Vector3.forward * AutoZoomSpeed * Time.deltaTime);
                }
            }
        }
        else { //Zoom back out to Zoom set by player and turn meshrenderer back on.
            Obstruction.gameObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            if (-Vector3.Distance(transform.position, Target.position) > currentZoom) {
                transform.Translate(Vector3.back * AutoZoomSpeed * Time.deltaTime);
            }
        }
    }
}
