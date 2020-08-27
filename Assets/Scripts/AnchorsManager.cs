using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARRaycastManager))]
[RequireComponent(typeof(ARAnchorManager))]
[RequireComponent(typeof(ARPointCloudManager))]
public class AnchorsManager : MonoBehaviour
{
    [SerializeField]
    private Text debugLog;

    [SerializeField]
    private Text anchorPointCount;

    [SerializeField]
    private Button toggleButton;

    [SerializeField]
    private Button clearAnchorsButton;

    private ARRaycastManager arRaycastManager;

    private ARAnchorManager arAnchorManager;

    private ARPointCloudManager arPointCloudManager;

    private List<ARAnchor> anchors = new List<ARAnchor>();

    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    // Start is called before the first frame update
    void Awake()
    {
        arRaycastManager = GetComponent<ARRaycastManager>();
        arPointCloudManager = GetComponent<ARPointCloudManager>();
        arAnchorManager = GetComponent<ARAnchorManager>();

        toggleButton.onClick.AddListener(TogglePlaneDetection);
        clearAnchorsButton.onClick.AddListener(ClearAnchors);

        debugLog.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.touchCount == 0)
            return;

        Touch touch = Input.GetTouch(0);

        if (touch.phase != TouchPhase.Began)
            return;

        if(arRaycastManager.Raycast(touch.position, hits, TrackableType.FeaturePoint))
        {
            Pose hitPose = hits[0].pose;
            ARAnchor arAnchor = arAnchorManager.AddAnchor(hitPose);

            if (arAnchor == null)
            {
                debugLog.gameObject.SetActive(true);
                string errorEntry = "There was an error creating an anchor\n";
                Debug.Log(errorEntry);
                debugLog.text += errorEntry;
            }
        }
    }

    private void TogglePlaneDetection()
    {
        arPointCloudManager.enabled = !arPointCloudManager.enabled;

        foreach(ARPointCloud pointCloud in arPointCloudManager.trackables)
        {
            pointCloud.gameObject.SetActive(arPointCloudManager.enabled);
        }

        toggleButton.GetComponentInChildren<Text>().text = arPointCloudManager.enabled ? "Disable Plane Detection" : "Enable Plane Detection";
    }

    private void ClearAnchors()
    {
        foreach (ARAnchor anchor in anchors)
        {
            arAnchorManager.RemoveAnchor(anchor);
        }
        anchors.Clear();
        anchorPointCount.text = $"Anchor Point Count: {anchors.Count}";
    }
}
