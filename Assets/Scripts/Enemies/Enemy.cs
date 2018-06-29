using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class Enemy : MonoBehaviour
{
    public string id;
    public float updateRate = 0.1f;

    private Vector3 prevPos;
    private Quaternion prevRot;

    private new Rigidbody rigidbody;
    private Health health;

    void Reset()
    {
        id = Guid.NewGuid().ToString("N");
    }

    void OnValidate()
    {
        id = Guid.NewGuid().ToString("N");
    }

	void Awake ()
    {
        rigidbody = GetComponent<Rigidbody>();

        health = GetComponent<Health>();
        health.Initialize(false);
    }

    void Start()
    {
        GameManager.Instance().RegisterEnemy(this);

        if(GameManager.Instance().IsHost)
        {
            StartCoroutine(SendMovement());
        }
    }

    public void UpdateTransform(Vector3 position, Vector2 rotation)
    {
        transform.position = position;
        transform.rotation = Quaternion.Euler(rotation.x, rotation.y, 0f);
    }

    private IEnumerator SendMovement()
    {
        // If we are moving
        if ((transform.position != prevPos) || transform.rotation != prevRot)
        {
            GameManager.Instance().SendNpcPosition(this);
            prevPos = transform.position;
        }

        yield return new WaitForSeconds(updateRate);
        StartCoroutine(SendMovement());
    }
}