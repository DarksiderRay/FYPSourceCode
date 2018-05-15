using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using UnityEngine.UI;

public class TiltCollider : MonoBehaviour {

    public enum Version { AllThreeAxesAutoTilt, TwoAxesAutoTiltOneAxisManualTilt, AllThreeAxesManualTilt }

    public Version version = Version.AllThreeAxesAutoTilt;

    public GameObject scalableUI;
    public GameObject scalableUIObject;

    private bool isTilting = false;
    private RigidHand tiltingHand = null;
	
	// Update is called once per frame
	void Update ()
    {
        if (isTilting == true && tiltingHand.GetLeapHand().PinchStrength > 0.95f)
        {
            this.GetComponent<Renderer>().material.color = Color.green;
            this.transform.parent.parent.GetComponent<Renderer>().material.color = Color.green;

            Vector3 pinchPosition = tiltingHand.fingers[1].GetLeapFinger().TipPosition.ToVector3();

            Vector3 tiltingDirection = pinchPosition - this.transform.parent.parent.position;

            if (version == Version.TwoAxesAutoTiltOneAxisManualTilt)
            {
                Vector3 initialVector = scalableUIObject.transform.InverseTransformPoint(this.transform.position) - scalableUIObject.transform.InverseTransformPoint(scalableUI.transform.position);
                Vector3 finalVector = scalableUIObject.transform.InverseTransformPoint(pinchPosition) - scalableUIObject.transform.InverseTransformPoint(scalableUI.transform.position);

                Quaternion quaternion = Quaternion.FromToRotation(initialVector, finalVector);

                quaternion.eulerAngles = new Vector3(quaternion.eulerAngles.x, 0, 0);

                scalableUI.transform.Rotate(quaternion.eulerAngles);
            }
            else if (version == Version.AllThreeAxesManualTilt)
            {
                scalableUI.transform.forward = -tiltingDirection;
            }
        }

        else
        {
            isTilting = false;
            tiltingHand = null;

            this.GetComponent<Renderer>().material.color = Color.red;
            this.transform.parent.parent.GetComponent<Renderer>().material.color = Color.red;
        }
    }

    void OnTriggerStay(Collider collider)
    {
        if (collider.transform.GetComponentInParent<RigidFinger>())
        {
            if ((collider.transform.GetComponentInParent<RigidFinger>().fingerType == Finger.FingerType.TYPE_THUMB ||
                 collider.transform.GetComponentInParent<RigidFinger>().fingerType == Finger.FingerType.TYPE_INDEX) &&
                collider.name == "bone3" &&
                collider.transform.parent.GetComponentInParent<RigidHand>().GetLeapHand().PinchStrength > 0.95f)
            {
                isTilting = true;
                tiltingHand = collider.transform.parent.GetComponentInParent<RigidHand>();
            }
        }         
    }
}
