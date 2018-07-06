using GameSparks.RT;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Player : NetworkObject
{
    public Health Health { get; private set; }
    public WeaponBehaviour WeaponBehaviour { get; private set; }

    private new Rigidbody rigidbody;

    public void OnDeath()
    {
        Debug.Log("Player::OnDeath");
    }

    void Start()
    {
        GameManager.Instance().playerList[owner-1] = this;
        Initialize();
    }

    public void Initialize()
    {
        rigidbody = GetComponent<Rigidbody>();
        WeaponBehaviour = GetComponent<WeaponBehaviour>();
        WeaponBehaviour.Initialize(this);

        Health = GetComponent<Health>();
        Health.Initialize(isOwner);

        if (isOwner)
        {
            transform.Find("Model").gameObject.SetActive(false);

            WeaponBehaviour.SetWeapon(PlayerSetting.Current.startingWeapon);
        }
        else
        {
            Destroy(GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>());
            Destroy(GetComponent<Look>());

            foreach (var c in GetComponentsInChildren<AudioListener>())
                c.enabled = false;

            foreach (var c in GetComponentsInChildren<FlareLayer>())
                c.enabled = false;

            foreach (var c in GetComponentsInChildren<Camera>())
                c.enabled = false;
        }
    }
}