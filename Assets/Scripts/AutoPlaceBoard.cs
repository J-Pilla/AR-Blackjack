using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using System.Collections.Generic;

public class ARBoardController : MonoBehaviour
{
    public ARPlaneManager planeManager;
    public ARRaycastManager raycastManager;

    public GameObject boardPrefab;

    private GameObject spawnedBoard;
    private List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Update()
    {
        AutoPlaceBoard();
        RealignBoard();
    }

    void AutoPlaceBoard()
    {
        if (spawnedBoard != null) return;

        foreach (ARPlane plane in planeManager.trackables)
        {
            if (plane.trackingState == TrackingState.Tracking &&
                plane.alignment == PlaneAlignment.HorizontalUp)
            {
                Vector3 forward = Camera.main.transform.forward;
                forward.y = 0;
                forward.Normalize();

                Vector3 position = Camera.main.transform.position + forward * 1.2f;
                position.y = plane.transform.position.y + 0.05f;

                Quaternion rotation = Quaternion.LookRotation(forward);

                spawnedBoard = Instantiate(boardPrefab, position, rotation);
                break;
            }
        }
    }

    void RealignBoard()
    {
        if (spawnedBoard == null) return;

        Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);

        if (raycastManager.Raycast(screenCenter, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

            Vector3 forward = Camera.main.transform.forward;
            forward.y = 0;
            forward.Normalize();

            Quaternion rotation = Quaternion.LookRotation(forward);

            Vector3 position = hitPose.position;
            position.y += 0.05f;

            spawnedBoard.transform.SetPositionAndRotation(position, rotation);
        }
    }
}