using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitUI : MonoBehaviour
{
    public Slider slider;

    public GameObject unit;

    private void Start()
    {
        slider.maxValue = unit.GetComponent<Unit>().lives;
    }

    void Update()
    {
        transform.LookAt(2 * transform.position - Camera.main.transform.position);
        slider.value = unit.GetComponent<Unit>().lives;
    }
}
