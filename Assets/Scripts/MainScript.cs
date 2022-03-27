using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine.UI;

public class MainScript : MonoBehaviour
{ 
    public SortScript Sort;
    public Dropdown Preset;

    public ToggleScript Country;
    public ToggleScript Equipment;
    public ToggleScript OS;
    public ToggleScript CPU;
    public ToggleScript ScrinSize;
    public ToggleScript ScrinType;
    public ToggleScript ScrinDefend;


    public MinMaxScript Cost;
    public MinMaxScript Year;
    public MinMaxScript MemorySize;
    public MinMaxScript CameraSize;


    [Serializable]
    public class Phone
    {
        public string name;
        public string link;
        public string image;
        public string cost;
    }

    public string server;

    public OutputScript output;
    public async UniTask GetData()
    {
        var data0 = Get();
        output.Destroy();
        var data = FixString(await data0);
        if (data == "{\"Items\":Товары не найдены}" || data == "{\"Items\":[]}")
        {
            await output.Create("Телефоны не найдены", "https://ukr.host/kb/wp-content/uploads/2018/05/404.jpg", "https://ukr.host/kb/wp-content/uploads/2018/05/404.jpg", "");
        }
        else if (data == "{\"Items\":Неправильно переданы данные}")
        {
            await output .Create("Ошибка в передаче данных", "https://cleverics.ru/digital/wp-content/uploads/2014/03/error.png", "https://cleverics.ru/digital/wp-content/uploads/2014/03/error.png", "");
        }
        else
        {
            Phone[] autos = JsonHelper.FromJson<Phone>(data);
            var CreateCards = autos.Select(async card =>
            {
                await output.Create(card.name, card.link, card.image, "от " + card.cost + " руб.");
            });
            await UniTask.WhenAll(CreateCards);
        }
    }

    private void Start()
    {

    }

    public async void Search()
    {
        var g = Country.ready && Equipment.ready && OS.ready && CPU.ready && ScrinSize.ready && ScrinType.ready && ScrinDefend.ready && Cost.ready && Year.ready && MemorySize.ready && CameraSize.ready;
        if (!g)
        {
            await Task.Delay(5);
            Search();
        }
        else
        {
            output.Destroy();
            if (output.ready)
                await GetData();
        }
    }

    public string FixString(string data)
    {
        var str = "{\"Items\":" + data + "}";
        return str;
    }

    public void Exit()
    {
        output.Destroy();
        Application.Quit();
    }

    private async UniTask<string> Get(CancellationToken cancellationtoken = default)
    {
        WWWForm form = new WWWForm();
        form.AddField("SQL", CreateQuery());
        var www = UnityWebRequest.Post(server, form);
        await www.SendWebRequest().WithCancellation(cancellationtoken);
        return www.result == UnityWebRequest.Result.Success ? www.downloadHandler.text : null;
    }

    private string CreateQuery()
    {
        string query = "SELECT name, link, image, cost FROM phone WHERE " + Country.Get_Names() + " AND " +
                                                                     Equipment.Get_Names() + " AND " +
                                                                     OS.Get_Names() + " AND " +
                                                                     CPU.Get_Names() + " AND " +
                                                                     ScrinDefend.Get_Names() + " AND " +
                                                                     ScrinSize.Get_Names() + " AND " +
                                                                     ScrinType.Get_Names() + " AND " +
                                                                     Cost.getValues() + " AND " +
                                                                     MemorySize.getValues() + " AND " +
                                                                     CameraSize.getValues() + " AND " +
                                                                     Year.getValues() + " " +
                                                                     Sort.Get_Sort();
        return query;
    }

    public void Presets()
    {
        int value = Preset.value;

        switch (value)
        {
            case 1:
                Default();
                Defend();
                break;
            case 2:
                Default();
                Style();
                break;
            default:
                Default();
                break;
        }
    }

    private void Style()
    {
        string[] os = { "IOS" };
        OS.Set(os);
    }

    private void Defend()
    {
        string[] def = {"Защита от влаги и пыли"};
        string[] eq = { "Защитная плёнка", "Чехол" };
        ScrinDefend.Set(def);
        Equipment.Set(eq);
    }

    private void Default()
    {
        OS.Default();
        ScrinDefend.Default();
        Equipment.Default();
        Country.Default();
        Cost.Default();
        Year.Default();
    }
}

public static class Checking
{
    public static bool Get = false;
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}