using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using System;
using LitJson;
using System.IO;

public class PointCloudParser : MonoBehaviour
{
    public ARPointCloudManager pointCloudManager;
    // Start is called before the first frame update

    private string savePath;

    private void OnEnable() {
        savePath = Application.persistentDataPath + "/tags.json";
        pointCloudManager.pointCloudsChanged += PointCloudManager_pointCloudsChanged;
    }

    private void PointCloudManager_pointCloudsChanged(ARPointCloudChangedEventArgs obj) {
        List<ARPoint> addedPoints = new List<ARPoint>();
        foreach(var pointCloud in obj.added)
        {
            foreach(var pos in pointCloud.positions)
            {
                ARPoint newPoint = new ARPoint(pos);
                addedPoints.Add(newPoint);
            }
        }
        savePoints(addedPoints);

        List<ARPoint> updatedPoints = new List<ARPoint>();
        foreach(var pointCloud in obj.updated)
        {
            foreach(var pos in pointCloud.positions)
            {
                ARPoint newPoint = new ARPoint(pos);
                updatedPoints.Add(newPoint);
            }
        }
        savePoints(updatedPoints);
    }

    private void savePoints(List<ARPoint> points) {
        string jsonData = JsonMapper.ToJson(points);
        File.WriteAllText(savePath, jsonData);
    }
}

[Serializable]
public class ARPoint 
{
    public float x;
    public float y;
    public float z;

    public ARPoint(Vector3 pos)
    {
        x = pos.x;
        y = pos.y;
        z = pos.z;
    }
}
