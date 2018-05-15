using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;

public class ScaleCollider2D : MonoBehaviour
{
    public GameObject scalableUIObject;
    public GameObject UIPanel;
    public GameObject colliderGroup;
    public GameObject grabCollider;

    private Vector3 pinchPosition;
    private Vector2 distanceVectorFromPinchPositionToPivot;

    private bool isScaling = false;
    private RigidHand scalingHand = null;

    private float initialWidth;
    private float initialHeight;

    void Start()
    {
        initialWidth = UIPanel.GetComponent<RectTransform>().sizeDelta.x;
        initialHeight = UIPanel.GetComponent<RectTransform>().sizeDelta.y;
    }

    void Update()
    {
        if (isScaling == true && scalingHand.GetLeapHand().PinchStrength > 0.95f)
        {
            this.GetComponent<Renderer>().material.color = Color.green;

            pinchPosition = scalingHand.fingers[1].GetLeapFinger().TipPosition.ToVector3();

            float xOffset = Vector3.Distance(UIPanel.transform.position, pinchPosition) * Mathf.Cos(Vector3.Angle(UIPanel.transform.right, pinchPosition - UIPanel.transform.position) * Mathf.Deg2Rad);
            float yOffset = Vector3.Distance(UIPanel.transform.position, pinchPosition) * Mathf.Cos(Vector3.Angle(-UIPanel.transform.up, pinchPosition - UIPanel.transform.position) * Mathf.Deg2Rad) / initialHeight * initialWidth;

            if (xOffset < yOffset)
            {
                scalableUIObject.GetComponent<RectTransform>().sizeDelta = new Vector2(xOffset, xOffset);

                UIPanel.transform.localScale = new Vector3(Mathf.Clamp(xOffset / 0.2f, 0.5f, Mathf.Infinity), Mathf.Clamp(xOffset / 0.2f, 0.5f, Mathf.Infinity), UIPanel.transform.localScale.z);
                colliderGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Clamp(xOffset, 0.1f, Mathf.Infinity), Mathf.Clamp(xOffset / initialWidth * initialHeight, 0.1f * initialHeight / initialWidth, Mathf.Infinity));

                grabCollider.transform.localScale = new Vector3(Mathf.Clamp(xOffset / 0.2f, 0.5f, Mathf.Infinity) * 0.15f, grabCollider.transform.localScale.y, grabCollider.transform.localScale.z);
            }

            else
            {
                scalableUIObject.GetComponent<RectTransform>().sizeDelta = new Vector2(yOffset, yOffset);

                UIPanel.transform.localScale = new Vector3(Mathf.Clamp(yOffset / 0.2f, 0.5f, Mathf.Infinity), Mathf.Clamp(yOffset / 0.2f, 0.5f, Mathf.Infinity), UIPanel.transform.localScale.z);
                colliderGroup.GetComponent<RectTransform>().sizeDelta = new Vector2(Mathf.Clamp(yOffset, 0.1f, Mathf.Infinity), Mathf.Clamp(yOffset / initialWidth * initialHeight, 0.1f * initialHeight / initialWidth, Mathf.Infinity));

                grabCollider.transform.localScale = new Vector3(Mathf.Clamp(yOffset / 0.2f, 0.5f, Mathf.Infinity) * 0.15f, grabCollider.transform.localScale.y, grabCollider.transform.localScale.z);
            }
        }

        else
        {
            isScaling = false;
            scalingHand = null;

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
                collider.transform.parent.GetComponentInParent<RigidHand>().GetLeapHand().PinchStrength > 0.95f)
            {
                isScaling = true;
                scalingHand = collider.transform.parent.GetComponentInParent<RigidHand>();          
            }
        }
    }
    

}