﻿using GameSparks.RT;
using UnityEngine;

public class WeaponBehaviour : NetworkObject
{
    private Weapon weapon;

    public Camera eyes;
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
        Weapon[] weapons = Resources.LoadAll<Weapon>("Weapons");
        Weapon newWeapon = weapons[index];

        if(isOwner)
        {
            SendInt((int)OpCode.SetWeapon, 1, index);
        }

        if (weapon != null)
        {
            weapon.OnDestroy();
        }

        // Weapon
        weapon = Instantiate(newWeapon);
        weapon.Setup(eyes, hud, transform, weaponHolder, this);
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
                SetWeapon(packet.Data.GetInt(1).Value);
                break;

            default:
                OnWeaponUpdate(packet, code);
                break;
        }
    }
}
