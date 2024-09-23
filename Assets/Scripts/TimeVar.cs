using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
    
    internal struct TimeVar
    {
        static string rawString;//жисон светится тут и там, вынесли на уровень повыше
        internal static DateTime GetNetworkTime()
        {
            rawData currentData;//локальный кеш
            currentData = JsonUtility.FromJson<rawData>(rawString);//десер сюда
            DateTime parsedDate = DateTime.Parse(currentData.datetime);//парсим нужную строку
            DateTime currentTime = parsedDate;//присваем отдельно
            return currentTime; //вернули
        }

        internal static IEnumerator LoadTextFromServer(string url)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);//гет сайту
            yield return request.SendWebRequest();//работаем
            if (request.result == UnityWebRequest.Result.Success)
            {
                rawString = request.downloadHandler.text;//успех? пишем текст
            }
            else
            {
                Debug.LogErrorFormat("error request [{0}, {1}]", url, request.error);
                rawString = null;//ошибка - не пишем текст
            }
            request.Dispose();//чистимся
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

