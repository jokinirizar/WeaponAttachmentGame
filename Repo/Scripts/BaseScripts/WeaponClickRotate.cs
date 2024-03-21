using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponClickRotate : MonoBehaviour {



    private Transform weaponTransform;
    private Vector3 lastMousePosition;



    private void Awake() {
        weaponTransform = transform;
    }

    private void Update() {
        if (Input.GetMouseButton(0)) {
            Vector3 mouseDelta = lastMousePosition - Input.mousePosition;


            // Fix huge jumps when Unity loses focus
            mouseDelta.y = Mathf.Clamp(mouseDelta.y, -200, +200);
            mouseDelta.x = Mathf.Clamp(mouseDelta.x, -200, +200);

            float rotateSpeed = .2f;

            weaponTransform.localEulerAngles += new Vector3(mouseDelta.y, mouseDelta.x, 0f) * rotateSpeed;

            float rotationXMin = -7f;
            float rotationXMax = +10f;

            float localEulerAnglesX = weaponTransform.localEulerAngles.x;
            if (localEulerAnglesX > 180) {
                localEulerAnglesX -= 360f;
            }
            float rotationX = Mathf.Clamp(localEulerAnglesX, rotationXMin, rotationXMax);

            weaponTransform.localEulerAngles = new Vector3(rotationX, weaponTransform.localEulerAngles.y, weaponTransform.localEulerAngles.z);
        }

        lastMousePosition = Input.mousePosition;
    }

}