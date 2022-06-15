using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    List<Unit> allUnits;
    List<Unit> team1;
    List<Unit> team2;

    private void Start()
    {
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

}
