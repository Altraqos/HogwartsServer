﻿/*
 * GestureRecognition - VR gesture recognition library plug-in for Unity.
 * Copyright (c) 2019 MARUI-PlugIn (inc.)
 * 
 * This software is free to use for non-commercial purposes.
 * You may use this software in part or in full for any project
 * that does not pursue financial gain, including free software 
 * and projectes completed for evaluation or educational purposes only.
 * Any use for commercial purposes is prohibited.
 * You may not sell or rent any software that includes
 * this software in part or in full, either in it's original form
 * or in altered form.
 * If you wish to use this software in a commercial application,
 * please contact us at support@marui-plugin.com to obtain
 * a commercial license.
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
 * "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
 * THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
 * PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, 
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, 
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR 
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY 
 * OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT 
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * --------------------------------------------------------------------------
 * # HOW TO USE:
 * 
 * (1) Place the GestureRecognition_Win_x86_64.dll (Windows) and/or
 * GestureRecognition_Android_ARM64.so (Android / MobileVR) files 
 * in the /Assets/Plugins/ folder in your unity project
 * and add the GestureRecognition.cs file to your project scripts. 
 * 
 * 
 * (2) Create a new Gesture recognition object and register the gestures that you want to identify later.
 * <code>
 * GestureRecognition gr = new GestureRecognition();
 * int myFirstGesture = gr.createGesture("my first gesture");
 * int mySecondGesture = gr.createGesture("my second gesture");
 * </code>
 * 
 * 
 * (3) Record a number of samples for each gesture by calling startStroke(), contdStroke() and endStroke()
 * for your registered gestures, each time inputting the headset and controller transformation.
 * <code>
 * Vector3 hmd_p = Camera.main.gameObject.transform.position;
 * Quaternion hmd_q = Camera.main.gameObject.transform.rotation;
 * gr.startStroke(hmd_p, hmd_q, myFirstGesture);
 * 
 * // repeat the following while performing the gesture with your controller:
 * Vector3 p = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
 * Quaternion q = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
 * gr.contdStroke(p,q);
 * // ^ repead while performing the gesture with your controller.
 * 
 * gr.endStroke();
 * </code>
 * Repeat this multiple times for each gesture you want to identify.
 * We recommend recording at least 20 samples for each gesture.
 * 
 * 
 * (4) Start the training process by calling startTraining().
 * You can optionally register callback functions to receive updates on the learning progress
 * by calling setTrainingUpdateCallback() and setTrainingFinishCallback().
 * <code>
 * gr.setMaxTrainingTime(10); // Set training time to 10 seconds.
 * gr.startTraining();
 * </code>
 * You can stop the training process by calling stopTraining().
 * After training, you can check the gesture identification performance by calling recognitionScore()
 * (a value of 1 means 100% correct recognition).
 * 
 * 
 * (5) Now you can identify new gestures performed by the user in the same way
 * as you were recording samples:
 * <code>
 * Vector3 hmd_p = Camera.main.gameObject.transform.position;
 * Quaternion hmd_q = Camera.main.gameObject.transform.rotation;
 * gr.startStroke(hmd_p, hmd_q);
 * 
 * // repeat the following while performing the gesture with your controller:
 * Vector3 p = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
 * Quaternion q = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);
 * gr.contdStroke(p,q);
 * // ^ repeat while performing the gesture with your controller.
 * 
 * int identifiedGesture = gr.endStroke();
 * if (identifiedGesture == myFirstGesture) {
 *     // ...
 * }
 * </code>
 * 
 * 
 * (6) Now you can save and load the artificial intelligence.
 * <code>
 * gr.saveToFile("C:/myGestures.dat");
 * // ...
 * gr.loadFromFile("C:/myGestures.dat");
 * </code>
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Runtime.InteropServices;
using System.Text;

public class GestureRecognition {

    //                                                          ________________________________
    //_________________________________________________________/  ignoreHeadRotationLeftRight
    /// <summary>
    /// Setting whether the horizontal rotation of the users head
    /// (commonly called "pan" or "yaw", looking left or right) should be considered when 
    /// recording and performing gestures.
    /// By default, the users view direction is considered in the calculation of gestures.
    /// So if you turn away from your PC and make a "swipe right" gesture, your visual "right"
    /// is considered, not "the right side of your PC or room".
    /// If you consider gestures to be "room-relative", set this variable to "true".
    /// </summary>
    public bool ignoreHeadRotationLeftRight
    {
        get
        {
            return GestureRecognition_getIgnoreHeadRotationY(m_gro) != 0;
        }
        set
        {
            GestureRecognition_setIgnoreHeadRotationY(m_gro, value ? 1 : 0);
        }
    }


    //                                                          ________________________________
    //_________________________________________________________/   ignoreHeadRotationUpDown
    /// <summary>
    /// Setting whether the vertical rotation of the users head
    /// (commonly called "pitch", looking up or down) should be considered when 
    /// recording and performing gestures.
    /// By default, the users view direction is considered in the calculation of gestures.
    /// So if you look up and make a "pushing" gesture, it is considered to be a "forward"
    /// motion, not an "upward" motion.
    /// If you consider gestures to be "room-relative", set this variable to "true".
    /// </summary>
    public bool ignoreHeadRotationUpDown
    {
        get
        {
            return GestureRecognition_getIgnoreHeadRotationX(m_gro) != 0;
        }
        set
        {
            GestureRecognition_setIgnoreHeadRotationX(m_gro, value ? 1 : 0);
        }
    }


    //                                                          ________________________________
    //_________________________________________________________/   ignoreHeadRotationTilt
    /// <summary>
    /// Setting whether the tilting rotation of the users head
    /// (also called "roll" or "bank", tilting the head to the site without changing the
    /// view direction) should be considered when recording and performing gestures.
    /// By default, the users view direction is considered in the calculation of gestures.
    /// So if you tilt your head 90 degrees to the side a and make a "swipe right" gesture,
    /// your visual "right" is considered, not "the right side of your PC or room".
    /// If you consider gestures to be "room-relative", set this variable to "true".
    /// </summary>
    public bool ignoreHeadRotationTilt
    {
        get
        {
            return GestureRecognition_getIgnoreHeadRotationZ(m_gro) != 0;
        }
        set
        {
            GestureRecognition_setIgnoreHeadRotationZ(m_gro, value ? 1 : 0);
        }
    }


    //                                                          ________________________________
    //_________________________________________________________/     GestureRecognition()
    /// <summary>
    /// Constructor.
    /// </summary>
    public GestureRecognition()
    {
        m_gro = GestureRecognition_create();
    }
    //                                                          ________________________________
    //_________________________________________________________/   ~GestureRecognition()
    /// <summary>
    /// Destructor.
    /// </summary>
    ~GestureRecognition()
    {
        if (m_gro.ToInt64() != 0)
        {
            GestureRecognition_delete(m_gro);
        }
    }
    //                                                          ________________________________
    //_________________________________________________________/         startStroke()
    /// <summary>
    /// Start a new gesture (stroke) performance.
    /// If record_as_sample is set to a gesture ID, the following gesture performance will be recorded as a
    /// sample of that gesture (sample-recording-mode).
    /// If record_as_sample is not set, the gesture recognition library will attempt to identify the
    /// gesture based on previously recorded samples (gesture-identification-mode).
    /// Note that you must first record samples and train the artificial intelligence (by calling startTraining())
    /// before new and unknown gestures can be identified.
    /// </summary>
    /// <param name="hmd">Current transformation of the VR headset / HMD.
    /// This must be a double[4,4] column-major matrix (where the translational component is in the m[3][*] sub-array).</param>
    /// <param name="record_as_sample">[OPTIONAL] Set this to the ID of a gesture to start recording a sample for that gesture.</param>
    /// <returns>
    /// True on success, false on failure.
    /// </returns>
    public bool startStroke(double[,] hmd, int record_as_sample = -1)
    {
        return GestureRecognition_startStrokeM(m_gro, hmd, record_as_sample) != 0;
    }
    //                                                          ________________________________
    //_________________________________________________________/         startStroke()
    /// <summary>
    /// Start a new gesture (stroke) performance.
    /// If record_as_sample is set to a gesture ID, the following gesture performance will be recorded as a
    /// sample of that gesture (sample-recording-mode).
    /// If record_as_sample is not set, the gesture recognition library will attempt to identify the
    /// gesture based on previously recorded samples (gesture-identification-mode).
    /// Note that you must first record samples and train the artificial intelligence (by calling startTraining())
    /// before new and unknown gestures can be identified.
    /// </summary>
    /// <param name="hmd_p">The current position of the VR headset (users POV).</param>
    /// <param name="hmd_q">The current rotation of the VR headset (users POV).</param>
    /// <param name="record_as_sample">[OPTIONAL] Set this to the ID of a gesture to start recording a sample for that gesture.</param>
    /// <returns>
    /// True on success, false on failure.
    /// </returns>
    public bool startStroke(Vector3 hmd_p, Quaternion hmd_q, int record_as_sample = -1)
    {
        if (ignoreHeadRotationLeftRight || ignoreHeadRotationUpDown || ignoreHeadRotationTilt)
        {
            Vector3 r = hmd_q.eulerAngles;
            if (ignoreHeadRotationUpDown)
            {
                r.x = 0;
            }
            if (ignoreHeadRotationLeftRight)
            {
                r.y = 0;
            }
            if (ignoreHeadRotationTilt)
            {
                r.z = 0;
            }
            hmd_q.eulerAngles = r;
        }
        double[] p = new double[3] { hmd_p.x, hmd_p.y, hmd_p.z };
        double[] q = new double[4] { hmd_q.x, hmd_q.y, hmd_q.z, hmd_q.w };
        return GestureRecognition_startStroke(m_gro, p, q, record_as_sample) != 0;
    }
    //                                                          ________________________________
    //_________________________________________________________/         startStroke()
    /// <summary>
    /// Start a new gesture (stroke) performance.
    /// If record_as_sample is set to a gesture ID, the following gesture performance will be recorded as a
    /// sample of that gesture (sample-recording-mode).
    /// If record_as_sample is not set, the gesture recognition library will attempt to identify the
    /// gesture based on previously recorded samples (gesture-identification-mode).
    /// Note that you must first record samples and train the artificial intelligence (by calling startTraining())
    /// before new and unknown gestures can be identified.
    /// </summary>
    /// <param name="hmd_p">The current position of the VR headset (users POV). This must be a double[3] array.</param>
    /// <param name="hmd_q">The current rotation of the VR headset (users POV). This must be a double[4] array.</param>
    /// <param name="record_as_sample">[OPTIONAL] Set this to the ID of a gesture to start recording a sample for that gesture.</param>
    /// <returns>
    /// True on success, false on failure.
    /// </returns>
    public bool startStroke(double[] hmd_p, double[] hmd_q, int record_as_sample = -1)
    {
        return GestureRecognition_startStroke(m_gro, hmd_p, hmd_q, record_as_sample) != 0;
    }
    //                                                          ________________________________
    //_________________________________________________________/         contdStroke()
    /// <summary>
    /// Continue performing a gesture.
    /// </summary>
    /// <param name="m">Transformation of the controller with which the gesture is performed.
    /// This must be a double[4,4] column-major matrix (where the translational component is in the m[3][*] sub-array).</param>
    /// <returns>
    /// True on success, false on failure.
    /// </returns>
    public bool contdStroke(double[,] m)
    {
        return GestureRecognition_contdStrokeM(m_gro, m) != 0;
    }
    //                                                          ________________________________
    //_________________________________________________________/         contdStroke()
    /// <summary>
    /// Continue performing a gesture.
    /// </summary>
    /// <param name="p">Position of the controller with which the gesture is performed.</param>
    /// <param name="q">Rotation of the controller with which the gesture is performed.</param>
    /// <returns>
    /// True on success, false on failure.
    /// </returns>
    public bool contdStroke(Vector3 p, Quaternion q)
    {
        double[] _p = new double[3] { p.x, p.y, p.z };
        double[] _q = new double[4] { q.x, q.y, q.z, q.w };
        return GestureRecognition_contdStroke(m_gro, _p, _q) != 0;
    }
    //                                                          ________________________________
    //_________________________________________________________/          contdStroke()
    /// <summary>
    /// Continue performing a gesture.
    /// </summary>
    /// <param name="p">Position of the controller with which the gesture is performed. This must be a double[3] array.</param>
    /// <param name="q">Rotation of the controller with which the gesture is performed. This must be a double[4] array.</param>
    /// <returns>
    /// True on success, false on failure.
    /// </returns>
    public bool contdStroke(double[] p, double[] q)
    {
        return GestureRecognition_contdStroke(m_gro, p, q) != 0;
    }
    //                                                          ________________________________
    //_________________________________________________________/          endStroke()
    /// <summary>
    /// End a gesture (stroke).
    /// If the gesture was started as recording a new sample (sample-recording-mode), then
    /// the gesture will be added to the reference examples for this gesture.
    /// If the gesture (stroke) was started in identification mode, the gesture recognition
    /// library will attempt to identify the gesture.
    /// </summary>
    /// <param name="pos">[OUT] The position where the gesture was performed.</param>
    /// <param name="scale">[OUT] The scale at which the gesture was performed.</param>
    /// <param name="dir0">[OUT] The primary direction in which the gesture was performed (ie. widest direction).</param>
    /// <param name="dir1">[OUT] The secondary direction in which the gesture was performed.</param>
    /// <param name="dir2">[OUT] The minor direction in which the gesture was performed (ie. narrowest direction).</param>
    /// <returns>
    /// The ID of the gesture identified.
    /// </returns>
    public int endStroke(ref Vector3 pos, ref double scale, ref Vector3 dir0, ref Vector3 dir1, ref Vector3 dir2)
    {
        double[] _pos = new double[3];
        double[] _scale = new double[1];
        double[] _dir0 = new double[3];
        double[] _dir1 = new double[3];
        double[] _dir2 = new double[3];
        int ret = GestureRecognition_endStroke(m_gro, _pos, _scale, _dir0, _dir1, _dir2);
        pos.x = (float)_pos[0];
        pos.y = (float)_pos[1];
        pos.z = (float)_pos[2];
        scale = _scale[0];
        dir0.x = (float)_dir0[0];
        dir0.y = (float)_dir0[1];
        dir0.z = (float)_dir0[2];
        dir1.x = (float)_dir1[0];
        dir1.y = (float)_dir1[1];
        dir1.z = (float)_dir1[2];
        dir2.x = (float)_dir2[0];
        dir2.y = (float)_dir2[1];
        dir2.z = (float)_dir2[2];
        return ret;
    }
    //                                                          ________________________________
    //_________________________________________________________/          endStroke()
    /// <summary>
    /// End a gesture (stroke).
    /// If the gesture was started as recording a new sample (sample-recording-mode), then
    /// the gesture will be added to the reference examples for this gesture.
    /// If the gesture (stroke) was started in identification mode, the gesture recognition
    /// library will attempt to identify the gesture.
    /// </summary>
    /// <param name="pos">[OUT] The position where the gesture was performed.</param>
    /// <param name="scale">[OUT] The scale at which the gesture was performed.</param>
    /// <param name="dir0">[OUT] The primary direction in which the gesture was performed (ie. widest direction).</param>
    /// <param name="dir1">[OUT] The secondary direction in which the gesture was performed.</param>
    /// <param name="dir2">[OUT] The minor direction in which the gesture was performed (ie. narrowest direction).</param>
    /// <returns>
    /// The ID of the gesture identified.
    /// </returns>
    public int endStroke(ref Vector3 pos, ref double scale, ref Quaternion q)
    {
        double[] _pos = new double[3];
        double[] _scale = new double[1];
        double[] _dir0 = new double[3];
        double[] _dir1 = new double[3];
        double[] _dir2 = new double[3];
        int ret = GestureRecognition_endStroke(m_gro, _pos, _scale, _dir0, _dir1, _dir2);
        pos.x = (float)_pos[0];
        pos.y = (float)_pos[1];
        pos.z = (float)_pos[2];
        scale = _scale[0];
        double tr = _dir0[0] + _dir1[1] + _dir2[2];
        if (tr > 0)
        {
            double s = Math.Sqrt(tr + 1.0) * 2.0;
            q.w = (float)(0.25 * s);
            q.x = (float)((_dir1[2] - _dir2[1]) / s);
            q.y = (float)((_dir2[0] - _dir0[2]) / s);
            q.z = (float)((_dir0[1] - _dir1[0]) / s);
        }
        else if ((_dir0[0] > _dir1[1]) & (_dir0[0] > _dir2[2]))
        {
            double s = Math.Sqrt(1.0 + _dir0[0] - _dir1[1] - _dir2[2]) * 2.0;
            q.w = (float)((_dir1[2] - _dir2[1]) / s);
            q.x = (float)(0.25 * s);
            q.y = (float)((_dir1[0] + _dir0[1]) / s);
            q.z = (float)((_dir2[0] + _dir0[2]) / s);
        }
        else if (_dir1[1] > _dir2[2])
        {
            double s = Math.Sqrt(1.0 + _dir1[1] - _dir0[0] - _dir2[2]) * 2.0;
            q.w = (float)((_dir2[0] - _dir0[2]) / s);
            q.x = (float)((_dir1[0] + _dir0[1]) / s);
            q.y = (float)(0.25 * s);
            q.z = (float)((_dir2[1] + _dir1[2]) / s);
        }
        else
        {
            double s = Math.Sqrt(1.0 + _dir2[2] - _dir0[0] - _dir1[1]) * 2.0;
            q.w = (float)((_dir0[1] - _dir1[0]) / s);
            q.x = (float)((_dir2[0] + _dir0[2]) / s);
            q.y = (float)((_dir2[1] + _dir1[2]) / s);
            q.z = (float)(0.25 * s);
        }
        return ret;
    }
    //                                                          ________________________________
    //_________________________________________________________/          endStroke()
    /// <summary>
    /// End a gesture (stroke).
    /// If the gesture was started as recording a new sample (sample-recording-mode), then
    /// the gesture will be added to the reference examples for this gesture.
    /// If the gesture (stroke) was started in identification mode, the gesture recognition
    /// library will attempt to identify the gesture.
    /// </summary>
    /// <param name="pos">[OUT] The position where the gesture was performed. This must be a double[3] array.</param>
    /// <param name="scale">[OUT] The scale at which the gesture was performed. This must be a double[1] array.</param>
    /// <param name="dir0">[OUT] The primary direction in which the gesture was performed (ie. widest direction). This must be a double[3] array.</param>
    /// <param name="dir1">[OUT] The secondary direction in which the gesture was performed. This must be a double[3] array.</param>
    /// <param name="dir2">[OUT] The minor direction in which the gesture was performed (ie. narrowest direction). This must be a double[3] array.</param>
    /// <returns>
    /// The ID of the gesture identified.
    /// </returns>
    public int endStroke(ref double[] pos, ref double[] scale, ref double[] dir0, ref double[] dir1, ref double[] dir2)
    {
        return GestureRecognition_endStroke(m_gro, pos, scale, dir0, dir1, dir2);
    }
    //                                                          ________________________________
    //_________________________________________________________/          endStroke()
    /// <summary>
    /// End a gesture (stroke).
    /// If the gesture was started as recording a new sample (sample-recording-mode), then
    /// the gesture will be added to the reference examples for this gesture.
    /// If the gesture (stroke) was started in identification mode, the gesture recognition
    /// library will attempt to identify the gesture.
    /// </summary>
    /// <returns>
    /// The ID of the gesture identified.
    /// </returns>
    public int endStroke()
    {
        return GestureRecognition_endStroke(m_gro, null, null, null, null, null);
    }
    //                                                          ________________________________
    //_________________________________________________________/          endStroke()
    /// <summary>
    /// End a gesture (stroke).
    /// If the gesture was started as recording a new sample (sample-recording-mode), then
    /// the gesture will be added to the reference examples for this gesture.
    /// If the gesture (stroke) was started in identification mode, the gesture recognition
    /// library will attempt to identify the gesture.
    /// </summary>
    /// <param name="similarity">[OUT] Similarity between the performed stroke, and the identified gesture.</param>
    /// <param name="pos">[OUT] The position where the gesture was performed.</param>
    /// <param name="scale">[OUT] The scale at which the gesture was performed.</param>
    /// <param name="dir0">[OUT] The primary direction in which the gesture was performed (ie. widest direction).</param>
    /// <param name="dir1">[OUT] The secondary direction in which the gesture was performed.</param>
    /// <param name="dir2">[OUT] The minor direction in which the gesture was performed (ie. narrowest direction).</param>
    /// <returns>
    /// The ID of the gesture identified.
    /// </returns>
    public int endStroke(ref double similarity, ref Vector3 pos, ref double scale, ref Vector3 dir0, ref Vector3 dir1, ref Vector3 dir2)
    {
        double[] _similarity = new double[1];
        double[] _pos = new double[3];
        double[] _scale = new double[1];
        double[] _dir0 = new double[3];
        double[] _dir1 = new double[3];
        double[] _dir2 = new double[3];
        int ret = GestureRecognition_endStrokeAndGetSimilarity(m_gro, _similarity, _pos, _scale, _dir0, _dir1, _dir2);
        similarity = _similarity[0];
        pos.x = (float)_pos[0];
        pos.y = (float)_pos[1];
        pos.z = (float)_pos[2];
        scale = _scale[0];
        dir0.x = (float)_dir0[0];
        dir0.y = (float)_dir0[1];
        dir0.z = (float)_dir0[2];
        dir1.x = (float)_dir1[0];
        dir1.y = (float)_dir1[1];
        dir1.z = (float)_dir1[2];
        dir2.x = (float)_dir2[0];
        dir2.y = (float)_dir2[1];
        dir2.z = (float)_dir2[2];
        return ret;
    }
    //                                                          ________________________________
    //_________________________________________________________/          endStroke()
    /// <summary>
    /// End a gesture (stroke).
    /// If the gesture was started as recording a new sample (sample-recording-mode), then
    /// the gesture will be added to the reference examples for this gesture.
    /// If the gesture (stroke) was started in identification mode, the gesture recognition
    /// library will attempt to identify the gesture.
    /// </summary>
    /// <param name="similarity">[OUT] Similarity between the performed stroke, and the identified gesture.</param>
    /// <param name="pos">[OUT] The position where the gesture was performed.</param>
    /// <param name="scale">[OUT] The scale at which the gesture was performed.</param>
    /// <param name="dir0">[OUT] The primary direction in which the gesture was performed (ie. widest direction).</param>
    /// <param name="dir1">[OUT] The secondary direction in which the gesture was performed.</param>
    /// <param name="dir2">[OUT] The minor direction in which the gesture was performed (ie. narrowest direction).</param>
    /// <returns>
    /// The ID of the gesture identified.
    /// </returns>
    public int endStroke(ref double similarity, ref Vector3 pos, ref double scale, ref Quaternion q)
    {
        double[] _similarity = new double[1];
        double[] _pos = new double[3];
        double[] _scale = new double[1];
        double[] _dir0 = new double[3];
        double[] _dir1 = new double[3];
        double[] _dir2 = new double[3];
        int ret = GestureRecognition_endStrokeAndGetSimilarity(m_gro, _similarity, _pos, _scale, _dir0, _dir1, _dir2);
        similarity = _similarity[0];
        pos.x = (float)_pos[0];
        pos.y = (float)_pos[1];
        pos.z = (float)_pos[2];
        scale = _scale[0];
        double tr = _dir0[0] + _dir1[1] + _dir2[2];
        if (tr > 0)
        {
            double s = Math.Sqrt(tr + 1.0) * 2.0;
            q.w = (float)(0.25 * s);
            q.x = (float)((_dir1[2] - _dir2[1]) / s);
            q.y = (float)((_dir2[0] - _dir0[2]) / s);
            q.z = (float)((_dir0[1] - _dir1[0]) / s);
        }
        else if ((_dir0[0] > _dir1[1]) & (_dir0[0] > _dir2[2]))
        {
            double s = Math.Sqrt(1.0 + _dir0[0] - _dir1[1] - _dir2[2]) * 2.0;
            q.w = (float)((_dir1[2] - _dir2[1]) / s);
            q.x = (float)(0.25 * s);
            q.y = (float)((_dir1[0] + _dir0[1]) / s);
            q.z = (float)((_dir2[0] + _dir0[2]) / s);
        }
        else if (_dir1[1] > _dir2[2])
        {
            double s = Math.Sqrt(1.0 + _dir1[1] - _dir0[0] - _dir2[2]) * 2.0;
            q.w = (float)((_dir2[0] - _dir0[2]) / s);
            q.x = (float)((_dir1[0] + _dir0[1]) / s);
            q.y = (float)(0.25 * s);
            q.z = (float)((_dir2[1] + _dir1[2]) / s);
        }
        else
        {
            double s = Math.Sqrt(1.0 + _dir2[2] - _dir0[0] - _dir1[1]) * 2.0;
            q.w = (float)((_dir0[1] - _dir1[0]) / s);
            q.x = (float)((_dir2[0] + _dir0[2]) / s);
            q.y = (float)((_dir2[1] + _dir1[2]) / s);
            q.z = (float)(0.25 * s);
        }
        return ret;
    }
    //                                                          ________________________________
    //_________________________________________________________/          endStroke()
    /// <summary>
    /// End a gesture (stroke).
    /// If the gesture was started as recording a new sample (sample-recording-mode), then
    /// the gesture will be added to the reference examples for this gesture.
    /// If the gesture (stroke) was started in identification mode, the gesture recognition
    /// library will attempt to identify the gesture.
    /// </summary>
    /// <param name="similarity">[OUT] Similarity between the performed stroke, and the identified gesture.</param>
    /// <param name="pos">[OUT] The position where the gesture was performed. This must be a double[3] array.</param>
    /// <param name="scale">[OUT] The scale at which the gesture was performed. This must be a double[1] array.</param>
    /// <param name="dir0">[OUT] The primary direction in which the gesture was performed (ie. widest direction). This must be a double[3] array.</param>
    /// <param name="dir1">[OUT] The secondary direction in which the gesture was performed. This must be a double[3] array.</param>
    /// <param name="dir2">[OUT] The minor direction in which the gesture was performed (ie. narrowest direction). This must be a double[3] array.</param>
    /// <returns>
    /// The ID of the gesture identified.
    /// </returns>
    public int endStroke(ref double[] similarity, ref double[] pos, ref double[] scale, ref double[] dir0, ref double[] dir1, ref double[] dir2)
    {
        return GestureRecognition_endStrokeAndGetSimilarity(m_gro, similarity, pos, scale, dir0, dir1, dir2);
    }
    //                                                          ________________________________
    //_________________________________________________________/          endStroke()
    /// <summary>
    /// End a gesture (stroke).
    /// If the gesture was started as recording a new sample (sample-recording-mode), then
    /// the gesture will be added to the reference examples for this gesture.
    /// If the gesture (stroke) was started in identification mode, the gesture recognition
    /// library will attempt to identify the gesture.
    /// </summary>
    /// <param name="similarity">[OUT] Similarity between the performed stroke, and the identified gesture.</param>
    /// <returns>
    /// The ID of the gesture identified.
    /// </returns>
    public int endStroke(ref double similarity)
    {
        double[] _similarity = new double[1];
        int ret = GestureRecognition_endStrokeAndGetSimilarity(m_gro, _similarity, null, null, null, null, null);
        similarity = _similarity[0];
        return ret;
    }
    //                                                      ____________________________________
    //_____________________________________________________/ endStrokeAndGetAllProbabilities()
    /// <summary>
    /// End a gesture (stroke) and get the probability estimate of it to belong to each
    /// of the recorded (and trained) gestures.
    /// If the gesture was started as recording a new sample (sample-recording-mode), then
    /// the gesture will be added to the reference examples for this gesture.
    /// If the gesture (stroke) was started in identification mode, the gesture recognition
    /// library will calculate and return the probability for each recorded gesture.
    /// </summary>
    /// <returns>
    /// List of probability estimate values, where 1.0 is the highest and 0.0 is the lowest.
    /// </returns>
    public double[] endStrokeAndGetAllProbabilities()
    {
        int num_gestures = this.numberOfGestures();
        double[] p = new double[num_gestures];
        int n = GestureRecognition_endStrokeAndGetAllProbabilities(m_gro, p, num_gestures, null, null, null, null, null);
        if (n == num_gestures)
        {
            return p;
        }
        double[] p2 = new double[n];
        for (int i = 0; i < n; i++)
        {
            p2[i] = p[i];
        }
        return p2;
    }
    //                                                      ____________________________________
    //_____________________________________________________/ endStrokeAndGetAllProbabilities()
    /// <summary>
    /// End a gesture (stroke) and get the probability estimate of it to belong to each
    /// of the recorded (and trained) gestures.
    /// If the gesture was started as recording a new sample (sample-recording-mode), then
    /// the gesture will be added to the reference examples for this gesture.
    /// If the gesture (stroke) was started in identification mode, the gesture recognition
    /// library will calculate and return the probability for each recorded gesture.
    /// </summary>
    /// <returns>
    /// List of probability estimate values, where 1.0 is the highest and 0.0 is the lowest.
    /// </returns>
    public int endStrokeAndGetAllProbabilities(ref double[] p, ref double[] s)
    {
        int num_gestures = this.numberOfGestures();
        p = new double[num_gestures];
        s = new double[num_gestures];
        double[] pos = new double[3];
        double[] scale = new double[1];
        double[] dir0 = new double[3];
        double[] dir1 = new double[3];
        double[] dir2 = new double[3];
        return GestureRecognition_endStrokeAndGetAllProbabilitiesAndSimilarities(m_gro, p, s, num_gestures, pos, scale, dir0, dir1, dir2);
    }

    //                                                          ________________________________
    //_________________________________________________________/      numberOfGestures()
    /// <summary>
    /// Query the number of currently registered gestures.
    /// </summary>
    /// <returns>
    /// The number of gestures currently registered in the library.
    /// </returns>
    public int numberOfGestures()
    {
        return GestureRecognition_numberOfGestures(m_gro);
    }
    //                                                          ________________________________
    //_________________________________________________________/         deleteGesture()
    /// <summary>
    /// Delete currently registered gesture.
    /// </summary>
    /// <param name="index">ID of the gesture to delete.</param>
    /// <returns>
    /// False on failure, true on success.
    /// </returns>
    public bool deleteGesture(int index)
    {
        return GestureRecognition_deleteGesture(m_gro, index) != 0;
    }
    //                                                          ________________________________
    //_________________________________________________________/      deleteAllGestures()
    /// <summary>
    /// Delete all currently registered gestures.
    /// </summary>
    /// <returns>
    /// False on failure, true on success.
    /// </returns>
    public bool deleteAllGestures()
    {
        return GestureRecognition_deleteAllGestures(m_gro) != 0;
    }
    //                                                          ________________________________
    //_________________________________________________________/      createGesture()
    /// <summary>
    /// Create a new gesture.
    /// </summary>
    /// <param name="name">Name of the new gesture.</param>
    /// <returns>
    /// ID of the new gesture.
    /// </returns>
    public int createGesture(string name)
    {
        return GestureRecognition_createGesture(m_gro, name, IntPtr.Zero);
    }
    //                                                          ________________________________
    //_________________________________________________________/      recognitionScore()
    /// <summary>
    /// Get the current recognition performance
    /// (probabliity to recognize the correct gesture: a value between 0 and 1 where 0 is 0%
    /// and 1 is 100% correct recognition rate).
    /// </summary>
    /// <returns>
    /// The current recognition performance
    /// (probabliity to recognize the correct gesture: a value between 0 and 1 where 0 is 0%
    /// and 1 is 100% correct recognition rate).
    /// </returns>
    public double recognitionScore()
    {
        return GestureRecognition_recognitionScore(m_gro);
    }
    //                                                          ________________________________
    //_________________________________________________________/      getGestureName()
    /// <summary>
    /// Get the name associated to a gesture. 
    /// </summary>
    /// <param name="index">ID of the gesture to query.</param>
    /// <returns>
    /// Name of the gesture. On failure, an empty string is returned.
    /// </returns>
    public string getGestureName(int index)
    {
        int strlen = GestureRecognition_getGestureNameLength(m_gro, index);
        if (strlen <= 0)
        {
            return "";
        }
        StringBuilder sb = new StringBuilder(strlen + 1);
        GestureRecognition_copyGestureName(m_gro, index, sb, sb.Capacity);
        return sb.ToString();
    }
    //                                                          ________________________________
    //_________________________________________________________/   getGestureNumberOfSamples()
    /// <summary>
    /// Query the number of samples recorded for one gesture.
    /// </summary>
    /// <param name="index">ID of the gesture to query.</param>
    /// <returns>
    /// Number of samples recorded for this gesture or -1 if the gesture does not exist.
    /// </returns>
    public int getGestureNumberOfSamples(int index)
    {
        return GestureRecognition_getGestureNumberOfSamples(m_gro, index);
    }
    //                                                          ________________________________
    //_________________________________________________________/    getGestureSampleLength()
    /// <summary>
    /// Get the number of data points a sample has.
    /// </summary>
    /// <param name="gesture_index">The zero-based index (ID) of the gesture from where to retrieve the sample.</param>
    /// <param name="sample_index">The zero-based index(ID) of the sample to retrieve.</param>
    /// <param name="processed">Whether the raw data points should be retrieved (0) or the processed data points (1).</param>
    /// <returns>
    /// The number of data points on that sample.
    /// </returns>
    public int getGestureSampleLength(int gesture_index, int sample_index, int processed)
    {
        return GestureRecognition_getGestureSampleLength(m_gro, gesture_index, sample_index, processed);
    }
    //                                                          ________________________________
    //_________________________________________________________/    getGestureSampleStroke()
    /// <summary>
    /// Retrieve a sample stroke.
    /// </summary>
    /// <param name="gesture_index">The zero-based index (ID) of the gesture from where to retrieve the sample.</param>
    /// <param name="sample_index">The zero-based index(ID) of the sample to retrieve.</param>
    /// <param name="processed">Whether the raw data points should be retrieved (0) or the processed data points (1).</param>
    /// <param name="hmd_p">[OUT] A vector to receive the position of the HMD at the time of the stroke sample recording.</param>
    /// <param name="hmd_q">[OUT] A quaternion to receive the rotation of the HMD at the time of the stroke sample recording.</param>
    /// <param name="p">[OUT] A vector array to receive the positional data points of the stroke / recorded sample.</param>
    /// <param name="q">[OUT] A quaternion array to receive the rotational data points of the stroke / recorded sample.</param>
    /// <returns>
    /// The number of data points on that sample (ie. resulting length of p and q).
    /// </returns>
    public int getGestureSampleStroke(int gesture_index, int sample_index, int processed, ref Vector3 hmd_p, ref Quaternion hmd_q, ref Vector3[] p, ref Quaternion[] q)
    {
        int sample_length = this.getGestureSampleLength(gesture_index, sample_index, processed);
        if (sample_length == 0)
        {
            return 0;
        }
        double[] _hmd_p = new double[3];
        double[] _hmd_q = new double[4];
        double[] _p = new double[3 * sample_length];
        double[] _q = new double[4 * sample_length];
        int samples_written = GestureRecognition_getGestureSampleStroke(m_gro, gesture_index, sample_index, processed, _hmd_p, _hmd_q, _p, _q, sample_length);
        if (samples_written == 0)
        {
            return 0;
        }
        hmd_p.x = (float)_hmd_p[0];
        hmd_p.y = (float)_hmd_p[1];
        hmd_p.z = (float)_hmd_p[2];
        hmd_q.x = (float)_hmd_q[0];
        hmd_q.y = (float)_hmd_q[1];
        hmd_q.z = (float)_hmd_q[2];
        hmd_q.w = (float)_hmd_q[3];
        p = new Vector3[samples_written];
        q = new Quaternion[samples_written];
        for (int i = 0; i < samples_written; i++)
        {
            p[i].x = (float)_p[i * 3 + 0];
            p[i].y = (float)_p[i * 3 + 1];
            p[i].z = (float)_p[i * 3 + 2];
            q[i].x = (float)_q[i * 4 + 0];
            q[i].y = (float)_q[i * 4 + 1];
            q[i].z = (float)_q[i * 4 + 2];
            q[i].w = (float)_q[i * 4 + 3];
        }
        return samples_written;
    }
    //                                                          ________________________________
    //_________________________________________________________/      deleteGestureSample()
    /// <summary>
    /// Delete a gesture sample recording from the set.
    /// </summary>
    /// <param name="gesture_index">The zero-based index (ID) of the gesture from where to delete the sample.</param>
    /// <param name="sample_index">The zero-based index(ID) of the sample to delete.</param>
    /// <returns>
    /// True on success, false on failure.
    /// </returns>
    public bool deleteGestureSample(int gesture_index, int sample_index)
    {
        return GestureRecognition_deleteGestureSample(m_gro, gesture_index, sample_index) != 0;
    }
    //                                                          ________________________________
    //_________________________________________________________/    deleteAllGestureSamples()
    /// <summary>
    /// Delete all gesture sample recordings from the set.
    /// </summary>
    /// <param name="gesture_index">The zero-based index (ID) of the gesture from where to delete the sample.</param>
    /// <returns>
    /// True on success, false on failure.
    /// </returns>
    public bool deleteAllGestureSamples(int gesture_index)
    {
        return GestureRecognition_deleteAllGestureSamples(m_gro, gesture_index) != 0;
    }
    //                                                          ________________________________
    //_________________________________________________________/       setGestureName()
    /// <summary>
    /// Set the name associated with a gesture.
    /// </summary>
    /// <param name="index">ID of the gesture whose name to set.</param>
    /// <param name="name">The new name of the gesture.</param>
    /// <returns>
    /// True on success, false on failure.
    /// </returns>
    public bool setGestureName(int index, string name)
    {
        return GestureRecognition_setGestureName(m_gro, index, name) != 0;
    }
    //                                                          ________________________________
    //_________________________________________________________/      saveToFile()
    /// <summary>
    /// Save the current artificial intelligence to a file.
    /// </summary>
    /// <param name="path">File system path and filename where to save the file.</param>
    /// <returns>
    /// True on success, false on failure.
    /// </returns>
    public bool saveToFile(string path)
    {
        return GestureRecognition_saveToFile(m_gro, path) != 0;
    }
    //                                                          ________________________________
    //_________________________________________________________/    loadFromFile()
    /// <summary>
    /// Load a previously saved gesture recognition artificial intelligence from a file.
    /// </summary>
    /// <param name="path">File system path and filename from where to load.</param>
    /// <returns>
    /// True on success, false on failure.
    /// </returns>
    public bool loadFromFile(string path)
    {
        return GestureRecognition_loadFromFile(m_gro, path, null) != 0;
    }
    //                                                          ________________________________
    //_________________________________________________________/     loadFromBuffer()
    /// <summary>
    /// Load a previously saved gesture recognition artificial intelligence from a string buffer.
    /// </summary>
    /// <param name="buffer">The string buffer containing the artificial intelligence.</param>
    /// <returns>
    /// True on success, false on failure.
    /// </returns>
    public bool loadFromBuffer(string buffer)
    {
        return GestureRecognition_loadFromBuffer(m_gro, buffer, null) != 0;
    }
    //                                                          ________________________________
    //_________________________________________________________/      startTraining()
    /// <summary>
    /// Start the learning process.
    /// After calling this function the gesture recognition library will attempt to learn
    /// patterns in the previously recorded sample strokes in order to find commonalities
    /// and identify future gestures.
    /// </summary>
    /// <returns>
    /// False if starting the learning process failed, true if the learning process
    /// was successfully started.
    /// </returns>
    public bool startTraining()
    {
        return GestureRecognition_startTraining(m_gro) != 0;
    }
    //                                                          ________________________________
    //_________________________________________________________/       isTraining()
    /// <summary>
    /// Query whether the gesture recognition library is currently trying to learn
    /// gesture identification.
    /// </summary>
    /// <returns>
    /// False if the gesture recognition library is NOT currently in the learning process,
    /// true if it is learning.
    /// </returns>
    public bool isTraining()
    {
        return GestureRecognition_isTraining(m_gro) != 0;
    }
    //                                                          ________________________________
    //_________________________________________________________/       stopTraining()
    /// <summary>
    /// Stop the learning process.
    /// Due to the asynchronous nature of the learning process ("learning in background")
    /// it may take a moment before the training in the background is actually stopped.
    /// </summary>
    public void stopTraining()
    {
        GestureRecognition_stopTraining(m_gro);
    }
    //                                                          ________________________________
    //_________________________________________________________/     getMaxTrainingTime()
    /// <summary>
    /// Get the maximum time used for the learning process (in seconds).
    /// </summary>
    /// <returns>
    /// Maximum time to spend learning (in seconds).
    /// </returns>
    public int getMaxTrainingTime()
    {
        return GestureRecognition_getMaxTrainingTime(m_gro);
    }
    //                                                          ________________________________
    //_________________________________________________________/     setMaxTrainingTime()
    /// <summary>
    /// Set the maximum time used for the learning process (in seconds).
    /// </summary>
    /// <param name="t">Maximum time to spend learning (in seconds).</param>
    public void setMaxTrainingTime(int t)
    {
        GestureRecognition_setMaxTrainingTime(m_gro, t);
    }
    //                                                          ________________________________
    //_________________________________________________________/   setTrainingUpdateCallback()
    /// <summary>
    /// Set a function to be called during the training process.
    /// The function will be called whenever the gesture recognition library was able to
    /// increase its performance.
    /// The callback function will receive the current recognition performance
    /// (probabliity to recognize the correct gesture: a value between 0 and 1 where 0 is 0%
    /// and 1 is 100% correct recognition rate).
    /// </summary>
    /// <param name="cbf">The function to call when the recognition performance could be increased.</param>
    public void setTrainingUpdateCallback(TrainingCallbackFunction cbf)
    {
        GestureRecognition_setTrainingUpdateCallback(m_gro, cbf);
    }
    //                                                      ____________________________________
    //_____________________________________________________/ setTrainingUpdateCallbackMetadata()
    /// <summary>
    /// Set a pointer to a metadata object that will be provided to the
	/// TrainingUpdateCallbackFunction.
    /// </summary>
    /// <param name="metadata">The metadata pointer to provide to the TrainingUpdateCallbackFunction.</param>
    public void setTrainingUpdateCallbackMetadata(IntPtr metadata)
    {
        GestureRecognition_setTrainingUpdateCallbackMetadata(m_gro, metadata);
    }
    //                                                          ________________________________
    //_________________________________________________________/   setTrainingFinishCallback()
    /// <summary>
    /// Set a callback function to be called when the learning process finishes.
    /// The callback function will receive the final recognition performance
    /// (probabliity to recognize the correct gesture: a value between 0 and 1 where 0 is 0%
    /// and 1 is 100% correct recognition rate).
    /// </summary>
    /// <param name="cbf">The function to call when the learning process is finished.</param>
    public void setTrainingFinishCallback(TrainingCallbackFunction cbf)
    {
        GestureRecognition_setTrainingFinishCallback(m_gro, cbf);
    }
    //                                                      ____________________________________
    //_____________________________________________________/ setTrainingFinishCallbackMetadata()
    /// <summary>
    /// Set a pointer to a metadata object that will be provided to the
	/// TrainingFinishCallbackFunction.
    /// </summary>
    /// <param name="metadata">The metadata pointer to provide to the TrainingFinishCallbackFunction.</param>
    public void setTrainingFinishCallbackMetadata(IntPtr metadata)
    {
        GestureRecognition_setTrainingFinishCallbackMetadata(m_gro, metadata);
    }


    // ----------------------------------------------------------------------------------------------------------
    // Internal wrapper functions to the plug-in
    // ----------------------------------------------------------------------------------------------------------
    public delegate IntPtr MetadataCreatorFunction();
    public delegate void TrainingCallbackFunction(double performace, IntPtr metadata);

    public const string libfile = "gesturerecognition";

    [DllImport(libfile, EntryPoint = "GestureRecognition_create", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GestureRecognition_create();
    [DllImport(libfile, EntryPoint = "GestureRecognition_delete", CallingConvention = CallingConvention.Cdecl)]
    public static extern void GestureRecognition_delete(IntPtr gro);
    [DllImport(libfile, EntryPoint = "GestureRecognition_startStroke", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_startStroke(IntPtr gro, double[] hmd_p, double[] hmd_q, int record_as_sample);
    [DllImport(libfile, EntryPoint = "GestureRecognition_startStrokeM", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_startStrokeM(IntPtr gro, double[,] hmd, int record_as_sample);
    [DllImport(libfile, EntryPoint = "GestureRecognition_contdStroke", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_contdStroke(IntPtr gro, double[] p, double[] q);
    [DllImport(libfile, EntryPoint = "GestureRecognition_contdStrokeM", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_contdStrokeM(IntPtr gro, double[,] m);
    [DllImport(libfile, EntryPoint = "GestureRecognition_endStroke", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_endStroke(IntPtr gro, double[] pos, double[] scale, double[] dir0, double[] dir1, double[] dir2);
    [DllImport(libfile, EntryPoint = "GestureRecognition_endStrokeAndGetAllProbabilities", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_endStrokeAndGetAllProbabilities(IntPtr gro, double[] p, int n, double[] pos, double[] scale, double[] dir0, double[] dir1, double[] dir2);
    [DllImport(libfile, EntryPoint = "GestureRecognition_endStrokeAndGetSimilarity", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_endStrokeAndGetSimilarity(IntPtr gro, double[] similarity, double[] pos, double[] scale, double[] dir0, double[] dir1, double[] dir2);
    [DllImport(libfile, EntryPoint = "GestureRecognition_endStrokeAndGetAllProbabilitiesAndSimilarities", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_endStrokeAndGetAllProbabilitiesAndSimilarities(IntPtr gro, double[] p, double[] s, int n, double[] pos, double[] scale, double[] dir0, double[] dir1, double[] dir2); //!< End the stroke and get gesture probabilities and similarity values.
    [DllImport(libfile, EntryPoint = "GestureRecognition_numberOfGestures", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_numberOfGestures(IntPtr gro);
    [DllImport(libfile, EntryPoint = "GestureRecognition_deleteGesture", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_deleteGesture(IntPtr gro, int index);
    [DllImport(libfile, EntryPoint = "GestureRecognition_deleteAllGestures", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_deleteAllGestures(IntPtr gro);
    [DllImport(libfile, EntryPoint = "GestureRecognition_createGesture", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_createGesture(IntPtr gro, string name, IntPtr metadata);
    [DllImport(libfile, EntryPoint = "GestureRecognition_recognitionScore", CallingConvention = CallingConvention.Cdecl)]
    public static extern double GestureRecognition_recognitionScore(IntPtr gro);
    // [DllImport(libfile, EntryPoint = "GestureRecognition_getGestureName", CallingConvention = CallingConvention.Cdecl)]
    // public static extern string GestureRecognition_getGestureName(IntPtr gro, int index);
    [DllImport(libfile, EntryPoint = "GestureRecognition_getGestureNameLength", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_getGestureNameLength(IntPtr gro, int index);
    [DllImport(libfile, EntryPoint = "GestureRecognition_copyGestureName", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_copyGestureName(IntPtr gro, int index, StringBuilder buf, int buflen);
    [DllImport(libfile, EntryPoint = "GestureRecognition_getGestureMetadata", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr GestureRecognition_getGestureMetadata(IntPtr gro, int index);
    [DllImport(libfile, EntryPoint = "GestureRecognition_getGestureNumberOfSamples", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_getGestureNumberOfSamples(IntPtr gro, int index);
    [DllImport(libfile, EntryPoint = "GestureRecognition_getGestureSampleLength", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_getGestureSampleLength(IntPtr gro, int gesture_index, int sample_index, int processed);
    [DllImport(libfile, EntryPoint = "GestureRecognition_getGestureSampleStroke", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_getGestureSampleStroke(IntPtr gro, int gesture_index, int sample_index, int processed, double[] hmd_p, double[] hmd_q, double[] p, double[] q, int stroke_buf_size);
    [DllImport(libfile, EntryPoint = "GestureRecognition_deleteGestureSample", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_deleteGestureSample(IntPtr gro, int gesture_index, int sample_index);
    [DllImport(libfile, EntryPoint = "GestureRecognition_deleteAllGestureSamples", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_deleteAllGestureSamples(IntPtr gro, int gesture_index);
    [DllImport(libfile, EntryPoint = "GestureRecognition_setGestureName", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_setGestureName(IntPtr gro, int index, string name);
    [DllImport(libfile, EntryPoint = "GestureRecognition_setGestureMetadata", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_setGestureMetadata(IntPtr gro, int index, IntPtr metadata);
    [DllImport(libfile, EntryPoint = "GestureRecognition_saveToFile", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_saveToFile(IntPtr gro, string path);
    [DllImport(libfile, EntryPoint = "GestureRecognition_loadFromFile", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_loadFromFile(IntPtr gro, string path, MetadataCreatorFunction createMetadata);
    [DllImport(libfile, EntryPoint = "GestureRecognition_loadFromBuffer", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_loadFromBuffer(IntPtr gro, string buffer, MetadataCreatorFunction createMetadata);
    [DllImport(libfile, EntryPoint = "GestureRecognition_startTraining", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_startTraining(IntPtr gro);
    [DllImport(libfile, EntryPoint = "GestureRecognition_isTraining", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_isTraining(IntPtr gro);
    [DllImport(libfile, EntryPoint = "GestureRecognition_stopTraining", CallingConvention = CallingConvention.Cdecl)]
    public static extern void GestureRecognition_stopTraining(IntPtr gro);
    [DllImport(libfile, EntryPoint = "GestureRecognition_getMaxTrainingTime", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_getMaxTrainingTime(IntPtr gro);
    [DllImport(libfile, EntryPoint = "GestureRecognition_setMaxTrainingTime", CallingConvention = CallingConvention.Cdecl)]
    public static extern void GestureRecognition_setMaxTrainingTime(IntPtr gro, int t);
    [DllImport(libfile, EntryPoint = "GestureRecognition_setTrainingUpdateCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern void GestureRecognition_setTrainingUpdateCallback(IntPtr gro, TrainingCallbackFunction cbf);
    [DllImport(libfile, EntryPoint = "GestureRecognition_setTrainingFinishCallback", CallingConvention = CallingConvention.Cdecl)]
    public static extern void GestureRecognition_setTrainingFinishCallback(IntPtr gro, TrainingCallbackFunction cbf);
    [DllImport(libfile, EntryPoint = "GestureRecognition_setTrainingUpdateCallbackMetadata", CallingConvention = CallingConvention.Cdecl)]
    public static extern void GestureRecognition_setTrainingUpdateCallbackMetadata(IntPtr gro, IntPtr metadata);
    [DllImport(libfile, EntryPoint = "GestureRecognition_setTrainingFinishCallbackMetadata", CallingConvention = CallingConvention.Cdecl)]
    public static extern void GestureRecognition_setTrainingFinishCallbackMetadata(IntPtr gro, IntPtr metadata);
    [DllImport(libfile, EntryPoint = "GestureRecognition_getIgnoreHeadRotationX", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_getIgnoreHeadRotationX(IntPtr gro);
    [DllImport(libfile, EntryPoint = "GestureRecognition_setIgnoreHeadRotationX", CallingConvention = CallingConvention.Cdecl)]
    public static extern void GestureRecognition_setIgnoreHeadRotationX(IntPtr gro, int on_off);
    [DllImport(libfile, EntryPoint = "GestureRecognition_getIgnoreHeadRotationY", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_getIgnoreHeadRotationY(IntPtr gro);
    [DllImport(libfile, EntryPoint = "GestureRecognition_setIgnoreHeadRotationY", CallingConvention = CallingConvention.Cdecl)]
    public static extern void GestureRecognition_setIgnoreHeadRotationY(IntPtr gro, int on_off);
    [DllImport(libfile, EntryPoint = "GestureRecognition_getIgnoreHeadRotationZ", CallingConvention = CallingConvention.Cdecl)]
    public static extern int GestureRecognition_getIgnoreHeadRotationZ(IntPtr gro);
    [DllImport(libfile, EntryPoint = "GestureRecognition_setIgnoreHeadRotationZ", CallingConvention = CallingConvention.Cdecl)]
    public static extern void GestureRecognition_setIgnoreHeadRotationZ(IntPtr gro, int on_off);

    private IntPtr m_gro;
}
