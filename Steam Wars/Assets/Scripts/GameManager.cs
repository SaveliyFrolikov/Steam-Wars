using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    List<Node> validCells;
    List<GameObject> debugList;

    

    public bool canSeeUnit;

    public float delay;

    bool allValid = true;

    [Space]

    GridMap grid;
    Pathfinding pathfinding;

    public Material selected;
    public Material ground;
    public Material shot;

    [Space]

    public Unit unit;
    public List<Unit> allUnits;
    [SerializeField] List<Node> visibleCells;

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
        grid = GetComponent<GridMap>();
        pathfinding = GetComponent<Pathfinding>();
        validCells = new List<Node>();
        debugList = new List<GameObject>();
        visibleCells = new List<Node>();
    }

    

    private void Start()
    {

        ResetMaterials();
    }

    private void ResetMaterials()
    {
        for (int x = 0; x < grid.gridWorldSize.x; x++)
        {
            for (int y = 0; y < grid.gridWorldSize.y; y++)
            {
                grid.grid[x, y].valid = false;
                grid.grid[x, y].shootValid = false;
                validCells.Clear();

                if (grid.cells[x, y] != null)
                {
                    if (grid.cells[x, y].activeInHierarchy && !debugList.Contains(grid.cells[x, y]))
                    {
                        grid.cells[x, y].GetComponent<MeshRenderer>().material = ground;
                        GameObject fog = grid.cells[x, y].transform.GetChild(0).gameObject;
                        fog.GetComponent<MeshRenderer>().material = ground;
                    }
                }

                if (grid.grid[x, y] != null)
                {
                    grid.grid[x, y].unit = null;
                }
            }
        }

        foreach (Unit u in allUnits)
        {
            grid.NodeFromWorldPoint(u.transform.position).unit = u;
        }

        //visibleCells.Clear();
    }

    public void ResetFog()
    {
        for (int x = 0; x < grid.gridWorldSize.x; x++)
        {
            for (int y = 0; y < grid.gridWorldSize.y; y++)
            {
                if (grid.cells[x, y] != null)
                {
                    if (grid.cells[x, y].activeInHierarchy)
                    {
                        GameObject fog = grid.cells[x, y].transform.GetChild(0).gameObject;
                        fog.SetActive(true);
                        
                        Node node = grid.NodeFromWorldPoint(grid.cells[x, y].transform.position);
                        node.isVisible = false;
                    }
                }
            }
        }
    }

    public void CheckForVisibility()
    {
        //ResetFog();

        visibleCells.Clear();

        foreach (Unit thisUnit in TurnManager.Instance.allUnits)
        {
            grid.GetXY(thisUnit.transform.position, out int unitX, out int unitY);

            for (int x = unitX - thisUnit.viewRange; x <= thisUnit.viewRange + unitX + 1; x++)
            {
                for (int y = unitY - thisUnit.viewRange; y <= thisUnit.viewRange + unitY + 1; y++)
                {
                    GameObject cell = grid.CellFromWorldPoint(new Vector3(x, 0, y));

                    if (cell != null)
                    {
                        List<Vector3> path = pathfinding.FindPath(thisUnit.transform.position, new Vector3(x, 0, y), false);

                        if (path != null)
                        {
                            if (path.Count < thisUnit.viewRange)
                            {
                                Node node = grid.NodeFromWorldPoint(cell.transform.position);
                                node.isVisible = true;

                                if (thisUnit.teamID == 0)
                                {
                                    GameObject fog = cell.transform.GetChild(0).gameObject;
                                    fog.SetActive(false);
                                }
                                else
                                {
                                    visibleCells.Add(node);
                                }
                            }
                        }
                    }

                }
            }
        }
    }

    public void UpdateMovePositions()
    {
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
                        Node node = grid.NodeFromWorldPoint(cell.transform.position);

                        if (path.Count < unit.maxMoveDistance && node.unit == null)
                        {
                            if(unit.teamID == 0)
                            {
                                cell.GetComponent<MeshRenderer>().material = selected;
                                GameObject fog = cell.transform.GetChild(0).gameObject;
                                fog.GetComponent<MeshRenderer>().material = selected;

                            }
                            node.valid = true;
                            validCells.Add(node);
                        }
                    }
                }
            }
        }

        
    }

    public void UpdateShootPositions()
    {
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
                            if (unit.teamID == 0)
                            {
                                cell.GetComponent<MeshRenderer>().material = selected;
                                GameObject fog = cell.transform.GetChild(0).gameObject;
                                fog.GetComponent<MeshRenderer>().material = selected;
                            }

                            Node node = grid.NodeFromWorldPoint(cell.transform.position);
                            node.shootValid = true;
                            validCells.Add(node);
                        }
                    }
                }
            }
        }

        
    }

    private void LateUpdate()
    {
        if(unit != null)
        {
            
        }
    }

    private void Update()
    {
        unit = TurnManager.Instance.currentUnit;

        if(unit != null)
        {
            CheckForVisibility();

            if (unit.teamID == 0)
            {
                if (!unit.isMoving && unit.isSelected)
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Input.GetKeyDown(KeyCode.N))
                    {
                        Skip();
                    }

                    if (!unit.hasMoved)
                    {
                        UpdateMovePositions();
                        delay -= Time.deltaTime;
                    }

                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (Input.GetMouseButtonDown(0))// && delay < 0)
                        {
                            Node clickedNode = grid.NodeFromWorldPoint(hit.point);

                            if (clickedNode.valid && !unit.hasMoved)
                            {
                                GameObject cell = grid.CellFromWorldPoint(hit.point);
                                unit.target.position = cell.transform.position;
                                grid.NodeFromWorldPoint(hit.point);
                                unit.Move();
                            }

                            if (allValid && unit.hasMoved && !unit.hasShot)
                            {
                                ResetMaterials();
                                Shoot(hit.point);
                                unit.hasMoved = true;
                                unit.hasShot = true;
                                TurnManager.Instance.NextUnit();
                                UnityEngine.Debug.Log("Attack!");
                                CameraController.Instance.followTransform = null;
                                CameraController.Instance.newPos = Vector3.zero;
                                unit.isSelected = false;
                            }
                        }
                    }

                    if (unit.hasMoved && !unit.hasShot)
                    {
                        Node sNode = grid.NodeFromWorldPoint(hit.point);

                        allValid = true;

                        if (sNode.shootValid)
                        {
                            for (int x = 0; x < grid.gridWorldSize.x; x++)
                            {
                                for (int y = 0; y < grid.gridWorldSize.y; y++)
                                {
                                    if (grid.grid[x, y].shootValid)
                                    {
                                        grid.cells[x, y].GetComponent<MeshRenderer>().material = selected;
                                        GameObject fog = grid.cells[x, y].transform.GetChild(0).gameObject;
                                        fog.GetComponent<MeshRenderer>().material = selected;
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
                                        GameObject fog = cell.transform.GetChild(0).gameObject;
                                        fog.GetComponent<MeshRenderer>().material = shot;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                UnityEngine.Debug.Log("Enemy Turn");
                CameraController.Instance.followTransform = null;
                CameraController.Instance.newPos = Vector3.zero;

                if(!unit.isMoving && unit.isSelected)
                {
                    Vector3 unitPos;

                    if (!unit.hasMoved)
                    {
                        foreach (Node node in visibleCells)
                        {
                            if (node.unit != null)
                            {
                                if (node.unit.teamID == 0)
                                {
                                    unitPos = node.worldPosition;
                                    canSeeUnit = true;
                                    break;
                                }
                            }
                        }

                        if (canSeeUnit)
                        {
                            unit.hasMoved = true;
                        }
                        else
                        {
                            UpdateMovePositions();

                            Node randomNode = validCells[Random.Range(0, validCells.Count + 1)];
                            Vector3 randomPos = randomNode.worldPosition;

                            GameObject cell = grid.CellFromWorldPoint(randomPos);
                            unit.target.position = cell.transform.position;
                            grid.NodeFromWorldPoint(randomPos);
                            unit.Move();
                        }
                    }

                    if(unit.hasMoved && !unit.hasShot)
                    {
                        UpdateShootPositions();

                        Node randomNode = validCells[Random.Range(0, validCells.Count + 1)];
                        Vector3 randomPos = randomNode.worldPosition;

                        canSeeUnit = false;

                        foreach (Node node in visibleCells)
                        {
                            if (node.unit != null)
                            {
                                if (node.unit.teamID == 0)
                                {
                                    randomPos = node.worldPosition;
                                    canSeeUnit = true;
                                    break;
                                }
                            }
                        }

                        allValid = true;

                        if (unit.unitType == 2)
                        {
                            for (int x = 0; x <= 1; x++)
                            {
                                for (int y = 0; y <= 1; y++)
                                {
                                    Node node = grid.NodeFromWorldPoint(new Vector3(randomPos.x + x, 0, randomPos.z + y));

                                    if (node != null)
                                    {
                                        allValid = allValid && node.shootValid;
                                    }
                                    else if (node == null && canSeeUnit)
                                    {
                                        randomPos = new Vector3(randomPos.x - 1, 0, randomPos.z - 1);
                                        x = 0;
                                        y = 0;
                                    }
                                }
                            }
                        }

                        if (allValid)
                        {
                            Shoot(randomPos);
                            unit.hasMoved = true;
                            unit.hasShot = true;
                            TurnManager.Instance.NextUnit();
                            UnityEngine.Debug.Log("Attack!");
                            unit.isSelected = false;
                        }
                    }
                }
            }
        }
    }

    void Shoot(Vector3 clickedPos)
    {
        unit.transform.LookAt(new Vector3(clickedPos.x, transform.position.y, clickedPos.z));

        if (unit.unitType == 2)
        {
            for (int x = 0; x <= 1; x++)
            {
                for (int y = 0; y <= 1; y++)
                {
                    GameObject cell = grid.CellFromWorldPoint(new Vector3(clickedPos.x + x, 0, clickedPos.z + y));
                    Node node = grid.NodeFromWorldPoint(new Vector3(clickedPos.x + x, 0, clickedPos.z + y));
                    cell.GetComponent<MeshRenderer>().material = shot;
                    GameObject fog = grid.cells[x, y].transform.GetChild(0).gameObject;
                    fog.GetComponent<MeshRenderer>().material = shot;
                    if (node.unit != null && unit.IsEnemy(node.unit)) node.unit.lives -= 1;
                    debugList.Add(cell);
                }
            }
        }

        unit.hasShot = true;
        unit.isSelected = false;
        ResetMaterials();
    }

    public void Skip()
    {
        ResetMaterials();
        unit.hasMoved = true;
        unit.hasShot = true;
        TurnManager.Instance.NextUnit();
        CameraController.Instance.followTransform = null;
        CameraController.Instance.newPos = Vector3.zero;
        unit.isSelected = false;
    }
}

