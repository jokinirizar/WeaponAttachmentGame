using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WeaponBodySO : ScriptableObject {


    public enum Body {
        RifleA,
        RifleB,
        Pistol,
    }


    public Body body;
    public Transform prefab;
    public Transform prefabUI;
    public WeaponPartListSO weaponPartListSO;


}
