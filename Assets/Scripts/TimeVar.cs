using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
    
    internal struct TimeVar
    {
        internal static bool getTimeDone; //детекс окончания корутины и подключения
        internal static DateTime serverTime; //общественный дейт
        static string rawString;//жисон светится тут и там, вынесли на уровень повыше
        internal static rawData currentData;//локальный кеш

        internal static IEnumerator LoadTimeFromServer(string url, GameObject[] objectForClose, GameObject refreshWindow)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);//гет сайту
            yield return request.SendWebRequest();//работаем
            if (request.result == UnityWebRequest.Result.Success)
            {
                rawString = request.downloadHandler.text;//успех? пишем текст
                serverTime = GetTimeJson();//взяли из разметки время
                getTimeDone = true;//получили буль
            }
            else
            {
                Debug.LogErrorFormat("error request [{0}, {1}]", url, request.error);
                rawString = null;//ошибка - не пишем текст
                getTimeDone = false;
            }
            ErrorBeh(getTimeDone, objectForClose, refreshWindow);
            request.Dispose();//чистимся
        }

        internal static void ErrorBeh(bool state, GameObject[] objectsForClose, GameObject refreshWindow){
                for (int i = 0; i < objectsForClose.Length; i++){
                    objectsForClose[i].SetActive(state);
                }
                refreshWindow.SetActive(!state);
                Debug.Log("Done");
        }

        internal static IEnumerator CooldownRefsresh(int timer, Text timerText, Button buttonForClose){
        buttonForClose.interactable = false;
        while (timer > 0){
            timerText.text = $"in {timer} seconds";
            --timer;
            yield return new WaitForSeconds(1f);
        }
        buttonForClose.interactable = true;
        timerText.text = $"maybe now?";
        yield return null;
    }

        static DateTime GetTimeJson()
        {
            currentData = JsonUtility.FromJson<rawData>(rawString);//десер сюда
            DateTime currentTime = DateTime.Parse(currentData.datetime);//парсим нужную строку
            return currentTime; //вернули
        }
    }

    [Serializable]//разметка даты
    public struct rawData{
        public string utc_offset;
        public string timezone;
        public string day_of_week;
        public string day_of_year;
        public string datetime;
        public string utc_datetime;
        public string unixtime;
        public string raw_offset;
        public string week_number;
        public string dst;
        public string abbreviation;
        public string dst_offset;
        public string dst_from;
        public string dst_until;
        public string client_ip;
    }

