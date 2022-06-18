using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Unit : MonoBehaviour
{
	public Transform target;

	[Space]

	public bool hasMoved;
	public bool hasShot;
	public bool isSelected;

	[Space]

	public int lives;
	public float speed = 1;
	public int maxMoveDistance = 5;
	public int maxShootRange = 10;

	[Space]

	public int unitType;
	public int teamID;

	Animator animator;

	[HideInInspector] public bool isMoving = false;

	List<Vector3> path;
	int targetIndex;


    public void Start()
    {
		animator = GetComponent<Animator>();
		hasMoved = false;
		hasShot = false;
		isSelected = false;
    }

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

    private void Update()
    {
		animator.SetBool("isMoving", isMoving);

		if(Input.GetKeyDown(KeyCode.Escape))
        {
			isSelected = false;
        }

		if (isSelected && teamID == 0)
		{
			CameraController.Instance.followTransform = transform;
		}
		
	}

    IEnumerator FollowPath()
	{
		isMoving = true;
		Vector3 currentWaypoint = path[0];
		while (true)
		{
			if (transform.position.x == currentWaypoint.x && transform.position.z == currentWaypoint.z)
			{
				targetIndex++;
				if (targetIndex >= path.Count)
				{
					isMoving = false;
					GameManager.Instance.UpdateShootPositions();
					hasMoved = true;
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}

			transform.LookAt(new Vector3(currentWaypoint.x, transform.position.y, currentWaypoint.z));
			transform.position = Vector3.MoveTowards(transform.position, new Vector3(currentWaypoint.x, transform.position.y, currentWaypoint.z), speed * Time.deltaTime);
			yield return null;

		}
	}

	public bool IsEnemy(Unit unit)
	{
		if (teamID == unit.teamID)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

    private void OnMouseDown()
    {
		isSelected = true;
    }
}
