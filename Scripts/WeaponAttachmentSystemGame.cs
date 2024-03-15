using System.Collections;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WeaponAttachmentSystemGame : MonoBehaviour {


    public static string weaponCompleteJson;


    [SerializeField] private Button backButton;
    [SerializeField] private Transform weaponContainerTransform;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private AnimatorController pistolAnimatorController;
    [SerializeField] private AnimatorController rifleAnimatorController;
    [SerializeField] private WeaponBodyListSO weaponBodyListSO;


    private void Awake() {
        // Destroy old one
        Destroy(weaponContainerTransform.GetChild(0).gameObject);

        backButton.onClick.AddListener(() => {
            SceneManager.LoadScene(0);
        });

        if (weaponCompleteJson == null) return;

        WeaponComplete weaponComplete = WeaponComplete.LoadSpawnWeaponComplete(weaponCompleteJson, false);

        weaponComplete.transform.parent = weaponContainerTransform;
        weaponComplete.transform.localPosition = Vector3.zero;
        weaponComplete.transform.localEulerAngles = Vector3.zero;

        if (weaponComplete.GetWeaponBodySO() == weaponBodyListSO.pistolWeaponBodySO) {
            playerAnimator.runtimeAnimatorController = pistolAnimatorController;
        }
        if (weaponComplete.GetWeaponBodySO() == weaponBodyListSO.rifleAWeaponBodySO ||
            weaponComplete.GetWeaponBodySO() == weaponBodyListSO.rifleBWeaponBodySO) {
            playerAnimator.runtimeAnimatorController = rifleAnimatorController;
        }
    }

}