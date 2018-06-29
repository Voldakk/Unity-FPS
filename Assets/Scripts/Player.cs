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
    //public PlayerShooting PlayerShooting { get; private set; }

    private new Rigidbody rigidbody;

    private Vector3 prevPos;
    private Quaternion prevRot;

    public float updateRate;

    private Vector3 goToPos;
    private Quaternion goToRot;

    private bool isPlayer;

    public void OnDeath()
    {
        Debug.Log("Player::OnDeath");
    }

    public void Initialize(bool value)
    {
        isPlayer = value;

        Health = GetComponent<Health>();
        //PlayerShooting = GetComponent<PlayerShooting>();
        rigidbody = GetComponent<Rigidbody>();

        //PlayerShooting.Initialize(isPlayer, this);
        Health.Initialize(isPlayer);

        if (value)
        {
            transform.Find("Model").gameObject.SetActive(false);
            prevPos = transform.position;
            prevRot = transform.rotation;
            StartCoroutine(SendMovement());
        }
        else
        {
            GetComponent<UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController>().enabled = false;
            GetComponentInChildren<UnityStandardAssets.Characters.FirstPerson.HeadBob>().enabled = false;
            GetComponentInChildren<Camera>().enabled = false;
            GetComponentInChildren<AudioListener>().enabled = false;

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
        if (!isPlayer)
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
                //data.SetVector2(2, new Vector2(PlayerShooting.eyes.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y));

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
        //PlayerShooting.eyes.localRotation = Quaternion.Euler(eulerAngles.x, 0.0f, 0.0f);
    }
}
