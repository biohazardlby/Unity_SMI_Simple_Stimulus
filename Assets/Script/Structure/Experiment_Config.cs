using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class Experiment_Config : MonoBehaviour
{
    public GameObject SMI_Prefab;

     bool Instantiate_Gaze_Cursor = true;
     bool Enable_SMI_Notification = true;
     bool Is_Simulation_Mode_Active = true;

    public GameObject Instantiate_SMI()
    {
        try
        {
            Instantiate_Gaze_Cursor = GameObject.Find("Instantiate_Gaze_Cursor").GetComponent<Toggle>().isOn;
            Enable_SMI_Notification = GameObject.Find("Enable_SMI_Notification").GetComponent<Toggle>().isOn;
            Is_Simulation_Mode_Active = GameObject.Find("Is_Simulation_Mode_Active").GetComponent<Toggle>().isOn;
        }
        catch (System.Exception err)
        {
            Debug.LogError("Can't find canvas, " + err.Message);
            return null;
        }
        SMI.SMIEyeTrackingUnity SMI_Component = SMI_Prefab.GetComponent<SMI.SMIEyeTrackingUnity>();
        SMI_Component.enableSMINotificationInVR = Enable_SMI_Notification;
        SMI_Component.isSimulationModeActive = Is_Simulation_Mode_Active;
        Experiment_Ctrl ctrl = GetComponent<Experiment_Ctrl>();
        ctrl.user_name = GameObject.Find("user_name").GetComponent<Text>().text;
        ctrl.sample_rate = float.Parse(GameObject.Find("Sample_Rate").GetComponent<Text>().text);
        ctrl.stimulus_interval = float.Parse(GameObject.Find("Stimulus_Interval").GetComponent<Text>().text);
        ctrl.show_gaze_cursor = true;
        GameObject SMI_Instance = GameObject.Instantiate(SMI_Prefab);
        SMI_Instance.GetComponent<Camera>().backgroundColor = Color.gray;

        return SMI_Instance;
    }
}