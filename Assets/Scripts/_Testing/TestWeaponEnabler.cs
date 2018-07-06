using UnityEngine;

public class TestWeaponEnabler : MonoBehaviour
{
    public int weapon;

    public WeaponBehaviour weaponBehaviour;

	void Start ()
    {
        weaponBehaviour.Initialize(null);
        weaponBehaviour.SetWeapon(weapon);
    }
}
