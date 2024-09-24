using System;
using Unity.VisualScripting;
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

    internal static DateTime SubmitValue(DateTime instanceTime, InputField hourField, InputField minuteField, InputField secondField, Button button){
        int year = instanceTime.Year;
        int month = instanceTime.Month;
        int day = instanceTime.Day;
        int hour = hourField.text.NullIfEmpty() == null? instanceTime.Hour : Math.Clamp(Convert.ToInt32(hourField.text),0,23);
        int minute = minuteField.text.NullIfEmpty() == null ? instanceTime.Minute : Math.Clamp(Convert.ToInt32(minuteField.text),0,59);
        int second = secondField.text.NullIfEmpty() == null ? instanceTime.Second : Math.Clamp(Convert.ToInt32(secondField.text),0,59);
        DateTime ?submitTime = new DateTime(year, month, day, hour, minute, second);
        instanceTime = (DateTime)submitTime;
        ChangeClock(hourField,minuteField,secondField,button,false,"Set your time!");
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
