using CodeMonkey.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAutoRotate : MonoBehaviour {


    private FunctionPeriodic randomizeFunctionPeriodic;
    private FunctionPeriodic changeBodyFunctionPeriodic;

    private void Start() {
        randomizeFunctionPeriodic = FunctionPeriodic.Create(WeaponAttachmentSystem.Instance.RandomizeParts, .3f);
        changeBodyFunctionPeriodic = FunctionPeriodic.Create(WeaponAttachmentSystem.Instance.ChangeBody, 3f);
    }

    private void Update() {
        float rotateSpeed = 50;
        transform.eulerAngles += new Vector3(0f, rotateSpeed * Time.deltaTime, 0f);
    }

    private void OnDestroy() {
        randomizeFunctionPeriodic.DestroySelf();
        changeBodyFunctionPeriodic.DestroySelf();
    }

}