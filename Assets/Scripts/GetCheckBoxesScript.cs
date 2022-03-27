using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class GetCheckBoxesScript : MonoBehaviour
{
    public CheckBoxScript CheckBox;

    [SerializeField] private Toggle[] toggles;
    [SerializeField] private GameObject Parent;
    [SerializeField] private Text TName;
    public bool ready = false;

    [Serializable]
    public class Name
    {
        public string name;
    }

    async void Start()
    {
        ready = false;
        var data0 = Get();
        var data = FixString(await data0);
        Name[] boxes = JsonHelper.FromJson<Name>(data);
        foreach (Name box in boxes)
        {
            Create(box.name);
        }
        ready = true;
    }

    private async UniTask<string> Get(CancellationToken cancellationtoken = default)
    {
        WWWForm form = new WWWForm();
        form.AddField("SQL", CreateQuery());
        var www = UnityWebRequest.Post("http://asdasdadsads.ru/AutoScripts/Index.php", form);
        await www.SendWebRequest().WithCancellation(cancellationtoken);
        return www.result == UnityWebRequest.Result.Success ? www.downloadHandler.text : null;
    }

    private string CreateQuery()
    {
        string query = "SELECT " + TName.text + " as name FROM phone GROUP BY " + TName.text;
        return query;
    }

    public string FixString(string data)
    {
        var str = "{\"Items\":" + data + "}";
        return str;
    }

    public void Create(string name)
    {
        var nobj = Instantiate(CheckBox, Parent.transform);
        nobj.SetUp(name);
    }

    private void OnDestroy()
    {
        GameObject[] Objects;

        Objects = GameObject.FindGameObjectsWithTag("SizeBox");
        foreach (GameObject ob in Objects)
        {
            Destroy(ob);
        }
    }
}
