using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Experiment_Ctrl : MonoBehaviour
{
    public GameObject SMI_Prefab;

    float stimulus_eye_dist = 57.2285103656461f;
    public float sample_rate;
    public float stimulus_interval;
    public string user_name = "";
    public bool show_gaze_cursor;

    public GameObject cam;

    GameObject stimulus_prefab;
    GameObject stimulus;

    GameObject eyeball_prefab;
    GameObject leftEye, rightEye;

    public float stimulus_cur_rot;
    float[] stimulus_rotation_input = new float[] { -40, -30, -20, -10, 10, 20, 30, 40 };
    int[] stimulus_rotation_weights = new int[] { 12, 12, 12, 12, 12, 12, 12, 12 };
    public float[] stimulus_rotations_arr;

    public Vector3 gaze_l;
    public Vector3 gaze_r;
    public ulong SMI_timestamp;
    public int session_num = 0;

    public int customTagSize;
    public InputStringPair[] usrTags;
    public bool[] usrTagBools;

    DataLogger logger;
    GameObject UI_Cam;
    GameObject gaze_cursor;

    Ctrl_STATUS ctrl_status;
    void Start()
    {
        Start_Configuration();
    }

    void Start_Configuration()
    {
        ctrl_status = Ctrl_STATUS.CONFIG;
        UI_Cam = new GameObject("UI_Camera");
        UI_Cam.AddComponent<Camera>();
        UI_Cam.GetComponent<Camera>().backgroundColor = Color.black;
    }

    public void Finish_Configuration()
    {
        cam = GetComponent<Experiment_Config>().Instantiate_SMI();
        GameObject.Find("Menu").SetActive(false);
        GameObject.Destroy(UI_Cam);
        Start_Experiment();
        ctrl_status = Ctrl_STATUS.WAITING;
    }

    void Start_Experiment()
    {
        load_stimulus_prefab();
        load_eyeball_prefab();
        Init_Stimulus();
        Init_Gaze_Cursor();

        usrTagBools = new bool[usrTags.Length];
        logger = new DataLogger(this);
        logger.usrTags = usrTags;
        logger.usr_name = user_name;
        logger.init_writing();

        Init_Trial();

        InvokeRepeating("Update_Data", 0, 1 / sample_rate);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && ctrl_status == Ctrl_STATUS.WAITING)
        {
            Start_Trial();
            ctrl_status = Ctrl_STATUS.RUNNING;
            InvokeRepeating("Write_Data", 0, 1 / sample_rate);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();

        }
        for (int i = 0; i < usrTags.Length; i++)
        {
            if (Input.GetKey(usrTags[i].input))
            {
                usrTagBools[i] = true;
            }
            else
            {
                usrTagBools[i] = false;
            }
        }
        draw_gaze_ray();
    }


    void Init_Gaze_Cursor()
    {
        gaze_cursor = GameObject.Instantiate(Resources.Load<GameObject>("SMI_GazePoint"));
        gaze_cursor.name = "SMI_Gaze_Sprite_Prefab";
    }
    void Update_Gaze_Cursor()
    {

    }
    void Start_Trial()
    {
        //display play button
        Toggle_PlayButton();
        //play audio
        AudioSource audio = GetComponent<AudioSource>();

        audio.Play();
        InvokeRepeating("Next_Stimulus", 2, stimulus_interval);
    }
    void Toggle_PlayButton()
    {
        try
        {
            Image playButton = GameObject.Find("PlayButton").GetComponent<Image>();
            playButton.enabled = true;
            Invoke("Disable_PlayButton", 1);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("No playbutton found, will only play sound, " + e.Message);
        }
    }
    void Disable_PlayButton()
    {
        Image playButton = GameObject.Find("PlayButton").GetComponent<Image>();
        if (playButton != null)
        {
            playButton.enabled = false;
        }
    }
    /// <summary>
    /// Update SMI data and call logger to writer data to files
    /// </summary>
    void Update_Data()
    {
        Update_SMI_Data();
    }
    void Write_Data()
    {
        logger.writeData();
    }

    /// <summary>
    /// load stimulus prefab from resources file
    /// </summary>
    void load_stimulus_prefab()
    {
        //Load stimulus prefab
        stimulus_prefab = (GameObject)Resources.Load("Prefabs/Stimulus");
        if (stimulus_prefab == null)
        {
            Debug.LogError("No stimulus prefab found, check Assets/Resources/Prefabs/Stimulus.prefab");
            Application.Quit();
        }
    }
    /// <summary>
    /// load eyeball prefab from resources file
    /// </summary>
    void load_eyeball_prefab()
    {
        eyeball_prefab = Resources.Load("Prefabs/eyeball") as GameObject;
        if (eyeball_prefab == null)
        {
            Debug.LogError(
                "No eyeball prefabs found, check Asset/Resources/Prefabs/eyeball.prefab"
                );
            Application.Quit();
        }
        leftEye = GameObject.Instantiate(eyeball_prefab);
        leftEye.name = "leftEye";
        rightEye = GameObject.Instantiate(eyeball_prefab);
        rightEye.name = "rightEye";
    }
    /// <summary>
    /// update all data from SMI instance
    /// </summary>
    void Update_SMI_Data()
    {
        var leftEyePos = SMI.SMIEyeTrackingUnity.Instance.smi_GetLeftGazeBase();
        var rightEyePos = SMI.SMIEyeTrackingUnity.Instance.smi_GetRightGazeBase();
        if (SMI.SMIEyeTrackingUnity.smi_IsValid(leftEyePos))
        {
            leftEye.transform.position = leftEyePos + cam.transform.position;
        }

        if (SMI.SMIEyeTrackingUnity.smi_IsValid(rightEyePos))
        {
            rightEye.transform.position = rightEyePos + cam.transform.position;
        }

        var SMI_leftEyeGaze = SMI.SMIEyeTrackingUnity.Instance.smi_GetLeftGazeDirection();
        var SMI_rightEyeGaze = SMI.SMIEyeTrackingUnity.Instance.smi_GetRightGazeDirection();
        if (SMI.SMIEyeTrackingUnity.smi_IsValid(SMI_leftEyeGaze))
        {
            gaze_l = SMI_leftEyeGaze;
        }
        else
        {
            gaze_l.Set(-1, -1, -1);
        }
        if (SMI.SMIEyeTrackingUnity.smi_IsValid(SMI_rightEyeGaze))
        {
            gaze_r = SMI_rightEyeGaze;
        }
        else
        {
            gaze_r.Set(-1, -1, -1);
        }
        SMI_timestamp = SMI.SMIEyeTrackingUnity.Instance.smi_GetTimeStamp();
    }
    /// <summary>
    /// Instantiate stimulus at 0 degree
    /// </summary>
    void Init_Stimulus()
    {
        Transform camTrans = cam.transform;
        Vector3 targetPos = camTrans.position + camTrans.forward * stimulus_eye_dist;
        stimulus = GameObject.Instantiate(stimulus_prefab, camTrans);
        stimulus.transform.position = targetPos;
    }
    /// <summary>
    /// Generate random trial
    /// </summary>
    void Init_Trial()
    {
        stimulus_rotations_arr = Random_Trial_Generator.generate(stimulus_rotation_input, stimulus_rotation_weights, -30, 30);
        string s = "";
        foreach (float x in stimulus_rotations_arr)
        {
            s += x + ",";
        }
        Debug.Log(s);
    }
    /// <summary>
    /// Proceed to next stimulus in trial
    /// </summary>
    void Next_Stimulus()
    {
        if (session_num < stimulus_rotations_arr.Length)
        {
            change_stimulus_location(stimulus_cur_rot + stimulus_rotations_arr[session_num]);
            stimulus_cur_rot += stimulus_rotations_arr[session_num];
            session_num++;
        }
        else
        {
            CancelInvoke("Next_Stimulus");
        }
    }
    /// <summary>
    /// Change stimulus location in degree relative to head
    /// </summary>
    /// <param name="degree"></param>
    void change_stimulus_location(float degree)
    {
        stimulus.transform.localRotation = Quaternion.identity;
        Transform camTrans = cam.transform;
        Vector3 eyeball_Center = (leftEye.transform.position + rightEye.transform.position) / 2;

        Vector3 targetPos = eyeball_Center + camTrans.forward * stimulus_eye_dist;

        stimulus.transform.position = targetPos;
        stimulus.transform.RotateAround(eyeball_Center, camTrans.up, degree);
    }

    /// <summary>
    /// Debug: draw gaze ray for debug
    /// </summary>
    void draw_gaze_ray()
    {
        //Debug.DrawRay(leftEye.transform.position, cam.transform.rotation * gaze_l * 50,Color.red);
        //Debug.DrawRay(rightEye.transform.position, cam.transform.rotation * gaze_r * 50, Color.blue);
    }

    private void OnApplicationQuit()
    {
        logger.closeLogger();
    }
}

enum Ctrl_STATUS
{
    CONFIG, WAITING, RUNNING
}