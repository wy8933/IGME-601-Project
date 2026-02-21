using UnityEngine;
using UnityEngine.AI;

public class TestEnemyAI : MonoBehaviour
{
    public float attackDistance = 2.5f;

    private NavMeshAgent m_Agent;
    private Transform[] Waypoints;
    private int index = 0;
    private float m_Distance;

    void Start()
    {
        Waypoints = GameObject.Find("Waypoints").GetComponentsInChildren<Transform>();
        m_Agent = GetComponent<NavMeshAgent>();
        m_Agent.destination = Waypoints[index].position;
    }

    void Update()
    {
        m_Distance = Vector3.Distance(m_Agent.transform.position, Waypoints[index].position);

        if (m_Distance < attackDistance)
        {
            m_Agent.isStopped = true;
            index = (index + 1) % Waypoints.Length;
            m_Agent.destination = Waypoints[index].position;
        }
        else
        {
            m_Agent.isStopped = false;
        }
    }
}
