using UnityEngine;
using GameSparks.RT;
using System.Collections.Generic;

public class ModularWeapon : MonoBehaviour
{
    [HideInInspector] public Camera camera;
    [HideInInspector] public Camera weaponCamera;
    [HideInInspector] public RectTransform hud;
    [HideInInspector] public WeaponBehaviour weaponBehaviour;
    [HideInInspector] public bool isOwner;
    public enum OpCode { Fire = WeaponBehaviour.OpCode.Last, Reload, Ads, StopAds, Last }

    public string weaponName;

    public WeaponPartSlot bodySlot;
    private BodyBehaviour body;

    SightBehaviour sight;

    public Dictionary<WeaponPartStats, float> stats;
    public Dictionary<WeaponPartStats, float> modifiers;

    void Awake()
    {
        bodySlot = GetComponentInChildren<WeaponPartSlot>();
    }

    public void Setup( RectTransform hud, WeaponBehaviour weaponBehaviour)
    {
        this.hud = hud;
        this.weaponBehaviour = weaponBehaviour;
        camera = weaponBehaviour.eyes;
        weaponCamera = weaponBehaviour.weaponCamera;

        isOwner = weaponBehaviour.isOwner;
    }

    public void OnStart()
    {
        if (!isOwner)
        {
            MeshRenderer[] mrs = GetComponentsInChildren<MeshRenderer>();
            foreach (var mr in mrs)
            {
                mr.gameObject.layer = 0;
            }
        }

        body = GetComponentInChildren<BodyBehaviour>();
        if (body != null)
            body.OnStart();

        sight = GetComponentInChildren<SightBehaviour>();
        if (sight != null)
            sight.OnStart();
    }

    public void OnUpdate()
    {
        if (Input.GetButton("Fire1"))
        {
            if (body != null)
                body.Fire(true);
        }
        if(Input.GetButtonDown("Reload"))
        {
            body.Reload();
        }

        if (sight != null)
        {
            if (Input.GetButtonDown("Fire2"))
            {
                sight.Ads();
            }
            if (Input.GetButtonUp("Fire2"))
            {
                sight.StopAds();
            }
        }
    }

    public void OnWeaponUpdate(RTPacket packet, int code)
    {

    }

    void FindParts(ref List<WeaponPart> parts, WeaponPart part)
    {
        if (part == null)
            return;

        parts.Add(part);

        for (int i = 0; i < part.slots.Length; i++)
        {
            FindParts(ref parts, part.slots[i].part);
        }
    }

    public void UpdateStats()
    {
        stats = new Dictionary<WeaponPartStats, float>();
        modifiers = new Dictionary<WeaponPartStats, float>();

        if (bodySlot == null || bodySlot.part == null)
            return;

        var parts = new List<WeaponPart>();
        FindParts(ref parts, bodySlot.part);
        
        foreach (var part in parts)
        {
            if (part == null || part == null)
                continue;

            foreach (var stat in part.stats)
            {
                switch (stat.statsType)
                {
                    case StatsType.Base:
                    case StatsType.Additive:
                        if (stats.ContainsKey(stat.stats))
                            stats[stat.stats] += stat.GetValue(part);
                        else
                            stats.Add(stat.stats, stat.GetValue(part));
                        break;
                    case StatsType.Modifier:
                        if (modifiers.ContainsKey(stat.stats))
                            modifiers[stat.stats] *= stat.GetValue(part);
                        else
                            modifiers.Add(stat.stats, stat.GetValue(part));
                        break;
                    default:
                        Debug.LogError("ModularWeapon::UpdateStats - unknown StatsType");
                        break;
                }
            }
        }

        List<WeaponPartStats> keys = new List<WeaponPartStats>(stats.Keys);
        foreach (var key in keys)
        {
            if (modifiers.ContainsKey(key))
            {
                stats[key] *= modifiers[key];
                modifiers.Remove(key);
            }
        }
    }

    public float GetStats(WeaponPartStats stat, float defaultValue)
    {
        if (stats.ContainsKey(stat))
            return stats[stat];

        return defaultValue;
    }

    public float GetModifier(WeaponPartStats stat, float defaultValue)
    {
        if (modifiers.ContainsKey(stat))
            return stats[stat];

        return defaultValue;
    }
}