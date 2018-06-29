using GameSparks.RT;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour
{
    [HideInInspector]
    public int peerId;

    public Health Health { get; private set; }
    public WeaponBehaviour WeaponBehaviour { get; private set; }

    private new Rigidbody rigidbody;

    private Vector3 prevPos;
    private Quaternion prevRot;

    public float updateRate;

    private Vector3 goToPos;
    private Vector2 goToRot;

    private bool isLocal;

    public void OnDeath()
    {
        Debug.Log("Player::OnDeath");
    }

    public void Initialize(bool value)
    {
        isLocal = value;

        rigidbody = GetComponent<Rigidbody>();
        WeaponBehaviour = GetComponent<WeaponBehaviour>();
        WeaponBehaviour.Initialize(this, isLocal);

        Health = GetComponent<Health>();
        Health.Initialize(isLocal);

        if (value)
        {
            transform.Find("Model").gameObject.SetActive(false);
            prevPos = transform.position;
            prevRot = transform.rotation;
            StartCoroutine(SendMovement());
        }
        else
        {
            Destroy(GetComponent<UnityStandardAssets.Characters.FirstPerson.FirstPersonController>());
            Destroy(GetComponent<Look>());

            foreach (var c in GetComponentsInChildren<Camera>())
            {
                Destroy(c);
            }
            foreach (var al in GetComponentsInChildren<AudioListener>())
            {
                Destroy(al);
            }

            goToPos = transform.position;
            goToRot = XyRot();
        }
    }

    public void Respawn()
    {
        goToPos = transform.position;
        goToRot = XyRot();

        StartCoroutine(SendMovement());
    }

    void Update()
    {
        if (!isLocal)
        {
            float t = Time.deltaTime / updateRate;

            transform.position = Vector3.Lerp(transform.position, goToPos, t);

            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0.0f, goToRot.y, 0.0f), t);
            WeaponBehaviour.eyes.transform.localRotation = Quaternion.Lerp(WeaponBehaviour.eyes.transform.localRotation, Quaternion.Euler(goToRot.x, 0.0f, 0.0f), t);
        }
    }

    private IEnumerator SendMovement()
    {
        if ((transform.position != prevPos) || (Math.Abs(Input.GetAxis("Vertical")) > 0) || (Math.Abs(Input.GetAxis("Horizontal")) > 0) ||
            transform.rotation != prevRot)
        {
            using (RTData data = RTData.Get())
            {  
                data.SetVector3(1, transform.position);
                data.SetVector2(2, XyRot());

                GameSparksManager.Instance().GetRTSession().SendData((int)OpCodes.PlayerPosition, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
            prevPos = transform.position;
        }

        yield return new WaitForSeconds(updateRate);
        StartCoroutine(SendMovement());
    }

    public void SetPosition(Vector3 position, Vector2 eulerAngles)
    {
        goToPos = position;
        goToRot = eulerAngles;
    }

    Vector2 XyRot()
    {
        return new Vector2(WeaponBehaviour.eyes.transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y);
    }
}