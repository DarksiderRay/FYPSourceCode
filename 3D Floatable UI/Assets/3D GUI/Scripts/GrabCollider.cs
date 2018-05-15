using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class GrabCollider : MonoBehaviour {

    public enum Version { AllThreeAxesAutoTilt, TwoAxesAutoTiltOneAxisManualTilt, AllThreeAxesManualTilt }

    public Version version = Version.AllThreeAxesAutoTilt;

    public GameObject scalableUI;
    public GameObject UIPanel;
    public GameObject HMDCamera;
    
    private bool isGrabbing = false;
    private RigidHand grabbingHand = null;

    void Update()
    {
        if (isGrabbing == true && grabbingHand.GetLeapHand().PinchStrength > 0.8f)
        {
            this.GetComponent<Renderer>().material.color = Color.green;

            Vector3 pinchPosition = grabbingHand.fingers[1].GetLeapFinger().TipPosition.ToVector3();
            //print(Vector3.Dot(Vector3.down, HMDCamera.transform.position - pinchPosition));


            //if (Vector3.Dot(Vector3.down, HMDCamera.transform.position - pinchPosition) < 0.45f &&
            //    Vector3.Dot(Vector3.down, HMDCamera.transform.position - pinchPosition) > -0.25f)
            if (true)
            {
                Vector3 facingDirection = HMDCamera.transform.position - (scalableUI.transform.position + scalableUI.transform.right * UIPanel.transform.localScale.x * 0.2f / 2 - scalableUI.transform.up * UIPanel.transform.localScale.y * 0.2f / 2);

                if (version == Version.AllThreeAxesAutoTilt)
                {
                    scalableUI.transform.forward = -facingDirection;
                }
                else if (version == Version.TwoAxesAutoTiltOneAxisManualTilt)
                {
                    scalableUI.transform.forward = new Vector3(-facingDirection.x, scalableUI.transform.forward.y, -facingDirection.z);

                }

                scalableUI.transform.position = pinchPosition - UIPanel.transform.right * UIPanel.transform.localScale.x * 0.2f / 2;
            }
        }
        else
        {
            isGrabbing = false;
            grabbingHand = null;

            this.GetComponent<Renderer>().material.color = Color.red;
        }
    }

	void OnTriggerStay(Collider collider)
    {
        if (collider.transform.GetComponentInParent<RigidFinger>())
        {
            if ((collider.transform.GetComponentInParent<RigidFinger>().fingerType == Finger.FingerType.TYPE_THUMB ||
                 collider.transform.GetComponentInParent<RigidFinger>().fingerType == Finger.FingerType.TYPE_INDEX) &&
                 collider.name == "bone3" &&
                 collider.transform.parent.GetComponentInParent<RigidHand>().GetLeapHand().PinchStrength > 0.8f)
            {
                isGrabbing = true;
                grabbingHand = collider.transform.parent.GetComponentInParent<RigidHand>();
                


            }
        }
    }
}
