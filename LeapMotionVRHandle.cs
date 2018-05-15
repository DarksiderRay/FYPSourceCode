using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Leap.Unity;

public class LeapMotionVRHandle : MonoBehaviour {

    public bool thumb;
    public bool index;
    public bool middle;
    public bool ring;
    public bool pinky;

    public HandModel leftHandModel;
    public HandModel rightHandModel;

    private Slider slider;
    private RectTransform rectTransform;
    private float minX;
    private float maxX;
    private float minY;
    private float maxY;

    private float handleMinPositionX;
    private float handleMaxPositionX;

    void Start()
    {
        slider = GetComponentInParent<Slider>();
        rectTransform = GetComponent<RectTransform>();
        minX = rectTransform.offsetMin.x;
        minY = rectTransform.offsetMin.y;
        maxX = rectTransform.offsetMax.x;
        maxY = rectTransform.offsetMax.y;

        slider.value = 0;
        Debug.Log("MinHandlePos: " + transform.localPosition);
        handleMinPositionX = transform.localPosition.x;
        slider.value = slider.maxValue;
        Debug.Log("MaxHandlePos: " + transform.localPosition);
        handleMaxPositionX = transform.localPosition.x;

        // Default finger boolean values
        thumb = false;
        index = true;
        middle = true;
        ring = false;
        pinky = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(leftHandModel.GetPalmNormal());

        //if (leftHandModel.GetPalmNormal().z < 0)
        //{
        //    slider.gameObject.SetActive(false);
        //}
        //else if (leftHandModel.GetPalmNormal().z > 0)
        //{
        //    slider.gameObject.SetActive(true);
        //}

        // Debug.Log("Handle position: " + this.transform.localPosition + ", minX: " + minX + " , minY: " + minY + " , maxX: " + maxX + " , maxY: " + maxY);
    }

    void OnTriggerStay(Collider collider)
    {
        if ((collider.name == "bone3") && 
            ((collider.transform.parent.name == "thumb" && thumb) ||
            (collider.transform.parent.name == "index" && index) ||
            (collider.transform.parent.name == "middle" && middle) ||
            (collider.transform.parent.name == "ring" && ring) ||
            (collider.transform.parent.name == "pinky" && pinky)))
        {
            Vector3 colliderRelativeToSliderPosition = slider.transform.InverseTransformPoint(collider.transform.position);

            // Debug.Log(collider.transform.parent.name + " triggered!");

            slider.value = Mathf.Clamp(((colliderRelativeToSliderPosition.x - handleMinPositionX) / (handleMaxPositionX - handleMinPositionX)), 0, slider.maxValue);
            //Debug.Log("colliderRelativeToSliderPosition: " + colliderRelativeToSliderPosition + ", SliderValue: " + slider.value);
            //Debug.Log(slider.value);
        }
    }
}
