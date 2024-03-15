using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class WeaponComplete : MonoBehaviour {



    public class AttachedWeaponPart {

        public WeaponBody.PartTypeAttachPoint partTypeAttachPoint;
        public WeaponPartSO weaponPartSO;
        public Transform spawnedTransform;

    }


    [SerializeField] private List<WeaponPartSO> defaultWeaponPartSOList;


    private WeaponBody weaponBody;
    private Dictionary<WeaponPartSO.PartType, AttachedWeaponPart> attachedWeaponPartDic;


    private void Awake() {
        weaponBody = GetComponent<WeaponBody>();

        attachedWeaponPartDic = new Dictionary<WeaponPartSO.PartType, AttachedWeaponPart>();

        foreach (WeaponBody.PartTypeAttachPoint partTypeAttachPoint in weaponBody.GetPartTypeAttachPointList()) {
            attachedWeaponPartDic[partTypeAttachPoint.partType] = new AttachedWeaponPart {
                partTypeAttachPoint = partTypeAttachPoint,
            };
        }

        foreach (WeaponPartSO weaponPartSO in defaultWeaponPartSOList) {
            SetPart(weaponPartSO);
        }
    }


    public void SetPart(WeaponPartSO weaponPartSO) {
        // Destroy currently attached part
        if (attachedWeaponPartDic[weaponPartSO.partType].spawnedTransform != null) {
            Destroy(attachedWeaponPartDic[weaponPartSO.partType].spawnedTransform.gameObject);
        }

        // Spawn new part
        Transform spawnedPartTransform = Instantiate(weaponPartSO.prefab);
        AttachedWeaponPart attachedWeaponPart = attachedWeaponPartDic[weaponPartSO.partType];
        attachedWeaponPart.spawnedTransform = spawnedPartTransform;

        Transform attachPointTransform = attachedWeaponPart.partTypeAttachPoint.attachPointTransform;
        spawnedPartTransform.parent = attachPointTransform;
        spawnedPartTransform.localEulerAngles = Vector3.zero;
        spawnedPartTransform.localPosition = Vector3.zero;

        attachedWeaponPart.weaponPartSO = weaponPartSO;

        attachedWeaponPartDic[weaponPartSO.partType] = attachedWeaponPart;

        // Is it a barrel?
        if (weaponPartSO.partType == WeaponPartSO.PartType.Barrel) {
            BarrelWeaponPartSO barrelWeaponPartSO = (BarrelWeaponPartSO)weaponPartSO;

            AttachedWeaponPart barrelPartTypeAttachedWeaponPart = attachedWeaponPartDic[WeaponPartSO.PartType.Barrel];
            AttachedWeaponPart muzzlePartTypeAttachedWeaponPart = attachedWeaponPartDic[WeaponPartSO.PartType.Muzzle];

            muzzlePartTypeAttachedWeaponPart.partTypeAttachPoint.attachPointTransform.position =
                barrelPartTypeAttachedWeaponPart.partTypeAttachPoint.attachPointTransform.position +
                barrelPartTypeAttachedWeaponPart.partTypeAttachPoint.attachPointTransform.forward * barrelWeaponPartSO.muzzleOffset;
        }
    }

    public WeaponPartSO GetWeaponPartSO(WeaponPartSO.PartType partType) {
        AttachedWeaponPart attachedWeaponPart = attachedWeaponPartDic[partType];
        return attachedWeaponPart.weaponPartSO;
    }

    public List<WeaponPartSO.PartType> GetWeaponPartTypeList() {
        return new List<WeaponPartSO.PartType>(attachedWeaponPartDic.Keys);
    }

    public WeaponBodySO GetWeaponBodySO() {
        return weaponBody.GetWeaponBodySO();
    }



    public string Save() {
        List<WeaponPartSO> weaponPartSOList = new List<WeaponPartSO>();
        foreach (WeaponPartSO.PartType partType in attachedWeaponPartDic.Keys) {
            if (attachedWeaponPartDic[partType].weaponPartSO != null) {
                weaponPartSOList.Add(attachedWeaponPartDic[partType].weaponPartSO);
            }
        }

        SaveObject saveObject = new SaveObject() {
            weaponBodySO = GetWeaponBodySO(),
            weaponPartSOList = weaponPartSOList,
        };

        string json = JsonUtility.ToJson(saveObject);

        return json;
    }

    public void Load(string json) {
        SaveObject saveObject = JsonUtility.FromJson<SaveObject>(json);

        foreach (WeaponPartSO weaponPartSO in saveObject.weaponPartSOList) {
            SetPart(weaponPartSO);
        }
    }


    [Serializable]
    public class SaveObject {

        public WeaponBodySO weaponBodySO;
        public List<WeaponPartSO> weaponPartSOList;

    }





    public static WeaponComplete LoadSpawnWeaponComplete(string json, bool spawnUI) {
        SaveObject saveObject = JsonUtility.FromJson<SaveObject>(json);

        Transform weaponCompleteTransform = Instantiate(saveObject.weaponBodySO.prefab);
        WeaponComplete weaponComplete = weaponCompleteTransform.GetComponent<WeaponComplete>();

        weaponComplete.Load(json);

        if (spawnUI) {
            Instantiate(saveObject.weaponBodySO.prefabUI, weaponCompleteTransform);
        }

        return weaponComplete;
    }


}