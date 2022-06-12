using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Unit : MonoBehaviour
{

	public Transform target;

	public float speed = 1;
	public int maxMoveDistance = 5;
	public int maxShootRange = 10;

	public int teamID;

	[HideInInspector] public bool isMoving = false;

	List<Vector3> path;
	int targetIndex;

	public void Move()
	{
		path = Pathfinding.Instance.FindPath(transform.position, target.position, true);
		if (path != null && path.Count > 0)
		{
			targetIndex = 0;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	IEnumerator FollowPath()
	{
		isMoving = true;
		Vector3 currentWaypoint = path[0];
		while (true)
		{
			if (transform.position == currentWaypoint)
			{
				targetIndex++;
				if (targetIndex >= path.Count)
				{
					isMoving = false;
					GameManager.Instance.UpdateMovePositions();
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}

			transform.rotation = Quaternion.LookRotation(currentWaypoint - transform.position);
			transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
			yield return null;

		}
	}

	public bool IsEnemy(Unit unit)
	{
		if (teamID == unit.teamID)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
