using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int turnID;

    public float delay;

    GridMap grid;
    Pathfinding pathfinding;

    bool unitSelected;

    public Material selected;
    public Material ground;
    public Material shot;

    public Unit unit;

    public Unit[] units;

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
        grid = GetComponent<GridMap>();
        pathfinding = GetComponent<Pathfinding>();
        turnID = 0;
    }

    private void ResetMaterials()
    {
        for (int x = 0; x < grid.gridWorldSize.x; x++)
        {
            for (int y = 0; y < grid.gridWorldSize.y; y++)
            {
                grid.grid[x, y].valid = false;
                grid.grid[x, y].shootValid = false;

                if (grid.cells[x, y] != null)
                {
                    if (grid.cells[x, y].activeInHierarchy)
                    {
                        grid.cells[x, y].GetComponent<MeshRenderer>().material = ground;
                    }
                }
            }
        }
    }

    private void Start()
    {
        foreach (Unit u in units)
        {
            grid.NodeFromWorldPoint(u.transform.position).unit = u;
        }

        grid.NodeFromWorldPoint(unit.transform.position).unit = unit;

        unitSelected = false;
       // UpdateMovePositions();
    }

    public void UpdateMovePositions()
    {
        Stopwatch ms = new Stopwatch();
        ms.Start();

        ResetMaterials();
        
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

        unitSelected = true;
        ms.Stop();
        UnityEngine.Debug.Log(ms.ElapsedMilliseconds + " ms");
    }

    public void UpdateShootPositions()
    {
        Stopwatch ms = new Stopwatch();
        ms.Start();

        ResetMaterials();

        grid.GetXY(unit.transform.position, out int unitX, out int unitY);

        for (int x = unitX - unit.maxShootRange; x <= unit.maxShootRange + unitX + 1; x++)
        {
            for (int y = unitY - unit.maxShootRange; y <= unit.maxShootRange + unitY + 1; y++)
            {
                GameObject cell = grid.CellFromWorldPoint(new Vector3(x, 0, y));

                if (cell != null)
                {
                    List<Vector3> path = pathfinding.FindPath(unit.transform.position, new Vector3(x, 0, y), false);


                    if (path != null)
                    {
                        if (path.Count < unit.maxShootRange)
                        {
                            cell.GetComponent<MeshRenderer>().material = selected;
                            Node node = grid.NodeFromWorldPoint(cell.transform.position);
                            node.shootValid = true;
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

        if (!unit.isMoving && unit.isSelected)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (!unit.hasMoved)
            {
                UpdateMovePositions();
                delay -= Time.deltaTime;
            }

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (Input.GetMouseButtonDown(0) && delay < 0)
                {
                    Node clickedNode = grid.NodeFromWorldPoint(hit.point);

                    if (clickedNode.valid && !unit.hasMoved)
                    {
                        GameObject cell = grid.CellFromWorldPoint(hit.point);
                        unit.target.position = cell.transform.position;
                        grid.NodeFromWorldPoint(hit.point);
                        unit.Move();
                    }

                    if (clickedNode.shootValid && unit.hasMoved && !unit.hasShot)
                    {
                        ResetMaterials();
                        Shoot(hit.point, unit);
                        //unit.hasMoved = false;
                        unit.hasShot = true;
                        UnityEngine.Debug.Log("Attack!");
                        CameraController.Instance.followTransform = null;
                        CameraController.Instance.newPos = Vector3.zero;
                    }
                }
            }

            if(unit.hasMoved && !unit.hasShot)
            {
                Node sNode = grid.NodeFromWorldPoint(hit.point);

                bool allValid = true;

                if(sNode.shootValid)
                {
                    for (int x = 0; x < grid.gridWorldSize.x; x++)
                    {
                        for (int y = 0; y < grid.gridWorldSize.y; y++)
                        {
                            if (grid.grid[x, y].shootValid)
                            {
                                grid.cells[x, y].GetComponent<MeshRenderer>().material = selected;
                            }
                        }
                    }

                    for (int x = 0; x <= 1; x++)
                    {
                        for (int y = 0; y <= 1; y++)
                        {
                            Node node = grid.NodeFromWorldPoint(new Vector3(hit.point.x + x, 0, hit.point.z + y));
                            allValid = allValid && node.shootValid;
                        }
                    }

                    if (unit.unitType == 2 && allValid)
                    {
                        for (int x = 0; x <= 1; x++)
                        {
                            for (int y = 0; y <= 1; y++)
                            {
                                GameObject cell = grid.CellFromWorldPoint(new Vector3(hit.point.x + x, 0, hit.point.z + y));
                                cell.GetComponent<MeshRenderer>().material = shot;
                            }
                        }
                    }
                }
            }

           

        }
    }

    void Shoot(Vector3 clickedPos, Unit unitThatShot)
    {
        if (unitThatShot.unitType == 2)
        {
            for (int x = 0; x <= 1; x++)
            {
                for (int y = 0; y <= 1; y++)
                {
                    GameObject cell = grid.CellFromWorldPoint(new Vector3(clickedPos.x + x, 0, clickedPos.z + y));
                    Node node = grid.NodeFromWorldPoint(new Vector3(clickedPos.x + x, 0, clickedPos.z + y));
                    cell.GetComponent<MeshRenderer>().material = shot;
                    if(node.unit != null) node.unit.lives -= 1;
                }
            }
        }

        unit.hasShot = true;
        unit.isSelected = false;
        ResetMaterials();
    }
}

