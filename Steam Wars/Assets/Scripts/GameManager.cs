using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    List<Node> validCells;
    List<Node> debugList;
    List<Node> validStartsTeam1;
    List<Node> validStartsTeam2;

    public ParticleSystem smoke;

    public GameObject stateText;
    public GameObject number;
    public GameObject numberParent;

    public GameObject destroyedTree;
    public GameObject destroyedTrain;
    public GameObject rocket;

    public bool canSeeUnit;

    Vector3 clickedNode;

    bool zoom;

    public float delay;

    bool allValid = true;
    bool enemyTurn;

    [Space]

    GridMap grid;
    Pathfinding pathfinding;

    public Material selected;
    public Material grass;
    public Material grass2;

    public Material shot;

    public Material fogGround;
    public Material fogGround2;

    public Material fogSelected;
    
    public Material fogShot;

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
        debugList = new List<Node>();
        visibleCells = new List<Node>();
        validStartsTeam1 = new List<Node>();
        validStartsTeam2 = new List<Node>();
        zoom = false;
    }

    private void Start()
    {
        ResetMaterials();
        Numbers();
        ExtraObjects();
        RandomPositions();
    }

    private void ResetMaterials()
    {
        for (int x = 0; x < grid.gridWorldSize.x; x++)
        {
            for (int y = 0; y < grid.gridWorldSize.y; y++)
            {
                if (grid.grid[x, y] != null)
                {
                    grid.grid[x, y].unit = null;
                }

                grid.grid[x, y].valid = false;
                grid.grid[x, y].shootValid = false;
                validCells.Clear();

                if (grid.cells[x, y] != null)
                {
                    if (grid.cells[x, y].activeInHierarchy)// && !debugList.Contains(grid.cells[x, y]))
                    {
                        if(grid.grid[x, y].material == 0)
                        {
                            grid.cells[x, y].GetComponent<MeshRenderer>().material = grass;
                        }

                        if (grid.grid[x, y].material == 1)
                        {
                            grid.cells[x, y].GetComponent<MeshRenderer>().material = grass2;
                        }

                        GameObject fog = grid.cells[x, y].transform.GetChild(0).gameObject;

                        if (grid.grid[x, y].fogMaterial == 0)
                        {
                            fog.GetComponent<MeshRenderer>().material = fogGround;
                        }

                        if (grid.grid[x, y].fogMaterial == 1)
                        {
                            fog.GetComponent<MeshRenderer>().material = fogGround2;
                        }

                        if (grid.grid[x, y].unit != null)
                        {
                            if (!grid.grid[x, y].isVisible)
                            {
                                if (grid.grid[x, y].unit.teamID == 1)
                                {
                                    GameObject smoke = grid.grid[x, y].unit.transform.GetChild(1).gameObject;
                                    GameObject smoke2 = grid.grid[x, y].unit.transform.GetChild(2).gameObject;
                                    smoke.SetActive(false);
                                    smoke2.SetActive(false);
                                }
                            }
                            else
                            {
                                GameObject smoke = grid.grid[x, y].unit.transform.GetChild(1).gameObject;
                                GameObject smoke2 = grid.grid[x, y].unit.transform.GetChild(2).gameObject;
                                smoke.SetActive(false);
                                smoke2.SetActive(false);
                            }
                        }
                    }
                }
            }
        }

        foreach (Unit u in allUnits)
        {
            grid.NodeFromWorldPoint(u.transform.position).unit = u;
        }

        //visibleCells.Clear();
    }

    void Numbers()
    {
        for (int i = 0; i < grid.gridWorldSize.x; i++)
        {
            GameObject newNumber = Instantiate(number);
            newNumber.transform.SetParent(numberParent.transform);
            newNumber.GetComponent<TextMeshPro>().text = (i + 1).ToString();
            newNumber.transform.position = new Vector3(-24.5f + i, 2, -26);
        }

        for (int i = 0; i < grid.gridWorldSize.y; i++)
        {
            GameObject newNumber = Instantiate(number);
            newNumber.transform.SetParent(numberParent.transform);
            newNumber.GetComponent<TextMeshPro>().text = (i + 1).ToString();
            newNumber.transform.position = new Vector3(-26, 2, -24.5f + i);
        }
    }

    void ExtraObjects()
    {
        for (int x = 0; x < grid.gridWorldSize.x; x++)
        {
            for (int y = 0; y < grid.gridWorldSize.y; y++)
            {
                int random = Random.Range(1, 201);

                if((random == 1 || random == 2) && grid.grid[x, y].unit == null)
                {
                    grid.cells[x, y].transform.GetChild(1).gameObject.SetActive(true);
                    grid.grid[x, y].walkable = false;
                    grid.grid[x, y].extra = 1;
                }
                else if(random == 3 && grid.grid[x, y].unit == null)
                {
                    grid.cells[x, y].transform.GetChild(2).gameObject.SetActive(true);
                    grid.grid[x, y].walkable = false;
                    grid.grid[x, y].extra = 3;
                }
                else
                {
                    grid.grid[x, y].extra = 0;
                }
            }
        }
    }

    void RandomPositions()
    {
        foreach (Node node in grid.grid)
        {
            if (node.gridX > 0 && node.gridX < 50)
            {
                if (node.gridY < 23 && node.gridY > 0)
                {
                    validStartsTeam1.Add(node);
                }
                else if (node.gridY > 36 && node.gridY < 50)
                {
                    validStartsTeam2.Add(node);
                }
            }
        }

        foreach (Unit unit in allUnits)
        {
            if(unit.teamID == 0)
            {
                int random = Random.Range(0, validStartsTeam1.Count);
                unit.transform.position = new Vector3(validStartsTeam1[random].worldPosition.x, unit.transform.position.y, validStartsTeam1[random].worldPosition.z);
            }
            else
            {
                int random = Random.Range(0, validStartsTeam2.Count);
                unit.transform.position = new Vector3(validStartsTeam2[random].worldPosition.x, unit.transform.position.y, validStartsTeam2[random].worldPosition.z);
            }
        }
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
        if(MainMenu.Instance.difficulty.value == 1)
        {
            ResetFog();
        }

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
                        List<Vector3> path = pathfinding.FindPath(thisUnit.transform.position, new Vector3(x, 0, y), false, false);

                        if (path != null)
                        {
                            if (path.Count < thisUnit.viewRange)
                            {
                                Node node = grid.NodeFromWorldPoint(cell.transform.position);
                                

                                if (thisUnit.teamID == 0)
                                {
                                    GameObject fog = cell.transform.GetChild(0).gameObject;
                                    fog.SetActive(false);
                                    node.isVisible = true;
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
                    List<Vector3> path = pathfinding.FindPath(unit.transform.position, new Vector3(x, 0, y), false, true);

                    if (path != null)
                    {
                        Node node = grid.NodeFromWorldPoint(cell.transform.position);

                        if (path.Count < unit.maxMoveDistance && node.unit == null)
                        {
                            if(unit.teamID == 0)
                            {
                                cell.GetComponent<MeshRenderer>().material = selected;
                                GameObject fog = cell.transform.GetChild(0).gameObject;
                                fog.GetComponent<MeshRenderer>().material = fogSelected;

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
                    List<Vector3> path = pathfinding.FindPath(unit.transform.position, new Vector3(x, 0, y), false, false);

                    if (path != null)
                    {
                        if (path.Count < unit.maxShootRange)
                        {
                            if (unit.teamID == 0)
                            {
                                cell.GetComponent<MeshRenderer>().material = selected;
                                GameObject fog = cell.transform.GetChild(0).gameObject;
                                fog.GetComponent<MeshRenderer>().material = fogSelected;
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
    
    private void Update()
    {
        unit = TurnManager.Instance.currentUnit;

        if(unit != null)
        {
            HideSmoke();
            CheckForVisibility();

            if (unit.teamID == 0)
            {
                if(enemyTurn == true)
                {
                    enemyTurn = false;
                    stateText.GetComponent<TextMeshProUGUI>().text = "Your Turn";
                }

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
                        if(!zoom)
                        {
                            CameraController.Instance.newZoom = new Vector3(0, 12, -8);
                            zoom = true;
                        }
                    }

                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            Node clickedNode = grid.NodeFromWorldPoint(hit.point);

                            if (clickedNode.valid && !unit.hasMoved)
                            {
                                Debug.Log("s");
                                GameObject cell = grid.CellFromWorldPoint(hit.point);
                                unit.target.position = cell.transform.position;
                                grid.NodeFromWorldPoint(hit.point);
                                unit.Move();
                                zoom = false;
                            }

                            if (allValid && unit.hasMoved && !unit.hasShot && clickedNode.shootValid)
                            {
                                ResetMaterials();
                                Shoot(hit.point);

                                if(unit.unitType == 1)
                                {
                                    Invoke("AfterShot", 7f);
                                }
                                else if(unit.unitType == 2)
                                {
                                    Invoke("AfterShot", 2.5f);
                                }
                                else if (unit.unitType == 3)
                                {
                                    Invoke("AfterShot", 2.5f);
                                }
                                //AfterShot();
                            }
                        }
                    }

                    if (unit.hasMoved && !unit.hasShot)
                    {
                        Node sNode = grid.NodeFromWorldPoint(hit.point);
                        if(!zoom)
                        {
                            CameraController.Instance.followTransform = null;
                            CameraController.Instance.newPos = Vector3.zero;
                            CameraController.Instance.newZoom = new Vector3(0, 36.86602f, -24.57735f);
                            zoom = true;
                        }

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
                                        fog.GetComponent<MeshRenderer>().material = fogSelected;
                                    }
                                }
                            }

                            if (unit.unitType == 1)
                            {
                                Node node = grid.NodeFromWorldPoint(new Vector3(hit.point.x, 0, hit.point.z));
                                allValid = allValid && node.shootValid;
                            }

                            if (unit.unitType == 2)
                            {
                                for (int x = 0; x <= 1; x++)
                                {
                                    for (int y = 0; y <= 1; y++)
                                    {
                                        Node node = grid.NodeFromWorldPoint(new Vector3(hit.point.x + x, 0, hit.point.z + y));
                                        if(!node.shootValid)
                                        {
                                            allValid = false;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (unit.unitType == 3)
                            {
                                for (int x = -1; x <= 1; x++)
                                {
                                    for (int y = -1; y <= 1; y++)
                                    {
                                        Node node = grid.NodeFromWorldPoint(new Vector3(hit.point.x + x, 0, hit.point.z + y));
                                        if (!node.shootValid)
                                        {
                                            allValid = false;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (unit.unitType == 1 && allValid)
                            {
                                GameObject cell = grid.CellFromWorldPoint(new Vector3(hit.point.x, 0, hit.point.z));
                                cell.GetComponent<MeshRenderer>().material = shot;
                                GameObject fog = cell.transform.GetChild(0).gameObject;
                                fog.GetComponent<MeshRenderer>().material = fogShot;
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
                                        fog.GetComponent<MeshRenderer>().material = fogShot;
                                    }
                                }
                            }

                            if (unit.unitType == 3 && allValid)
                            {
                                for (int x = -1; x <= 1; x++)
                                {
                                    for (int y = -1; y <= 1; y++)
                                    {
                                        GameObject cell = grid.CellFromWorldPoint(new Vector3(hit.point.x + x, 0, hit.point.z + y));
                                        cell.GetComponent<MeshRenderer>().material = shot;
                                        GameObject fog = cell.transform.GetChild(0).gameObject;
                                        fog.GetComponent<MeshRenderer>().material = fogShot;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Debug.Log("Enemy Turn");
                CameraController.Instance.followTransform = null;
                CameraController.Instance.newPos = Vector3.zero;

                delay -= Time.deltaTime;

                if(!enemyTurn && delay < 0)
                {
                    enemyTurn = true;
                    stateText.GetComponent<TextMeshProUGUI>().text = "Enemy Turn";
                }

                if(!unit.isMoving && unit.isSelected && delay < 0)
                {
                    delay = 5;

                    if (!unit.hasMoved)
                    {
                        canSeeUnit = false;

                        foreach (Node node in visibleCells)
                        {
                            if (node.unit != null)
                            {
                                if (node.unit.teamID == 0)
                                {
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

                        if (unit.unitType == 1)
                        {
                            Node node = grid.NodeFromWorldPoint(new Vector3(randomPos.x, 0, randomPos.z));

                            if (node != null)
                            {
                                allValid = allValid && node.shootValid;
                            }
                        }

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
                                }
                            }
                        }

                        if (unit.unitType == 3)
                        {
                            for (int x = -1; x <= 1; x++)
                            {
                                for (int y = -1; y <= 1; y++)
                                {
                                    Node node = grid.NodeFromWorldPoint(new Vector3(randomPos.x + x, 0, randomPos.z + y));

                                    if (node != null)
                                    {
                                        allValid = allValid && node.shootValid;
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
                            Debug.Log("Attack!");
                            unit.isSelected = false;
                        }
                    }
                }
            }
        }
    }

    void Shoot(Vector3 clickedPos)
    {
        debugList.Clear();
        clickedNode = clickedPos;

        if (unit.unitType == 1)
        {
            unit.transform.LookAt(new Vector3(Random.Range(-25, 26), transform.position.y, Random.Range(-25, 26)));

            GameObject cell = grid.CellFromWorldPoint(new Vector3(clickedPos.x, 0, clickedPos.z));

            GameObject unitRocket = unit.transform.GetChild(3).gameObject;
            unitRocket.GetComponent<Rocket>().Fly(cell.transform.position);

            SoundManager.Instance.SpiderShoot();

            Invoke("RocketShoot", 2f);
        }


        if (unit.unitType == 2)
        {
            unit.transform.LookAt(new Vector3(clickedPos.x, transform.position.y, clickedPos.z));
            SoundManager.Instance.TankShoot();

            for (int x = 0; x <= 1; x++)
            {
                for (int y = 0; y <= 1; y++)
                {
                    GameObject cell = grid.CellFromWorldPoint(new Vector3(clickedPos.x + x, 0, clickedPos.z + y));
                    Node node = grid.NodeFromWorldPoint(new Vector3(clickedPos.x + x, 0, clickedPos.z + y));
                    cell.GetComponent<MeshRenderer>().material = shot;
                    GameObject fog = cell.transform.GetChild(0).gameObject;
                    fog.GetComponent<MeshRenderer>().material = fogShot;
                    if (node.unit != null && unit.IsEnemy(node.unit)) node.unit.lives -= unit.damage;
                    debugList.Add(node);
                    Instantiate(smoke, cell.transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));

                    if(node.extra == 1 || node.extra == 2)
                    {
                        node.walkable = true;
                        GameObject explosion = Instantiate(destroyedTree);
                        explosion.transform.parent = cell.transform;
                        explosion.transform.position = cell.transform.GetChild(1).position;
                        cell.transform.GetChild(1).gameObject.SetActive(false);
                    }
                    else if(node.extra == 3)
                    {
                        node.walkable = true;
                        GameObject explosion = Instantiate(destroyedTrain);
                        explosion.transform.parent = cell.transform;
                        explosion.transform.position = cell.transform.GetChild(2).position;
                        cell.transform.GetChild(2).gameObject.SetActive(false);
                    }

                    unit.transform.GetChild(4).GetComponent<ParticleSystem>().Play();
                }
            }
        }
        
        if (unit.unitType == 3)
        {
            unit.transform.LookAt(new Vector3(clickedPos.x, transform.position.y, clickedPos.z));
            SoundManager.Instance.MortarShoot();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    GameObject cell = grid.CellFromWorldPoint(new Vector3(clickedPos.x + x, 0, clickedPos.z + y));
                    Node node = grid.NodeFromWorldPoint(new Vector3(clickedPos.x + x, 0, clickedPos.z + y));
                    cell.GetComponent<MeshRenderer>().material = shot;
                    GameObject fog = cell.transform.GetChild(0).gameObject;
                    fog.GetComponent<MeshRenderer>().material = fogShot;
                    if (node.unit != null && unit.IsEnemy(node.unit)) node.unit.lives -= unit.damage;
                    debugList.Add(node);
                    Instantiate(smoke, cell.transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));

                    if (node.extra == 1 || node.extra == 2)
                    {
                        node.walkable = true;
                        GameObject explosion = Instantiate(destroyedTree);
                        explosion.transform.parent = cell.transform;
                        explosion.transform.position = cell.transform.GetChild(1).position;
                        cell.transform.GetChild(1).gameObject.SetActive(false);
                    }
                    else if (node.extra == 3)
                    {
                        node.walkable = true;
                        GameObject explosion = Instantiate(destroyedTrain);
                        explosion.transform.parent = cell.transform;
                        explosion.transform.position = cell.transform.GetChild(2).position;
                        cell.transform.GetChild(2).gameObject.SetActive(false);
                    }

                    //unit.transform.GetChild(4).GetComponent<ParticleSystem>().Play();
                }
            }
        }

        unit.hasShot = true;
        unit.isSelected = false;
        zoom = false;
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
        CameraController.Instance.newZoom = new Vector3(0, 36.86602f, -24.57735f);
        unit.isSelected = false;
    }

    void HideSmoke()
    {
        for (int x = 0; x < grid.gridWorldSize.x; x++)
        {
            for (int y = 0; y < grid.gridWorldSize.y; y++)
            {
                if (grid.cells[x, y] != null)
                {
                    if (grid.cells[x, y].activeInHierarchy)
                    {
                        if (grid.grid[x, y].unit != null)
                        {
                            if (!grid.grid[x, y].isVisible)
                            {
                                if (grid.grid[x, y].unit.teamID == 1)
                                {
                                    GameObject smoke = grid.grid[x, y].unit.transform.GetChild(1).gameObject;
                                    GameObject smoke2 = grid.grid[x, y].unit.transform.GetChild(2).gameObject;
                                    smoke.SetActive(false);
                                    smoke2.SetActive(false);
                                }
                            }
                            else
                            {
                                GameObject smoke = grid.grid[x, y].unit.transform.GetChild(1).gameObject;
                                GameObject smoke2 = grid.grid[x, y].unit.transform.GetChild(2).gameObject;
                                smoke.SetActive(true);
                                smoke2.SetActive(true);
                            }
                        }
                    }
                }
            }
        }
    }

    void RocketShoot()
    {
        GameObject cell = grid.CellFromWorldPoint(new Vector3(clickedNode.x, 0, clickedNode.z));
        Node node = grid.NodeFromWorldPoint(new Vector3(clickedNode.x, 0, clickedNode.z));

        cell.GetComponent<MeshRenderer>().material = shot;
        GameObject fog = cell.transform.GetChild(0).gameObject;
        fog.GetComponent<MeshRenderer>().material = fogShot;
        if (node.unit != null && unit.IsEnemy(node.unit)) node.unit.lives -= unit.damage;
        debugList.Add(node);
        Instantiate(smoke, cell.transform.position, Quaternion.Euler(new Vector3(-90, 0, 0)));

        if (node.extra == 1 || node.extra == 2)
        {
            node.walkable = true;
            GameObject explosion = Instantiate(destroyedTree);
            explosion.transform.parent = cell.transform;
            explosion.transform.position = cell.transform.GetChild(1).position;
            cell.transform.GetChild(1).gameObject.SetActive(false);
        }
        else if (node.extra == 3)
        {
            node.walkable = true;
            GameObject explosion = Instantiate(destroyedTrain);
            explosion.transform.parent = cell.transform;
            explosion.transform.position = cell.transform.GetChild(2).position;
            cell.transform.GetChild(2).gameObject.SetActive(false);
        }
    }

    void AfterShot()
    {
        bool missed = true;

        foreach (Node node in debugList)
        {
            if (node.unit != null)
            {
                if (node.unit.teamID == 1)
                {
                    missed = false;
                    if (node.unit.lives > 0)
                    {
                        stateText.GetComponent<TextMeshProUGUI>().text = "Hit";
                        GetComponent<SoundManager>().PlayHit();
                    }
                    else if(node.unit.lives <= 0)
                    {
                         
                        stateText.GetComponent<TextMeshProUGUI>().text = "Killed";
                        GetComponent<SoundManager>().PlayDestroyed();
                    }
                }
            }
        }

        if (missed)
        {
            stateText.GetComponent<TextMeshProUGUI>().text = "Missed";
            GetComponent<SoundManager>().PlayMissed();
        }

        unit.hasMoved = true;
        unit.hasShot = true;
        TurnManager.Instance.NextUnit();
        Debug.Log("Attack!");
        CameraController.Instance.followTransform = null;
        CameraController.Instance.newPos = Vector3.zero;
        unit.isSelected = false;
    }
}