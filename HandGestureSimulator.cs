using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap.Unity;
using UnityEngine.Events;

public class HandGestureSimulator: MonoBehaviour {

    enum Pose { WaveIn, WaveOut, Fist, FingersSpread, Rest, DoubleClick };
    Pose currentPose;
    Pose lastPose;

    bool allFingersStretched;

    // Booleans and frame count for double-clicking
    int maxIntervalFrameCountBetweenPinches = 40;
    int intervalFrameCountAfterFirstPinch = 0;
    bool awaitingFirstPinch = true;
    bool firstPinch = false;
    bool awaitingSecondPinch = false;
    bool secondPinch = false;
    enum DoubleClickState {AwaitingFirstPinch, FirstPinch, AwaitingSecondPinch, SecondPinch, ExceededFrameCountLimit };
    DoubleClickState doubleClickState;
    DoubleClickState lastDoubleClickState;

    // Triggered events for each pose
    public UnityEvent WaveLeftInvokeOnce;
    public UnityEvent WaveLeftInvokeContinuously;
    public UnityEvent WaveRightInvokeOnce;
    public UnityEvent WaveRightInvokeContinuously;
    public UnityEvent WaveUpInvokeOnce;
    public UnityEvent WaveUpInvokeContinuously;
    public UnityEvent WaveDownInvokeOnce;
    public UnityEvent WaveDownInvokeContinuously;
    public UnityEvent FistInvokeOnce;
    public UnityEvent FistInvokeContinuously;
    public UnityEvent FingersSpreadInvokeOnce;
    public UnityEvent FingersSpreadInvokeContinuously;
    public UnityEvent DoubleClickInvokeOnce;

	// Use this for initialization
	void Start () {
        currentPose = Pose.Rest;
	}
	
