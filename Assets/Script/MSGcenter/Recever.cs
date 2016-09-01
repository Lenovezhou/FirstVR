using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Datas;

public class Recever : MsgCenter {
    // sorket 数据
    Dictionary<int, RootObject> sorketdic;
    Dictionary<int, RootObject> tempdic;
	// Use this for initialization
	void Start () 
    {
        RootObject root_role=new RootObject(0, "Player", Vector3.zero,
            Quaternion.identity, ObjectState.None, ObjectType.Player);
        RootObject root_ai=new RootObject(1, "enmy", Vector3.one,
            Quaternion.identity, ObjectState.None, ObjectType.AI);
        RootObject root_bullet=new RootObject(2, "bullet",new Vector3(1,0,1),
            Quaternion.identity, ObjectState.None, ObjectType.Bullet);
        tempdic = new Dictionary<int, RootObject>();
        tempdic.Add(0,root_role);
        tempdic.Add(1,root_ai);
        tempdic.Add(2,root_bullet);
        sorketdic = new Dictionary<int, RootObject>();
      
	}
    public void GetJson() 
    {
        TextAsset[] levels = Resources.LoadAll<TextAsset>("Json");
    }
    public void Refresh(Dictionary<int,RootObject> temp_dic)
    {
        foreach (int  key in temp_dic.Keys)
        {
           
            if (sorketdic.ContainsKey(key))
            {
                RootObject root;
                temp_dic.TryGetValue(key, out root);
                
            }
            else
            {
                try
                {
                    RootObject root;
                    temp_dic.TryGetValue(key, out root);
                    sorketdic.Add(key, root);
                }
                catch (System.Exception)
                {
                    Debug.LogError("sorketdic键值对冲突" + key + "个元素");
                    throw;
                }
            }
            
        }
    }

	// Update is called once per frame
	void Update () {
        Debug.Log(Camera.main.transform.rotation.eulerAngles.x + Camera.main.transform.rotation.eulerAngles.y + Camera.main.transform.rotation.eulerAngles.z);
	}
}
