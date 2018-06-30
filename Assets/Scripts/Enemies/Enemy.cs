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

    private Vector3 goToPos;
    private Vector2 goToRot;

    private new Rigidbody rigidbody;
    private Health health;

    private bool isHost;

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

        isHost = GameManager.Instance().IsHost;

        if (isHost)
            StartCoroutine(SendMovement());
    }

    void Update()
    {
        if (!isHost)
        {
            float t = Time.deltaTime / updateRate;

            transform.position = Vector3.Lerp(transform.position, goToPos, t);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(goToRot.x, goToRot.y, 0.0f), t);
        }
    }

    public void UpdateTransform(Vector3 position, Vector2 rotation)
    {
        goToPos = position;
        goToRot = rotation;
    }

    private IEnumerator SendMovement()
    {
        // If we are moving
        if ((transform.position != prevPos) || transform.rotation != prevRot)
        {
            GameManager.Instance().SendNpcPosition(this);
            prevPos = transform.position;
            prevRot = transform.rotation;
        }

        yield return new WaitForSeconds(updateRate);
        StartCoroutine(SendMovement());
    }
}