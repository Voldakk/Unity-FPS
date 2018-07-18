using UnityEngine;

public class SightBehaviour : WeaponPartBehaviour
{
    Sight data;

    public Transform adsPos;

    public Vector3 holderPos;
    Transform holder;

    float fov;

    Crosshair crosshair;

    GameObject scopeObject;

    Look look;

    float fovMod;

    bool ads;

    public override void SetPart(WeaponPart part)
    {
        base.SetPart(part);

        data = part as Sight;
    }

    public void OnStart()
    {
        holder = weapon.weaponBehaviour.weaponHolder;
        holderPos = holder.localPosition;

        crosshair = FindObjectOfType<Crosshair>();

        if (data.scopePrefab != null)
        {
            scopeObject = Instantiate(data.scopePrefab, crosshair.transform);
            scopeObject.SetActive(false);
        }

        look = GetComponentInParent<Look>();

        fovMod = weapon.camera.fieldOfView / data.fov;

        ads = false;
    }

    public void Ads()
    {
        if (ads)
            return;
        ads = true;

        Vector3 diff = adsPos.position - weapon.camera.transform.position;
        holder.position = holder.position - diff;

        fov = weapon.camera.fieldOfView;
        weapon.camera.fieldOfView = data.fov;
        weapon.weaponCamera.fieldOfView = data.fov;

        crosshair.Hide();

        if(scopeObject != null)
            scopeObject.SetActive(true);

        look.XSensitivity /= fovMod;
        look.YSensitivity /= fovMod;
    }

    public void StopAds()
    {
        if (!ads)
            return;
        ads = false;

        holder.localPosition = holderPos;
        weapon.camera.fieldOfView = fov;
        weapon.weaponCamera.fieldOfView = fov;

        crosshair.Show();

        if (scopeObject != null)
            scopeObject.SetActive(false);

        look.XSensitivity *= fovMod;
        look.YSensitivity *= fovMod;
    }
}
