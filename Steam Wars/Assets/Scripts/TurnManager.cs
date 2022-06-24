using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnManager : MonoBehaviour
{
    public List<Unit> allUnits;
    public List<Unit> team1;
    public List<Unit> team2;

    public int turn = 0;
    public float turnDelay;

    public Unit currentUnit;

    int unitID = 0;
    public int currentTeam = 1;

    public static TurnManager Instance;

    bool teamChanged;

    public GameObject finish;
    public GameObject win;
    public GameObject lose;

    bool isFinish;

    private void Start()
    {
        Time.timeScale = 1;

        Instance = this;
        currentUnit = null;
        isFinish = false;

        finish.SetActive(false);
        lose.SetActive(false);
        win.SetActive(false);

        team1 = new List<Unit>();
        team2 = new List<Unit>();

        foreach (Unit unit in allUnits)
        {
            if(unit.teamID == 0)
            {
                team1.Add(unit);
            }
            else
            {
                team2.Add(unit);
            }
        }
    }

    private void Update()
    {
        if (team1.Count == 0 && !isFinish)
        {
            Debug.Log("YOU LOST");
            //Time.timeScale = 0;
            finish.SetActive(true);
            lose.SetActive(true);
            SoundManager.Instance.PlayLose();
            isFinish = true;
        }
        else if (team2.Count == 0 && !isFinish)
        {
            Debug.Log("YOU WON");
            //Time.timeScale = 0;
            finish.SetActive(true);
            win.SetActive(true);
            SoundManager.Instance.PlayWin();
            isFinish = true;
        }

        if (currentUnit == null)
        {
            unitID = 0;
            currentTeam = 1;
            currentUnit = team1[unitID];
            currentUnit.isSelected = true;
        }

        if(TeamMoved(team1) && !teamChanged)
        {
            unitID = 0;
            currentUnit = team2[unitID];
            currentTeam = 2;
            teamChanged = true;
        }

        if(TeamMoved(team1) && TeamMoved(team2))
        {
            //Invoke("NextTurn", 2);
            NextTurn();
        }
    }

    bool TeamMoved(List<Unit> team)
    {
        bool teamMoved = true;

        foreach(Unit unit in team)
        {
            if(!unit.hasMoved || !unit.hasShot)
            {
                teamMoved = false;
                break;
            }
        }

        return teamMoved;
    }

    public void NextUnit()
    {
        unitID++;

        if(currentTeam == 1)
        {
            if(unitID >= team1.Count)
            {
                currentUnit.isSelected = false;
                currentUnit = team2[0];
                currentUnit.isSelected = true;
            }
            else
            {
                currentUnit.isSelected = false;
                currentUnit = team1[unitID];
                currentUnit.isSelected = true;
            }
        }
        else if (currentTeam == 2)
        {
            if (unitID >= team2.Count)
            {
                //Invoke("NextTurn2", 2);
                NextTurn2();
            }
            else
            {
                currentUnit.isSelected = false;
                currentUnit = team2[unitID];
                currentUnit.isSelected = true;
            }
        }
    }

    void NextTurn()
    {
        foreach (Unit unit in allUnits)
        {
            unit.hasMoved = false;
            unit.hasShot = false;
            unit.isSelected = false;
        }

        turn++;
        unitID = 0;
        currentUnit = team1[unitID];
        currentUnit.isSelected = true;
        currentTeam = 1;
        teamChanged = false;
    }

    void NextTurn2()
    {
        currentUnit.isSelected = false;
        currentUnit = team1[0];
        currentUnit.isSelected = true;
    }

    public void Rematch()
    {
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
