using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

internal class GameManager : MonoBehaviour
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

    [Header("Editor field")]
    [SerializeField]
    InputField hourField; 
    [SerializeField]
    InputField minuteField, secField;//ссылки на поля для ввода
    
    [Header("Others")]
    [SerializeField]
    float CheckDelay = 3600f;//доступна для инспектора, если нужны проверки чаще

    [SerializeField]
    Button editorOnButton, resetButton;//кнопка редактора и сброса

    [SerializeField]
    bool CheckingServer = true;//базово сверяем время (оттуда же и берем стратовое значение)
    #endregion
    
    void Awake()
    {
        //запускаем корутиы проверки и отсчета. В первом выполнении берем серверное время
        StartClock(); //вынесено внизы
    }

    private void Start() 
    {
        //подписываем кнопки
        editorOnButton.onClick.AddListener(EditorOn);
        resetButton.onClick.AddListener(ResetTime);
    }

    IEnumerator CheckServer()
    {
        while (true && CheckingServer)
        {
            serverTime = TimeVar.GetNetworkTime();//получаем из структа время переведенное в локальный пояс
            instanceTime = instanceTime != serverTime ? serverTime : instanceTime; //корректируем по условию
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
        editorOnButton.onClick.RemoveListener(EditorOn);
        editorOnButton.onClick.AddListener(SubmitEditor);//меняем подписку кнопки
    }

    void SubmitEditor(){
        instanceTime = UI.SubmitValue(instanceTime, hourField, minuteField, secField, editorOnButton);//присваиваем новое время
        StartCoroutine(PlayTime());//запускаем новый отсчет
        CheckingServer = false;//защищаем от случайно корутины
        UI.ResetBehaviour(resetButton,true);//включаем кнопку сброса
        editorOnButton.onClick.RemoveListener(SubmitEditor);
        editorOnButton.onClick.AddListener(EditorOn);//возвращаем стратовую подписку
    }

    void ResetTime(){
        StopAllCoroutines();//останавливаем все
        UI.ResetBehaviour(resetButton,false);//выключаем кнопку сброса
        StartClock();//снова запускаем часы
    }

    void StartClock(){
        CheckingServer = true;//снимаем буль
        StartCoroutine(CheckServer());//запускаем корутины
        StartCoroutine(PlayTime());
    }
}
