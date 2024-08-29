using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : ProjectBehaviour
{
    public int lvlIndex = 0;

    private void Awake()
    {
        GameManager.Walls.Add(this);
    }
}
