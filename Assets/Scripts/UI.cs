using System;
using UnityEngine.UI;

internal struct UI
{
    internal static void GetTextValue(DateTime dateTime, Text hour, Text minute, Text sec){
        hour.text = dateTime.Hour.ToString();
        minute.text = dateTime.Minute.ToString();
        sec.text = dateTime.Second.ToString();
    }

    internal static void EditorEnabled(InputField hour, InputField minute, InputField second, Button button){
        ChangeClock(hour,minute,second,button,true,"Enter & submit");
    }

    internal static DateTime SubmitValue(DateTime instanceTime, InputField hour, InputField minute, InputField second, Button button){
        DateTime submitTime = new DateTime(instanceTime.Year,instanceTime.Month,instanceTime.Day,Math.Clamp(System.Convert.ToInt32(hour.text),0,23),Math.Clamp(System.Convert.ToInt32(minute.text),0,59),Math.Clamp(System.Convert.ToInt32(second.text),0,59));
        instanceTime = submitTime;
        ChangeClock(hour,minute,second,button,false,"Get your time!");
        return instanceTime;
    }

    static void ChangeClock(InputField hour, InputField minute, InputField second, Button button, bool enabled, string banner){
        hour.transform.gameObject.SetActive(enabled);
        minute.transform.gameObject.SetActive(enabled);
        second.transform.gameObject.SetActive(enabled);
        button.transform.GetComponentInChildren<Text>().text = banner;
    }

    internal static void ResetBehaviour(Button button, bool state){
        button.transform.gameObject.SetActive(state);
    }

    internal static void ClearInputField(InputField hour, InputField minute, InputField second){
        //очищаем текст в полях. Можно и циклом было
        hour.text = null;
        minute.text = null;
        second.text = null;
    }
    
}
