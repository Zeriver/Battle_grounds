using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public Canvas menu;

    void Start()
    {
        menu = GetComponent<Canvas>();
        menu.enabled = false;
    }

    void Update()
    {

    }

    public void changeCanvasEnabled()
    {
        menu.enabled = !menu.enabled;
    }


}
