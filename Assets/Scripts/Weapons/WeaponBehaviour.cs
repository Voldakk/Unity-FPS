using UnityEngine;
using GameSparks.RT;

public class WeaponBehaviour : NetworkObject
{
    public ModularWeapon weapon;

    public Camera eyes;
    public Camera weaponCamera;
    public Transform weaponHolder;
    private RectTransform hud;

    [HideInInspector] public Player player;

    public enum OpCode { SetWeapon, Last }

    protected override void Awake()
    {
        base.Awake();

        hud = GameObject.FindGameObjectWithTag("WeaponHud").GetComponent<RectTransform>();
    }

	void Update ()
    {
        if (!isOwner)
            return;

        if (weapon != null)
            weapon.OnUpdate();
	}

    public void SetWeapon(int index)
    {
        PlayerData.instance.LoadWeapon(PlayerData.instance.currentWeapon, weapon);

        if (isOwner)
            SendString((int)OpCode.SetWeapon, 1, PlayerData.instance.WeaponToJson(weapon));

        weapon.Setup(hud, this);
        weapon.OnStart();
    }

    public void SetWeapon(string json)
    {
        PlayerData.instance.JsonToWeapon(weapon, json);
        weapon.Setup(hud, this);
        weapon.OnStart();
    }

    public void Initialize(Player player)
    {
        this.player = player;
    }

    public void OnWeaponUpdate(RTPacket packet, int code)
    {
        if (weapon != null)
            weapon.OnWeaponUpdate(packet, code);
    }

    public override void OnPacket(RTPacket packet, int code)
    {
        base.OnPacket(packet);

        switch ((OpCode)code)
        {
            case OpCode.SetWeapon:
                SetWeapon(packet.Data.GetString(1));
                break;

            default:
                OnWeaponUpdate(packet, code);
                break;
        }
    }
}
