using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{
    public Weapon startingWeapon;
    private Weapon weapon;

    public Camera eyes;
    public RectTransform hud;
    public Transform weaponHolder;

    void Start ()
    {
        SetWeapon(startingWeapon);
	}

	void Update ()
    {
        if (weapon != null)
            weapon.OnUpdate();
	}

    public void SetWeapon(Weapon newWeapon)
    {
        if (weapon != null)
        {
            weapon.OnDestroy();
        }

        // Weapon
        weapon = Instantiate(newWeapon);
        weapon.Setup(eyes, hud, transform, weaponHolder);
        weapon.OnStart();
    }
}
