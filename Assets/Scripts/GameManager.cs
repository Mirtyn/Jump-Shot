using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : ProjectBehaviour
{
    public GameObject BulletPrefab;
    public GameObject BarPrefab;
    public GameObject BarNodePrefab;
    public GameObject CompleteBarPrefab;
    public GameObject CompleteBarNodePrefab;
    public Color BackgroundColor1;
    public Color BackgroundColor2;
    public Color ForegroundColor;

    private void Awake()
    {
        GameManager = this;
    }
}
