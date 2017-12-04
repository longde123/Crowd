using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    #region Game Events
    public event Action<Vector3> OnPlayerMark = delegate { };
    #endregion

    #region Singleton
    public static GameManager Instance { get { return _instance; } }
    private static GameManager _instance;
    #endregion

    void Awake()
    {
        _instance = this;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                OnPlayerMark(hit.point);
            }
        }
    }
}
