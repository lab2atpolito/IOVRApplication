using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TasksListUI : MonoBehaviour
{
    [SerializeField] private TasksManager _taskManager;

    private List<TaskElementUI> _tasksListGUI; 

    public GameObject taskElementPrefab;
    public int _maxDescriptionLength; 

    void Start()
    {
        _tasksListGUI = new List<TaskElementUI>();
        List<ATask> _tasks = _taskManager.GetTasksList();
        int index = 1;
        foreach( ATask task in _tasks)
        {
            GameObject newTask = Instantiate(taskElementPrefab, this.transform);

            TaskElementUI taskGUI = newTask.GetComponent<TaskElementUI>();
            taskGUI.SetHeader("Task " + index);
            string description = task.GetDescription();
            if( description.Length < _maxDescriptionLength)
            {
                taskGUI.SetDescription(description);
            }
            else
            {
                taskGUI.SetDescription(description.Substring(0, _maxDescriptionLength) + "...");
            }
            _tasksListGUI.Add(taskGUI);
            index++;
        }
    }

    public void SetCurrentTaskAsCompleted(string timestamp)
    {
        int currentTask = _taskManager.GetCurrentTask();
        _tasksListGUI[currentTask].SetAsCompleated(timestamp);
    }
}