	// Update is called once per frame
	void Update () {
        //Debug.Log("Palm Normal: " + this.GetComponent<HandModel>().GetPalmNormal());
        //Debug.Log(this.GetComponent<HandModel>().GetPalmDirection());

        // Double-clicking logic =================================================================================================================

        // print(this.GetComponent<HandModel>().GetLeapHand().PinchStrength);

        switch(doubleClickState)
        {
            case DoubleClickState.AwaitingFirstPinch:
                if (this.GetComponent<HandModel>().GetLeapHand().PinchStrength > 0.95)
                {
                    doubleClickState = DoubleClickState.FirstPinch;
                    intervalFrameCountAfterFirstPinch++;
                }
                intervalFrameCountAfterFirstPinch = 0;
                break;

            case DoubleClickState.FirstPinch:
                if (intervalFrameCountAfterFirstPinch > maxIntervalFrameCountBetweenPinches)
                {
                    doubleClickState = DoubleClickState.ExceededFrameCountLimit;
                }
                else if (this.GetComponent<HandModel>().GetLeapHand().PinchStrength < 0.95)
                {
                    doubleClickState = DoubleClickState.AwaitingSecondPinch;
                }
                intervalFrameCountAfterFirstPinch++;
                break;

            case DoubleClickState.AwaitingSecondPinch:
                if (intervalFrameCountAfterFirstPinch > maxIntervalFrameCountBetweenPinches)
                {
                    doubleClickState = DoubleClickState.ExceededFrameCountLimit;
                }
                else if (this.GetComponent<HandModel>().GetLeapHand().PinchStrength > 0.95)
                {
                    doubleClickState = DoubleClickState.SecondPinch;
                }
                intervalFrameCountAfterFirstPinch++;
                break;

            case DoubleClickState.SecondPinch:
                if (this.GetComponent<HandModel>().GetLeapHand().PinchStrength < 0.95)
                {
                    doubleClickState = DoubleClickState.ExceededFrameCountLimit;
                }
                intervalFrameCountAfterFirstPinch = 0;
                break;

            case DoubleClickState.ExceededFrameCountLimit:
                intervalFrameCountAfterFirstPinch = 0;
                if (this.GetComponent<HandModel>().GetLeapHand().PinchStrength < 0.95)
                {
                    doubleClickState = DoubleClickState.AwaitingFirstPinch;
                }
                break;

            default:
                break;
        }

        //print(doubleClickState);

        // ========================================================================================================================================

        // Pose update ===========================================================================================================================

        allFingersStretched = true;
        FingerModel[] fingers = this.GetComponent<HandModel>().fingers;
        foreach (FingerModel finger in fingers)
        {
            if (!finger.GetLeapFinger().IsExtended)
            {
                allFingersStretched = false;
            }
        }

        if (Vector3.Dot(this.GetComponent<HandModel>().GetArmDirection(), this.GetComponent<HandModel>().GetPalmNormal()) > 0.7)
        {
            currentPose = Pose.WaveOut;
            print("Pose: WaveOut");
        }
        else if (Vector3.Dot(this.GetComponent<HandModel>().GetArmDirection(), this.GetComponent<HandModel>().GetPalmNormal()) < -0.5)
        {
            currentPose = Pose.WaveIn;
            print("Pose: WaveIn");
        }
        else if (this.GetComponent<HandModel>().GetLeapHand().GrabStrength == 1)
        {
            currentPose = Pose.Fist;
            print("Pose: Fist");
        }
        else if (allFingersStretched &&
                 Vector3.Dot(fingers[0].GetBoneDirection(1), fingers[1].GetBoneDirection(1)) < 0.83 &&
                 Vector3.Dot(fingers[1].GetBoneDirection(1), fingers[2].GetBoneDirection(1)) < 0.99 &&
                 Vector3.Dot(fingers[2].GetBoneDirection(1), fingers[3].GetBoneDirection(1)) < 0.99 &&
                 Vector3.Dot(fingers[3].GetBoneDirection(1), fingers[4].GetBoneDirection(1)) < 0.99)
        {
            currentPose = Pose.FingersSpread;
            print("Pose: Fingers Spread");
        }
        else if (doubleClickState == DoubleClickState.SecondPinch)
        {
            currentPose = Pose.DoubleClick;
            print("Pose: DoubleClick");
        }
        else
        {
            currentPose = Pose.Rest;
            print("Pose: Rest");
        }

        //print(Vector3.Dot(fingers[0].GetBoneDirection(1), fingers[1].GetBoneDirection(1)) + ", " +
        //      Vector3.Dot(fingers[1].GetBoneDirection(1), fingers[2].GetBoneDirection(1)) + "," +
        //      Vector3.Dot(fingers[2].GetBoneDirection(1), fingers[3].GetBoneDirection(1)) + "," +
        //      Vector3.Dot(fingers[3].GetBoneDirection(1), fingers[4].GetBoneDirection(1)));

        //print(Vector3.Dot(this.GetComponent<HandModel>().GetArmDirection(), this.GetComponent<HandModel>().GetPalmNormal()));

        // ========================================================================================================================================

        // Event for each respective pose ========================================================================================================

        if (currentPose == Pose.DoubleClick && lastDoubleClickState != DoubleClickState.SecondPinch)
        {
            DoubleClickInvokeOnce.Invoke();
        }
        else if (doubleClickState == DoubleClickState.AwaitingFirstPinch)
        {
            if (currentPose == Pose.Fist)
            {
                if (lastPose != Pose.Fist)
                {
                    FistInvokeOnce.Invoke();
                }
                    FistInvokeContinuously.Invoke();
            }

            else if (currentPose == Pose.FingersSpread)
            {
                if (lastPose != Pose.FingersSpread)
                {
                    FingersSpreadInvokeOnce.Invoke();
                }
                FingersSpreadInvokeContinuously.Invoke();
            }

            else if (currentPose == Pose.WaveIn && Vector3.Dot(this.GetComponent<HandModel>().forearm.transform.up, Vector3.up) > 0.5)  // move down
            {
                if (lastPose != Pose.WaveIn)
                {
                    WaveDownInvokeOnce.Invoke();
                }
                WaveDownInvokeContinuously.Invoke();
            }

            else if (currentPose == Pose.WaveOut && Vector3.Dot(this.GetComponent<HandModel>().forearm.transform.up, Vector3.up) > 0.5) // move up
            {
                if (lastPose != Pose.WaveOut)
                {
                    WaveUpInvokeOnce.Invoke();
                }
                WaveUpInvokeContinuously.Invoke();
            }

            else if ((currentPose == Pose.WaveIn && this.GetComponent<HandModel>().Handedness == Chirality.Right) ||
                (currentPose == Pose.WaveOut && this.GetComponent<HandModel>().Handedness == Chirality.Left))
            {
                if ((this.GetComponent<HandModel>().Handedness == Chirality.Left && Vector3.Dot(this.GetComponent<HandModel>().forearm.transform.right, Vector3.up) > 0.5) ||
                         (this.GetComponent<HandModel>().Handedness == Chirality.Right && Vector3.Dot(this.GetComponent<HandModel>().forearm.transform.right, Vector3.down) > 0.5)) // move left
                {
                    if (lastPose != Pose.WaveIn && lastPose != Pose.WaveOut)
                    {
                        WaveLeftInvokeOnce.Invoke();
                    }
                    WaveLeftInvokeContinuously.Invoke();
                }
            }

            else if ((currentPose == Pose.WaveIn && this.GetComponent<HandModel>().Handedness == Chirality.Left) ||
                    (currentPose == Pose.WaveOut && this.GetComponent<HandModel>().Handedness == Chirality.Right))
            {
                if ((this.GetComponent<HandModel>().Handedness == Chirality.Left && Vector3.Dot(this.GetComponent<HandModel>().forearm.transform.right, Vector3.up) > 0.5) ||
                         (this.GetComponent<HandModel>().Handedness == Chirality.Right && Vector3.Dot(this.GetComponent<HandModel>().forearm.transform.right, Vector3.down) > 0.5)) // move right
                {
                    if (lastPose != Pose.WaveIn && lastPose != Pose.WaveOut)
                    {
                        WaveRightInvokeOnce.Invoke();
                    }
                    WaveRightInvokeContinuously.Invoke();
                }
            }  
        }

        // =======================================================================================================================================

        lastDoubleClickState = doubleClickState;
        lastPose = currentPose;
    }
}
