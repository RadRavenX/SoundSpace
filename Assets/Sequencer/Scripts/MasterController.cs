﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MasterController : MonoBehaviour
{
    MusicNode selected = null;
    public Recorder r;
    private bool recording = false;
    int counter = 0;
    AudioClip clip;
    float startRecordingTime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TouchTrigger();
    }


    public void Test()
    {
        print("button");
    }
    public void TouchTrigger()
    {
        //if (selected != null)
        //{
        //    return;
        //}
        //if ((Input.touchCount > 0) && (Input.GetTouch(0).phase == TouchPhase.Began))
        if (Input.GetMouseButtonDown(0))
        {
            //Ray raycast = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
            Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;
            if (Physics.Raycast(raycast, out raycastHit))
            {
                string targ = null;
                if(raycastHit.collider.transform.parent != null)
                {
                    targ = raycastHit.collider.transform.parent.name;
                }
                if (targ != null)
                {
                    if (targ.Contains("music node"))
                    {

                        MusicNode selected_new = (MusicNode)raycastHit.collider.transform.parent.GetComponent("MusicNode");
                        if (selected != null)
                        {
                            if (selected_new.name != selected.name)
                            {
                                selected.Deselected();
                                selected = selected_new;
                            }
                        }
                        else
                        {
                            selected = selected_new;
                        }
                        selected.Selected();
                        print(targ);
                    }
                }

                if (raycastHit.collider.GetComponent("Recorder") != null)
                {
                    if (selected != null)
                    {
                        recording = true;
                        r.Selected();
                        StartRecording();
                        print("Recording");
                    }

                }
            }
            else
            {
                if (selected != null)
                {
                    selected.Deselected();
                    selected = null;
                }

            }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (recording == true)
            {
                recording = false;
                r.Deselected();
                StopRecording();
                print("stopped recording");
            }
        }
    }



    public void StartRecording()
    {
        //Get the max frequency of a microphone, if it's less than 44100 record at the max frequency, else record at 44100
        int minFreq;
        int maxFreq;
        int freq = 44100;
        Microphone.GetDeviceCaps("", out minFreq, out maxFreq);
        if (maxFreq < 44100)
            freq = maxFreq;

        //Start the recording, the length of 300 gives it a cap of 5 minutes
        clip = Microphone.Start("", false, 300, 44100);
        startRecordingTime = Time.time;
    }


    public void StopRecording()
    {
        //End the recording when the mouse comes back up, then play it
        Microphone.End("");

        //Trim the audioclip by the length of the recording
        AudioClip recordingNew = AudioClip.Create(clip.name, (int)((Time.time - startRecordingTime) * clip.frequency), clip.channels, clip.frequency, false);
        float[] data = new float[(int)((Time.time - startRecordingTime) * clip.frequency)];
        clip.GetData(data, 0);
        recordingNew.SetData(data, 0);
        selected.SetClip(recordingNew);
        //Play recording
    }



}
