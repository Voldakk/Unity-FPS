using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiWeaponStats : MonoBehaviour
{
    public ModularWeapon weapon;
    public Text text;

    private void Update()
    {
        /*text.text = string.Format(" accuracy: {0} \n recoil: {1} \n stability: {2} \n firerate: {3} \n magSize: {4} \n ",
            weapon.Accuracy,
            weapon.Recoil,
            weapon.Stability,
            weapon.Firerate,
            weapon.Damage,
            weapon.MagSize);*/
    }
}
