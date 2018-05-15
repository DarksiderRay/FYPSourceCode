using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class ButtonColliderTest : MonoBehaviour {

    public GameObject testObject;
    public GameObject buttonPressed;

    private static bool canGoLeft;
    private static bool canGoRight;

    void Start()
    {
        canGoLeft = true;
        canGoRight = true;
    }

    void OnTriggerStay(Collider collider)
    {
        Debug.Log("Collider: " + collider);
        if (collider.name == "bone3" || collider.name == "bone2")
        {
            if (this.name == "Left Button" && canGoLeft)
            {
                this.transform.GetComponent<MeshRenderer>().enabled = false;
                buttonPressed.GetComponent<MeshRenderer>().enabled = true;
                testObject.GetComponent<Rigidbody>().velocity = new Vector3(-1f, 0, 0);
                canGoRight = false;
            }
            else if (this.name == "Right Button" && canGoRight)
            {
                this.transform.GetComponent<MeshRenderer>().enabled = false;
                buttonPressed.GetComponent<MeshRenderer>().enabled = true;
                testObject.GetComponent<Rigidbody>().velocity = new Vector3(1f, 0, 0);
                canGoLeft = false;
            }
        }
    }

    void OnTriggerExit()
    {
        testObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

        this.transform.GetComponent<MeshRenderer>().enabled = true;
        buttonPressed.GetComponent<MeshRenderer>().enabled = false;
        canGoLeft = true;
        canGoRight = true;
    }
}
