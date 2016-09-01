using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Datas;

public class MsgCenter : MonoBehaviour {
    public static MsgCenter Instance;

    public Text HPTx;

    public string Host;
    public int Port;

    //  关卡数据
    public Dictionary<int, Role> RoleLevelData;

    public Dictionary<int, List<AI>> AILevelData;
    
    //  >>>>>>>>>>>>>>>>>  场景数据<id,对象> <<<<<<<<<<<<<
    public Dictionary<int,SenceGameObject> SenceObjects;
    //<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<

    // 玩家预设
    public List<GameObject> RolePrefabs;
    // AI预设
    public List<GameObject> AiPrefabs;

    // 武器预设
    public List<GameObject> WeaponPrefabs;

    // 武器名称
    public List<string> WeaponsName;

    private int id_index;
    private NetworkServer _network_server;
    void Awake() 
    {
        Instance = this;
        id_index = 0;
        SenceObjects = new Dictionary<int, SenceGameObject>();
       
        LoadLevelData();
        Init(1);
    }

  
	// Use this for initialization
	void Start () 
    {
	    
	}

    private Role _role;
	// Update is called once per frame
	void Update () 
    {
	    if(_role != null)
        {
            HPTx.text = _role.HP.ToString();
        }
	}
    /// <summary>
    /// 加载关卡数据
    /// </summary>
    public void LoadLevelData() 
    {
        RoleLevelData = new Dictionary<int, Role>();
        TextAsset[] levels = Resources.LoadAll<TextAsset>("levels_role");
        XmlDocument xml;
        XmlElement xmlRoot;
        Role role;
        foreach (TextAsset txt in levels)
        {
            xml = new XmlDocument();
            xml.LoadXml(txt.ToString());
            xmlRoot = xml.DocumentElement;
            role = new Role(-1,xmlRoot["Role"].Attributes["Name"].Value,
                Vector3.zero,Quaternion.identity,
                int.Parse(xmlRoot["Role"].Attributes["HP"].Value),
                float.Parse(xmlRoot["Role"].Attributes["LaunchSpeed"].Value),
                int.Parse(xmlRoot["Role"].Attributes["BulletLimit"].Value),
                int.Parse(xmlRoot["Role"].Attributes["BulletCount"].Value),
                ObjectState.None,
                ObjectType.Player,
                int.Parse(xmlRoot["Role"].Attributes["Grade"].Value),
                xmlRoot["Role"].Attributes["useWeapon"].Value,
                null,
                int.Parse(xmlRoot["Role"].Attributes["Attack"].Value),
                int.Parse(xmlRoot["Role"].Attributes["Gold"].Value),
                int.Parse(xmlRoot["Role"].Attributes["Index"].Value)
                );
            RoleLevelData.Add(int.Parse(xmlRoot["Role"].Attributes["Level"].Value), role);
        }
        AILevelData = new Dictionary<int,List<AI>>();
        levels = Resources.LoadAll<TextAsset>("levels_ai");
        AI ai;
        List<AI> ls_ai;
        int level = -1;
        foreach (TextAsset txt in levels)
        {
            xml = new XmlDocument();
            xml.LoadXml(txt.ToString());
            xmlRoot = xml.DocumentElement;
            ls_ai = new List<AI>();
            level = int.Parse(xmlRoot["ai_list"].Attributes["Level"].Value);
            foreach (XmlNode node in xmlRoot["ai_list"].ChildNodes)
            {
                ai = new AI(-1,node.Attributes["Name"].Value,Vector3.zero, Quaternion.identity,
                    int.Parse(node.Attributes["HP"].Value),
                    float.Parse(node.Attributes["LaunchSpeed"].Value),
                    int.Parse(node.Attributes["BulletLimit"].Value),
                    int.Parse(node.Attributes["BulletCount"].Value),
                    ObjectState.None,ObjectType.AI,
                    int.Parse(node.Attributes["Grade"].Value),
                    node.Attributes["useWeapon"].Value,
                    null,
                    int.Parse(node.Attributes["Attack"].Value),
                    0,
                    float.Parse(node.Attributes["next_create_time"].Value),
                    float.Parse(node.Attributes["launch_offset"].Value),
                    int.Parse(node.Attributes["Index"].Value)
                    );
                ls_ai.Add(ai);
            }
            AILevelData.Add(level, ls_ai);
        }
    }

    public void Init(int level)
    {
        _network_server = gameObject.AddComponent<NetworkServer>();

        if (RoleLevelData.ContainsKey(level))
        {
            GameObject obj = Instantiate(RolePrefabs[RoleLevelData[level].Index]) as GameObject;
            SenceGameObject obj_data = obj.AddComponent<SenceGameObject>();
            RoleLevelData[level].Position = obj.transform.position;
            RoleLevelData[level].Rotation = obj.transform.rotation;
            obj_data.SenceObject = RoleLevelData[level];
            obj_data.SenceObject.ID = id_index;
            _role = obj_data.SenceObject as Role;
            SenceObjects.Add(obj_data.SenceObject.ID, obj_data);
            obj.transform.parent = this.transform;
            obj.name = "id_" + id_index + "_" + RoleLevelData[level].Name;
            id_index++;
        }
        StartCoroutine(CreateAI(level));
    }

    public IEnumerator CreateAI(int level) 
    {
        if (AILevelData.ContainsKey(level))
        {
            List<AI> ls_ai = AILevelData[level];
            GameObject obj;
            SenceGameObject senceObj;
            for (int i = 0; i < ls_ai.Count; i++,id_index++)
            {
                obj = Instantiate(AiPrefabs[ls_ai[i].Index]) as GameObject;
                senceObj = obj.AddComponent<SenceGameObject>();
                ls_ai[i].ID = id_index;
                ls_ai[i].Position = obj.transform.position;
                ls_ai[i].Rotation = obj.transform.rotation;
                senceObj.SenceObject = ls_ai[i];
                SenceObjects.Add(ls_ai[i].ID, senceObj);
                obj.transform.parent = this.transform;
                obj.name = "id_" + id_index.ToString() + "_" + ls_ai[i].Name;
                yield return new WaitForSeconds(ls_ai[i].next_create_time);
            }
        }
    }
}
