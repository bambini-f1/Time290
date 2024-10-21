using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

internal class Observer : MonoBehaviour
{
    DateTime serverTime, instanceTime;//серверное и наше время 
    #region InspectorFields
    [Header("ArrowTransform")]
    [SerializeField] 
    Transform hour; 
    [SerializeField] Transform minute, sec;//ссылки на трансформ
    
    [Header("Clock text")]
    [SerializeField]
    Text hourText; 
    [SerializeField]
    Text minuteText, secText;//ссылки на текст часов
    [SerializeField]
    Text utcZone, timeZone, timerRefreshField;//ссылки на инфо-поля

    [Header("Editor field")]
    [SerializeField]
    InputField hourField; 
    [SerializeField]
    InputField minuteField, secField;//ссылки на поля для ввода
    
    [Header("Others")]
    [SerializeField]
    float CheckDelay = 3600f;//доступна для инспектора, если нужны проверки чаще
    [SerializeField]
    int timerRefresh = 60;
    [SerializeField]
    string webTarget = "worldtimeapi.org/api/ip";

    [SerializeField]
    Button editorOnButton, resetButton, errorButton;//кнопка редактора и сброса

    [SerializeField]
    GameObject[] objectsForClose;
     [SerializeField]
    GameObject refreshWindow;

    [SerializeField]
    bool CheckingServer = true;//базово сверяем время (оттуда же и берем стратовое значение)
    #endregion
    
    void Awake()
    {
        PreloadData();//сначала ждем гет с сервера
    }

    void Start() 
    {
        //запускаем корутиы проверки и отсчета. В первом выполнении берем серверное время
        StartCoroutine(StartClock()); //вынесено внизы
        //подписываем кнопки
        editorOnButton.onClick.AddListener(EditorOn);
        resetButton.onClick.AddListener(ResetTime);
        errorButton.onClick.AddListener(ErrorReset);

    }

    IEnumerator CompareServerSetInfo()
    {
        while (true && CheckingServer)
        {
            instanceTime = instanceTime != serverTime ? serverTime : instanceTime; //корректируем по условию
            utcZone.text = TimeVar.currentData.utc_offset;
            timeZone.text = TimeVar.currentData.timezone;
            yield return new WaitForSeconds(CheckDelay);//задержка до следующей проверки
        }
    }

    IEnumerator PlayTime()
    {
        while (true)
        {
            instanceTime = instanceTime.AddSeconds(1);//добавляем секунду
            UI.GetTextValue(instanceTime, hourText, minuteText, secText);//записываем в UI значения
            ClockDirections.MoveArrow(instanceTime, hour, minute, sec);//здесь двигаем стрелки
            yield return new WaitForSeconds(1f);//ежесекундно
        }
    }

    void EditorOn(){
        UI.EditorEnabled(hourField, minuteField, secField, editorOnButton);//добавляем поля для ввода и меняем баннер
        StopAllCoroutines();//останавливаем корутины
        TimeVar.getTimeDone = false;
        editorOnButton.onClick.RemoveListener(EditorOn);
        editorOnButton.onClick.AddListener(SubmitEditor);//меняем подписку кнопки
    }

    void SubmitEditor(){
        instanceTime = UI.SubmitValue(instanceTime, hourField, minuteField, secField, editorOnButton);//присваиваем новое время
        StartCoroutine(PlayTime());//запускаем новый отсчет
        CheckingServer = false;//защищаем от случайно корутины
        UI.ResetBehaviour(resetButton,true);//включаем кнопку сброса
        UI.ClearInputField(hourField, minuteField, secField);//чистим для повторонго ввода
        editorOnButton.onClick.RemoveListener(SubmitEditor);
        editorOnButton.onClick.AddListener(EditorOn);//возвращаем стратовую подписку
    }

    void ResetTime(){
        StopAllCoroutines();//останавливаем все
        PreloadData();
        UI.ResetBehaviour(resetButton,false);//выключаем кнопку сброса
        StartCoroutine(StartClock());//снова запускаем часы
    }

    IEnumerator StartClock(){
        while (!TimeVar.getTimeDone)//пока нет времени, нет старта
        {
            yield return new WaitForSeconds(0.1f);
        }
        CheckingServer = true;//снимаем буль
        serverTime = TimeVar.serverTime;
        StartCoroutine(CompareServerSetInfo());//запускаем корутины
        StartCoroutine(PlayTime());
        StopCoroutine(StartClock());
        yield return null; //сделали
    }

    void PreloadData(){
        StartCoroutine(TimeVar.LoadTimeFromServer(webTarget, objectsForClose, refreshWindow));//получаем в разметку данные с сервера
    }

    void ErrorReset(){
        ResetTime();
        StartCoroutine(TimeVar.CooldownRefsresh(timerRefresh, timerRefreshField, errorButton));
    }
}
