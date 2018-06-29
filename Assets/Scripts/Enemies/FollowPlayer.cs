using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(SphereCollider))]
public class FollowPlayer : MonoBehaviour
{
    public float viewDistance = 20.0f;

    private NavMeshAgent agent;
    private SphereCollider sphereCollider;

    private List<Player> nearbyPlayers;

    private Player target;

	void Awake ()
    {
        nearbyPlayers = new List<Player>();

        agent = GetComponent<NavMeshAgent>();
        sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.isTrigger = true;
        sphereCollider.radius = viewDistance;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (!GameManager.Instance().IsHost)
            return;

        if (nearbyPlayers.Count > 0)
            SetTarget(nearbyPlayers[0]);
        else
            SetTarget(null);
    }

    void OnTriggerEnter(Collider other)
    {
        Player p = other.GetComponent<Player>();
        if (p != null)
        {
            nearbyPlayers.Add(p);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Player p = other.GetComponent<Player>();
        if (p != null)
        {
            nearbyPlayers.Remove(p);
        }
    }

    void SetTarget(Player player)
    {
        target = player;

        if (target == null)
        {
            agent.isStopped = true;
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(target.transform.position);
        }
    }

    void OnDeath()
    {
        Destroy(gameObject);
    }
}
