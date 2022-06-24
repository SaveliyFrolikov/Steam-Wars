using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] Transform[] waypoints;

    int targetIndex = 0;

    void Start()
    {
        
    }

    public void Fly(Vector3 target)
    {
        waypoints[3].position = target;
        targetIndex = 0;
        StopCoroutine("FollowPath");
        StartCoroutine("FollowPath");
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = waypoints[0].position;
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= waypoints.Length)
                {
                    transform.position = waypoints[0].position;
                    yield break;
                }
                currentWaypoint = waypoints[targetIndex].position;
            }

            transform.LookAt(currentWaypoint);
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, 10 * Time.deltaTime);
            yield return null;
        }
    }
}
