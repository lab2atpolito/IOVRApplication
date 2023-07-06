using System.Collections;
using System.Collections.Generic;
using Oculus;
using Oculus.Interaction.Grab;
using Oculus.Interaction.HandGrab;
using UnityEngine;

public class DrillCase : MonoBehaviour
{
    [SerializeField]
    private Transform _topBone;
    [SerializeField]
    private Transform _bottomBone;
    [SerializeField]
    private float _openingAngle = 50f;

    [SerializeField] private TasksManager _controller;

    [SerializeField] private DrillCaseState _state;
    [SerializeField] private GameObject[] _handsGrab;

    private void Start()
    {
        _state = DrillCaseState.CLOSED;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 topBoneDir = _topBone.up;
        Debug.DrawLine(_topBone.position, topBoneDir * 2f, Color.red);

        Vector3 bottomBoneDir = _bottomBone.up;
        Debug.DrawLine(_bottomBone.position, bottomBoneDir * 2f, Color.red);

        float angle = Vector3.Angle(bottomBoneDir, topBoneDir);
        //Debug.Log(angle);

        if(angle > _openingAngle && _state == DrillCaseState.CLOSED)
        {
            _state = DrillCaseState.OPENED;
            if (!_controller.IsGuideActive())
            {
                foreach(GameObject intercatable in _handsGrab)
                {
                    intercatable.SetActive(true);
                }
            }
            Debug.Log("Drill Case OPENED!");
        }
        else if( angle < _openingAngle && _state == DrillCaseState.OPENED)
        {
            _state = DrillCaseState.CLOSED;
            if (!_controller.IsGuideActive())
            {
                foreach (GameObject intercatable in _handsGrab)
                {
                    intercatable.SetActive(false);
                }
            }
            Debug.Log("Drill Case CLOSED!");
        }
    }

    public bool IsOpened()
    {
        return _state == DrillCaseState.OPENED;
    }
}

public enum DrillCaseState
{
    CLOSED,
    OPENED
}
