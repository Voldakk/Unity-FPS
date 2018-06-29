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
    private Quaternion goToRot;

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
            Destroy(GetComponentInChildren<Camera>());
            Destroy(GetComponentInChildren<AudioListener>());

            goToPos = transform.position;
            goToRot = transform.rotation;
        }
    }

    public void Respawn()
    {
        StartCoroutine(SendMovement());
    }

    void Update()
    {
        if (!isLocal)
        {
            //transform.position = Vector2.Lerp(transform.position, goToPos, Time.deltaTime / updateRate);
        }
    }

    private IEnumerator SendMovement()
    {
        // If we are moving
        if ((transform.position != prevPos) || (Math.Abs(Input.GetAxis("Vertical")) > 0) || (Math.Abs(Input.GetAxis("Horizontal")) > 0) ||
            transform.rotation != prevRot)
        {
            using (RTData data = RTData.Get())
            {  
                data.SetVector3(1, transform.position);
                data.SetVector2(2, new Vector2(WeaponBehaviour.eyes.transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y));

                GameSparksManager.Instance().GetRTSession().SendData(2, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
            }
            prevPos = transform.position;
        }

        yield return new WaitForSeconds(updateRate);
        StartCoroutine(SendMovement());
    }

    public void SetPosition(Vector3 position, Vector2 eulerAngles)
    {
        transform.position = position;
        transform.localRotation = Quaternion.Euler(0.0f, eulerAngles.y, 0.0f);
        WeaponBehaviour.eyes.transform.localRotation = Quaternion.Euler(eulerAngles.x, 0.0f, 0.0f);
    }
}
