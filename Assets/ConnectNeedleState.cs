using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectNeedleState : ATask
{
    [SerializeField] private PowerDrill _drill;

    private void Start()
    {
        _description = "Connect needle set to driver.";
    }

    public override void OnEntry(TasksManager controller)
    {
        controller.DisableButton();
    }

    public override void OnExit(TasksManager controller)
    {
        
    }

    public override void OnUpdate(TasksManager controller)
    {
        if(!_drill.IsEmpty())
        {
            controller.EnableButton();
        }
    }
}
