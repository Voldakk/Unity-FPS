using GameSparks.RT;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour
{
    public Health Health { get; private set; }
    public PlayerShooting PlayerShooting { get; private set; }

    private new Rigidbody rigidbody;

    private Vector3 prevPos;

    public float updateRate;

    void Awake ()
    {
        Health = GetComponent<Health>();
        PlayerShooting = GetComponent<PlayerShooting>();
        rigidbody = GetComponent<Rigidbody>();
    }

    public void OnDeath()
    {
        Debug.Log("Player::OnDeath");
    }

    public void SetIsPlayer(bool value)
    {
        if(value)
        {
            transform.Find("Model").gameObject.SetActive(false);
            StartCoroutine(SendMovement());
            prevPos = transform.position;
        }
        else
        {
            GetComponent<UnityStandardAssets.Characters.FirstPerson.RigidbodyFirstPersonController>().enabled = false;
            GetComponentInChildren<UnityStandardAssets.Characters.FirstPerson.HeadBob>().enabled = false;
            GetComponentInChildren<Camera>().enabled = false;
            PlayerShooting.enabled = false;
        }
    }

    /// <summary>
    /// Sends the tank position, rotation and velocity
    /// </summary>
    /// <returns>The tank movement.</returns>
    private IEnumerator SendMovement()
    {
        // we don't want to send position updates until we are actually moving //
        // so we check that the axis-input values are greater or less than zero before sending //
        if ((transform.position != prevPos) || (Math.Abs(Input.GetAxis("Vertical")) > 0) || (Math.Abs(Input.GetAxis("Horizontal")) > 0))
        {
            using (RTData data = RTData.Get())
            {  
                // we put a using statement here so that we can dispose of the RTData objects once the packet is sent
                data.SetVector3(1, transform.position);
                data.SetVector3(2, transform.rotation.eulerAngles);

                GameSparksManager.Instance().GetRTSession().SendData(2, GameSparks.RT.GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);// send the data
            }
            prevPos = this.transform.position; // record position for any discrepancies
        }

        yield return new WaitForSeconds(updateRate);
        StartCoroutine(SendMovement());
    }

    public void SetPosition(Vector3 position, Vector3 eulerAngles)
    {
        transform.position = position;
        transform.rotation = Quaternion.Euler(eulerAngles);
    }
}
