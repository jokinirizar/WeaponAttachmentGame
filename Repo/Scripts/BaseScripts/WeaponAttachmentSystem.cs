using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static WeaponAttachmentSystem;

public class WeaponAttachmentSystem : MonoBehaviour {


    public static WeaponAttachmentSystem Instance { get; private set; }


    public event EventHandler OnAnyPartChanged;


    [SerializeField] private WeaponBodyListSO weaponBodyListSO;
    [SerializeField] private WeaponBodySO weaponBodySO;
    [SerializeField] private WeaponComplete weaponComplete;



    private void Awake() {
        Instance = this;
    }

    private void Start() {
        SetWeaponBody(weaponBodySO);
    }

    /*
    private void Update() {
        if (Input.GetKeyDown(KeyCode.T)) {
            weaponComplete.Save();
        }
        if (Input.GetKeyDown(KeyCode.Y)) {
            string json = "{\"weaponPartSOList\":[{\"instanceID\":55448},{\"instanceID\":63630},{\"instanceID\":63752},{\"instanceID\":82794},{\"instanceID\":90262}]}";
            weaponComplete.Load(json);
        }
    }*/


    public void SetWeaponBody(WeaponBodySO weaponBodySO) {
        Vector3 previousEulerAngles = Vector3.zero;

        if (weaponComplete != null) {
            // Clear previous WeaponComplete
            previousEulerAngles = weaponComplete.transform.eulerAngles;
            Destroy(weaponComplete.gameObject);
        }

        this.weaponBodySO = weaponBodySO;

        Transform weaponBodyTransform = Instantiate(weaponBodySO.prefab);
        weaponBodyTransform.eulerAngles = previousEulerAngles;
        weaponComplete = weaponBodyTransform.GetComponent<WeaponComplete>();

        Instantiate(weaponBodySO.prefabUI, weaponBodyTransform);

        // Auto Rotate for recording video
        //Debug.Log("AUTO ROTATE");
        //weaponBodyTransform.AddComponent<WeaponAutoRotate>();
    }

    public int GetPartIndex(WeaponPartSO.PartType partType) {
        WeaponPartSO equippedWeaponPartSO = weaponComplete.GetWeaponPartSO(partType);
        if (equippedWeaponPartSO == null) {
            return 0;
        } else {
            List<WeaponPartSO> weaponPartSOList = weaponBodySO.weaponPartListSO.GetWeaponPartSOList(partType);
            int partIndex = weaponPartSOList.IndexOf(equippedWeaponPartSO);
            return partIndex;
        }
    }

    public void ChangePart(WeaponPartSO.PartType partType) {
        WeaponPartSO equippedWeaponPartSO = weaponComplete.GetWeaponPartSO(partType);
        if (equippedWeaponPartSO == null) {
            weaponComplete.SetPart(weaponBodySO.weaponPartListSO.GetWeaponPartSOList(partType)[0]);

            OnAnyPartChanged?.Invoke(this, EventArgs.Empty);
        } else {
            List<WeaponPartSO> weaponPartSOList = weaponBodySO.weaponPartListSO.GetWeaponPartSOList(partType);
            int partIndex = weaponPartSOList.IndexOf(equippedWeaponPartSO);
            int nextPartIndex = (partIndex + 1) % weaponPartSOList.Count;
            weaponComplete.SetPart(weaponPartSOList[nextPartIndex]);

            OnAnyPartChanged?.Invoke(this, EventArgs.Empty);
        }

    }

    public void RandomizeParts() {
        foreach (WeaponPartSO.PartType partType in weaponComplete.GetWeaponPartTypeList()) {
            int randomAmount = UnityEngine.Random.Range(0, 50);
            for (int i = 0; i < randomAmount; i++) {
                ChangePart(partType);
            }
        }
    }

    public WeaponBodySO GetWeaponBodySO() {
        return weaponBodySO;
    }

    public void ChangeBody() {
        if (weaponBodySO == weaponBodyListSO.rifleAWeaponBodySO) {
            SetWeaponBody(weaponBodyListSO.rifleBWeaponBodySO);
        } else {
            SetWeaponBody(weaponBodyListSO.rifleAWeaponBodySO);
        }
    }

    public WeaponComplete GetWeaponComplete() {
        return weaponComplete;
    }

    public void SetWeaponComplete(WeaponComplete weaponComplete) {
        if (this.weaponComplete != null) {
            // Clear previous WeaponComplete
            Destroy(this.weaponComplete.gameObject);
        }

        weaponBodySO = weaponComplete.GetWeaponBodySO();

        this.weaponComplete = weaponComplete;
    }

    public void ResetWeaponRotation() {
        weaponComplete.transform.eulerAngles = Vector3.zero;
    }

}