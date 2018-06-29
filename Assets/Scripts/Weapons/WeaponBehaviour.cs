using UnityEngine;

public class WeaponBehaviour : MonoBehaviour
{
    private Weapon weapon;

    public Camera eyes;
    public Transform weaponHolder;
    private RectTransform hud;

    [HideInInspector] public Player player;

    [HideInInspector] public bool isLocal;

    void Awake()
    {
        hud = GameObject.FindGameObjectWithTag("WeaponHud").GetComponent<RectTransform>();
    }

	void Update ()
    {
        if (!isLocal)
            return;

        if (weapon != null)
            weapon.OnUpdate();
	}

    public void SetWeapon(int index)
    {
        Weapon[] weapons = Resources.LoadAll<Weapon>("Weapons");
        SetWeapon(weapons[index]);
    }

    public void SetWeapon(Weapon newWeapon)
    {
        if (weapon != null)
        {
            weapon.OnDestroy();
        }

        // Weapon
        weapon = Instantiate(newWeapon);
        weapon.Setup(eyes, hud, transform, weaponHolder, this);
        weapon.OnStart();
    }

    public void Initialize(Player player, bool isLocal)
    {
        this.isLocal = isLocal;
        this.player = player;
    }

    public void OnWeaponUpdate(GameSparks.RT.RTPacket packet)
    {
        if (weapon != null)
        {
            weapon.OnWeaponUpdate(packet);
        }
    }
}
