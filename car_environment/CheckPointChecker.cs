using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointChecker : MonoBehaviour
{
    static List<Transform> checkpoints;
    public void Awake()
    {
        InitializeCheckpoints();
    }

    public static void InitializeCheckpoints()
    {
        checkpoints = new List<Transform>();
        Transform parent = GameObject.Find("CheckPoints").transform;
        if (parent != null)
        {
            foreach (Transform child in parent)
            {
                if(child.tag == "CheckPoint")
                checkpoints.Add(child);
            }
            setCheckpointsBack();
        }
        else Debug.Log("The parent does not exist");
    }
    public static Transform getCheckPointFromIndex(int index)
    {
        return checkpoints[index];
    }
    public static int getMaxCheckPoints()
    {
        return checkpoints.Count;
    }

    public static void setCheckpointsBack()
    {
        foreach(Transform child in checkpoints) {
            child.gameObject.tag = "CheckPoint";
        }
    }
}
