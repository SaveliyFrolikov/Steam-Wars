using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GridMap grid;
    Pathfinding pathfinding;

    public Material selected;
    public Material ground;

    public Unit unit;

    public Unit[] units;

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
        grid = GetComponent<GridMap>();
        pathfinding = GetComponent<Pathfinding>();
    }

    private void Start()
    {
        foreach (Unit u in units)
        {
            grid.NodeFromWorldPoint(u.transform.position).unit = u;
        }

        grid.NodeFromWorldPoint(unit.transform.position).unit = unit;

        UpdateMovePositions();
    }

    public void UpdateMovePositions()
    {
        Stopwatch ms = new Stopwatch();
        ms.Start();

        for (int x = 0; x < grid.gridWorldSize.x; x++)
        {
            for (int y = 0; y < grid.gridWorldSize.y; y++)
            {
                grid.grid[x, y].valid = false;

                if (grid.cells[x, y] != null)
                {
                    if (grid.cells[x, y].activeInHierarchy)
                    {
                        grid.cells[x, y].GetComponent<MeshRenderer>().material = ground;
                    }
                }
            }


        }
        grid.GetXY(unit.transform.position, out int unitX, out int unitY);



        for (int x = unitX - unit.maxMoveDistance; x <= unit.maxMoveDistance + unitX + 1; x++)
        {
            for (int y = unitY - unit.maxMoveDistance; y <= unit.maxMoveDistance + unitY + 1; y++)
            {
                GameObject cell = grid.CellFromWorldPoint(new Vector3(x, 0, y));

                if (cell != null)
                {
                    List<Vector3> path = pathfinding.FindPath(unit.transform.position, new Vector3(x, 0, y), false);


                    if (path != null)
                    {
                        if (path.Count < unit.maxMoveDistance)
                        {
                            cell.GetComponent<MeshRenderer>().material = selected;
                            Node node = grid.NodeFromWorldPoint(cell.transform.position);
                            node.valid = true;
                        }
                    }
                }
            }
        }

        ms.Stop();
        UnityEngine.Debug.Log(ms.ElapsedMilliseconds + " ms");
    }

    private void Update()
    {
        if (!unit.isMoving)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Node clickedNode = grid.NodeFromWorldPoint(hit.point);

                    if (clickedNode.unit != null)
                    {
                        if (unit.IsEnemy(clickedNode.unit) && unit != clickedNode.unit)
                        {
                            UnityEngine.Debug.Log("Attack!");
                        }
                    }

                    if (grid.NodeFromWorldPoint(hit.point).valid)
                    {
                        GameObject cell = grid.CellFromWorldPoint(hit.point);
                        unit.target.position = cell.transform.position;
                        grid.NodeFromWorldPoint(hit.point);
                        unit.Move();
                    }
                }
            }
        }
    }
}

