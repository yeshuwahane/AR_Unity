using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleARCore;

public class ScoreboardController : MonoBehaviour
{
    public Camera firstPersonCamera;
    private Anchor anchor;
    private DetectedPlane detectedPlane;
    private float yOffset;
    private int score;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = false;
        }
    }

    public void SetSelectedPlane(DetectedPlane detectedPlane)
    {
        this.detectedPlane = detectedPlane;
        CreateAnchor();
    }
    void CreateAnchor()
    {
        // Create position of anchor by raycasting a point towards
        //top of screen
        Vector2 pos = new Vector2(Screen.width * .5f, Screen.height * .90f);
        Ray ray = firstPersonCamera.ScreenPointToRay(pos);
        Vector3 anchorPosition = ray.GetPoint(5f);

        //create anchor at point
        if (anchor != null)
        {
            DestroyObject(anchor);
        }
        anchor = detectedPlane.CreateAnchor(new Pose(anchorPosition, Quaternion.identity));

        // attach scoreboard to anchor
        transform.position = anchorPosition;
        transform.SetParent(anchor.transform);

        //Record the y offset from plane
        yOffset = transform.position.y - detectedPlane.CenterPose.position.y;

        //Finally, Enabling the renderer

        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.enabled = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // tracking state must be FrameTrackingState.Tracking
        //In order to acces the frame
        if (Session.Status != SessionStatus.Tracking)
        {
            return;
        }

        //if no plane detected
        if (detectedPlane == null)
        {
            return;
        }

        // Check for the plane being subsumed.
        //if plane has been subsumed switch attachment to the subsuming plane.
        while (detectedPlane.SubsumedBy != null)
        {
            detectedPlane = detectedPlane.SubsumedBy;
        }

        //Make the score board face the viewer.
        //Transform.LookAt(firstPersonCamera.transform);

        // move position to stay consistent with the plane
        transform.position = new Vector3(transform.position.x, detectedPlane.CenterPose.position.y + yOffset, transform.position.z);

    }
}
