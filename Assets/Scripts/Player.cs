using GameSparks.RT;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Player : NetworkObject
{
    public int peerId;

    public Health Health { get; private set; }
    public WeaponBehaviour WeaponBehaviour { get; private set; }

    private new Rigidbody rigidbody;

    private Vector3 prevPos;
    private Vector2 prevRot;

    public float updateRate;

    private Vector3 goToPos;
    private Vector2 goToRot;

    private bool isLocal;

    public void OnDeath()
    {
        Debug.Log("Player::OnDeath");
    }

    void Start()
    {
        peerId = owner;
        GameManager.Instance().playerList[peerId-1] = this;
        Initialize(peerId == GameSparksManager.PeerId());
    }

    public void Initialize(bool _isLocal)
    {
        isLocal = _isLocal;

        rigidbody = GetComponent<Rigidbody>();
        WeaponBehaviour = GetComponent<WeaponBehaviour>();
        WeaponBehaviour.Initialize(this, isLocal);

        Health = GetComponent<Health>();
        Health.Initialize(isLocal);

        if (isLocal)
        {
            transform.Find("Model").gameObject.SetActive(false);
            prevPos = transform.position;
            prevRot = XyRot();
            StartCoroutine(SendMovement());

            WeaponBehaviour.SetWeapon(PlayerSetting.Current.startingWeapon);
            GameManager.Instance().SetPlayerWeapon(PlayerSetting.Current.startingWeapon);
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
        if (transform.position != prevPos || 
            Math.Abs(Input.GetAxis("Vertical")) > 0 || 
            Math.Abs(Input.GetAxis("Horizontal")) > 0 ||
            XyRot() != prevRot )
        {
            using (RTData data = RTData.Get())
            {  
                data.SetVector3(1, transform.position);
                data.SetVector2(2, XyRot());

                GameSparksManager.Instance().SendRTData(OpCodes.PlayerPosition, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
            prevPos = transform.position;
            prevRot = XyRot();
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