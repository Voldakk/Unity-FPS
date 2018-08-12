using UnityEngine;

public class SightBehaviour : WeaponPartBehaviour
{
    Sight data;

    public Transform adsPos;

    public Vector3 holderPos;
    Transform holder;

    Crosshair crosshair;

    GameObject scopeObject;

    Look look;

    float prevFov;
    float fov;
    float fovMod;

    bool ads;

    public override void SetPart(WeaponPart part)
    {
        base.SetPart(part);

        data = part as Sight;

        fov = Sight.baseFov * data.Zoom;
    }

    public void OnStart()
    {
        holder = weapon.weaponBehaviour.weaponHolder;
        holderPos = holder.localPosition;

        crosshair = FindObjectOfType<Crosshair>();

        if (data.scopePrefab != null)
        {
            scopeObject = Instantiate(data.scopePrefab, crosshair.transform.parent);
            scopeObject.SetActive(false);
        }

        look = GetComponentInParent<Look>();

        fovMod = weapon.camera.fieldOfView / fov;

        ads = false;
    }

    public void Ads()
    {
        if (ads)
            return;
        ads = true;

        Vector3 diff = adsPos.position - weapon.camera.transform.position;
        holder.position = holder.position - diff;

        prevFov = weapon.camera.fieldOfView;
        weapon.camera.fieldOfView = fov;
        weapon.weaponCamera.fieldOfView = fov;

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
        weapon.camera.fieldOfView = prevFov;
        weapon.weaponCamera.fieldOfView = prevFov;

        crosshair.Show();

        if (scopeObject != null)
            scopeObject.SetActive(false);

        look.XSensitivity *= fovMod;
        look.YSensitivity *= fovMod;
    }
}
