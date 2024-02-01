using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Rand_move : MonoBehaviour
{
    private NavMeshAgent agent;
    private Vector3 anchor;
    public Vector2 rand_range = new Vector2(-3, 3);
    public float max_timer;
    private float timer;
    public bool online = false;

    private void Awake()
    {
        anchor = transform.position;
        agent = GetComponent<NavMeshAgent>();
        timer = max_timer;
    }

    private void FixedUpdate()
    {
        if(online)
        {
            if(timer <= 0)
            {
                Vector3 destination = anchor + new Vector3(Random.Range(rand_range.x, rand_range.y), 0, Random.Range(rand_range.x, rand_range.y));
                agent.SetDestination(destination);
                timer = max_timer;
                // Big roaming
                // /*anchor = transform.position;*/
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
    }

    public NavMeshAgent GetAgent()
    {
        return agent;
    }

}
