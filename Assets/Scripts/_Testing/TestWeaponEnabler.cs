using UnityEngine;

public class TestWeaponEnabler : MonoBehaviour
{
    public Weapon weapon;

    public WeaponBehaviour weaponBehaviour;

	void Start ()
    {
        weaponBehaviour.Initialize(null, true);
        weaponBehaviour.SetWeapon(weapon);
    }
}
