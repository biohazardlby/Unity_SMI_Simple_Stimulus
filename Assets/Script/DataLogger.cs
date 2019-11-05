using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SMI;

public class DataLogger
{
    Experiment_Ctrl experiment_ctrl;
    public InputStringPair[] usrTags;
    public string usr_name;
    public bool IsLogging = true;

    /// <summary>
    /// Constructor
    ///     The datalogger should be constructed by Experiment_Ctrl
    /// </summary>
    /// <param name="ctrl"></param>
    public DataLogger(Experiment_Ctrl ctrl)
    {
        experiment_ctrl = ctrl;
    }
    StreamWriter writer;

    /// <summary>
    /// initiate writing, including creating directory and cvs file
    /// </summary>
    public void init_writing()
    {
        //create folders and cvs file
        string folder_path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) + "/experiment_output";
        string date_time_info = System.DateTime.Now.ToString("MM-dd-yyyy");

        if (usr_name != "")
        {
            folder_path += "/" + usr_name;
        }
        folder_path += "/" + date_time_info;

        Directory.CreateDirectory(folder_path);
        string file_name = System.DateTime.Now.ToString("HH-mm-ss") + ".cvs";
        string file_path = folder_path + "/" + file_name;

        //connect Streamwriter to cvs file
        writer = new StreamWriter(file_path, false);
        //write header
        string header = "SMI_timestamp" + "," +
                        "Unity_timestamp" + "," +
                        "head_pos_x" + "," +
                        "head_pos_y" + "," +
                        "head_pos_z" + "," +
                        "head_rot_x" + "," +
                        "head_rot_y" + "," +
                        "head_rot_z" + "," +
                        "head_rot_w" + "," +
                        "gaze_l_x" + "," +
                        "gaze_l_y" + "," +
                        "gaze_l_z" + "," +
                        "gaze_r_x" + "," +
                        "gaze_r_y" + "," +
                        "gaze_r_z" + "," +
                        "session_idx" + "," +
                        "stimulus_deg"
                        ;
                        
        foreach (InputStringPair x in usrTags)
        {
            header += "," + x.value;
        }
        writer.WriteLine(header);
    }
    /// <summary>
    /// Get new data from experiment control
    /// </summary>
    /// <returns></returns>
    string get_new_data()
    {
        Transform camTrans = experiment_ctrl.cam.transform;
        Vector3 gaze_l = experiment_ctrl.gaze_l;
        Vector3 gaze_r = experiment_ctrl.gaze_r;
        string res = "";
        res += experiment_ctrl.SMI_timestamp + "," +
                Time.realtimeSinceStartup + "," +
                camTrans.position.x + "," +
                camTrans.position.y + "," +
                camTrans.position.z + "," +
                camTrans.rotation.x + "," +
                camTrans.rotation.y + "," +
                camTrans.rotation.z + "," +
                camTrans.rotation.w + "," +
                gaze_l.x + "," +
                gaze_l.y + "," +
                gaze_l.z + "," +
                gaze_r.x + "," +
                gaze_r.y + "," +
                gaze_r.z + "," +
                experiment_ctrl.session_num + "," +
                experiment_ctrl.stimulus_cur_rot
            ;
        foreach (bool b in experiment_ctrl.usrTagBools)
        {
            res += "," +  b.ToString();
        }
        return res;
    }
    /// <summary>
    /// write data to cvs file
    /// </summary>
    public void writeData()
    {
        writer.WriteLine(get_new_data());
    }
    /// <summary>
    /// close writestream
    /// </summary>
    public void closeLogger()
    {
        writer.Close();
    }
}
