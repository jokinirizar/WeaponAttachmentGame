using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponBodyUI : MonoBehaviour {


    public static WeaponBodyUI Instance { get; private set; }



    [Serializable]
    public class WeaponPartButton {
        public WeaponPartSO.PartType partType;
        public Button button;
        public TextMeshProUGUI buttonTextMesh;
    }


    [SerializeField] private List<WeaponPartButton> weaponPartButtonList;


    private void Awake() {
        Instance = this;

        foreach (WeaponPartButton weaponPartButton in weaponPartButtonList) {
            weaponPartButton.button.onClick.AddListener(() => {
                WeaponAttachmentSystem.Instance.ChangePart(weaponPartButton.partType);
            });
        }
    }

    private void Start() {
        WeaponAttachmentSystem.Instance.OnAnyPartChanged += WeaponAttachmentSystem_OnAnyPartChanged;

        RefreshButtonTextMesh();
    }

    private void WeaponAttachmentSystem_OnAnyPartChanged(object sender, EventArgs e) {
        RefreshButtonTextMesh();
    }

    private void RefreshButtonTextMesh() {
        foreach (WeaponPartButton weaponPartButton in weaponPartButtonList) {
            // Don't like how the numbers look
            //weaponPartButton.buttonTextMesh.text = weaponPartButton.partType.ToString().ToUpper() + " " + (WeaponAttachmentSystem.Instance.GetPartIndex(weaponPartButton.partType)+1);
            weaponPartButton.buttonTextMesh.text = weaponPartButton.partType.ToString().ToUpper();
        }
        
    }

    public void ToggleVisibility() {
        gameObject.SetActive(!gameObject.activeSelf);
    }


}