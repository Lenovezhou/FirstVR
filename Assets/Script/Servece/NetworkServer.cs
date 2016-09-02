using UnityEngine;
using System.Collections;
using Newtonsoft.Json;
using System.Collections.Generic;
using Datas;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System;
using System.Threading;

public class NetworkServer : MonoBehaviour {
    //  定义数据包头
    public const string PACK_HIDE = "$***&&&&&$$$";
    //  定义数据包尾
    public const string PACK_TAIL = "#()__((@@@@#";
    //  定义数据包的长度
    public const int PACK_SIZE = 5120;
    //  监听的地址
    public string Host;
    //  监听的端口号
    public int ListenPort;
    //  
    public Connection Connect;
    //  接收消息的socket
    public Socket socReceive;
    // 发送消息的socket
    public Socket udpClient;

    //  从连接读取到的字节
    private byte[] _bts_read;
    //  要向连接写入的字节
    private byte[] _bts_write;

    private EndPoint receiveIP;
    private EndPoint endPoint;
    private EndPoint send_end_point;
    private int n;
    private List<string> receiveDatas;
    private Thread receiveThread;
    private bool isRuntime;
    private bool _is_receiving;
	// Use this for initialization
	public void Awake () 
    {
        OnAwake();
	}
	
    public virtual void OnAwake()
    {
		// 创建socket实例
        socReceive = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
       // 创建IPEndPoint实例
		IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse(MsgCenter.Instance.Host), MsgCenter.Instance.Port);
        socReceive.Bind(ipEndPoint);
        // 发送的数据长度
		socReceive.SendBufferSize = PACK_SIZE;
       // 根据自定义的数据包长度来创建读取和写入的Byte[]数组：
		_bts_read = new byte[PACK_SIZE];
        _bts_write = new byte[PACK_SIZE];
		// 客户端socket实例
udpClient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);  //  测试
		// 测试用的Endpoint实例
send_end_point = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12801);
        udpClient.SendBufferSize = PACK_SIZE * 2;
        //udpClient.Bind(new IPEndPoint(IPAddress.Parse("192.1.1.5"),12801));
		// 接收端的IpEndPoint实例
        receiveIP = new IPEndPoint(IPAddress.Any, 0);
		// 打印接收端的地址：
        Debug.Log((receiveIP as IPEndPoint).Address.ToString());
        receiveDatas = new List<string>();
        isRuntime = true;
        _is_receiving = false;
        receiveThread = new Thread(ReceiveData);
        
        //  监听客户端消息
        StartCoroutine(Respond());
        n = -1;
    }

	// Update is called once per frame
	public virtual void FixedUpdate () 
    {
        //  发送刷新的数据
        SendData(MsgCenter.Instance.SenceObjects);
	}

    public virtual IEnumerator Respond() 
    {
        receiveThread.Start();
        while(isRuntime)
        {
            if (!_is_receiving && receiveDatas.Count > 0)
            {
                Debug.Log(receiveDatas[0]);
                ExecuteRequest(JsonConvert.DeserializeObject<List<RootObject>>(receiveDatas[0]));
                receiveDatas.RemoveAt(0);
            }
            yield return new WaitForSeconds(0.002f);
        }
    }

    public virtual void ExecuteRequest(List<RootObject> objs) 
    {
        
    }

    public void ReceiveData() 
    {
        string requset;
        int x, y;
        while (isRuntime)
        {
			// 将数据报接收到数据缓冲区并存储终结点
            n = socReceive.ReceiveFrom(_bts_read, ref receiveIP);
            
            if (n > 0 && n <= PACK_SIZE)
            {
                if (Connect == null)
                {
                    Connect = new Connection((receiveIP as IPEndPoint).Address.ToString(), (receiveIP as IPEndPoint).Port);
                    send_end_point = new IPEndPoint((receiveIP as IPEndPoint).Address, MsgCenter.Instance.Port);

                }
                requset = Encoding.UTF8.GetString(_bts_read);
                x = requset.IndexOf(PACK_HIDE);
                Debug.Log("x = " + x.ToString());
                if (x != -1)
                {
                    y = requset.IndexOf(PACK_TAIL);
                    while (y == -1)
                    {
                        n = socReceive.ReceiveFrom(_bts_read, ref receiveIP);
                        if(n > 0)
                        {
                            requset += Encoding.UTF8.GetString(_bts_read);
                        }
                        y = requset.IndexOf(PACK_TAIL);
                    }
                    Debug.Log("y = " + y);
                    requset = requset.Substring(x + PACK_HIDE.Length,y - PACK_HIDE.Length);
                    _is_receiving = true;
                    receiveDatas.Add(requset);
                    _is_receiving = false;
                }
            }
        }
        
    }

    public virtual void SendData(Dictionary<int,SenceGameObject> senceObjs) 
    {
        if (udpClient != null)
        {
			// 将场景内的所有物体都加入List
            List<RootObject> rList = new List<RootObject>();
            foreach (SenceGameObject obj_data in senceObjs.Values)
            {
                rList.Add(obj_data.SenceObject);
            }
			// 利用JsonConvert的SerializeObject方法将数据打包+包头和包尾；
            string str = PACK_HIDE + JsonConvert.SerializeObject(rList) + PACK_TAIL;
			//将Json包转换成UTF8格式
            byte[] bt = Encoding.UTF8.GetBytes(str);
            Debug.Log(bt.Length + " : " + str);
			// 如果数据长度不足
            if (bt.Length <= PACK_SIZE)
            {
                bt.CopyTo(_bts_write, 0);
                //Debug.Log(_bts_write.Length + "; " + udpClient.SendBufferSize);
                udpClient.SendTo(_bts_write, send_end_point);
                //Debug.Log("send Succ");
            }
            else 
            {
                if (bt.Length % PACK_SIZE == 0)
                {
                    for (int i = 0; i < bt.Length / PACK_SIZE; i++)
                    {
						// 1.使用System.arrycopy方法：System.arrycopy
						// (源数组 ，从第几个下标开始被复制 ，目标数组 ，从第几个下标开始复制 ，
						// 要复制的元素的数目（长度）)   如：System.arrycopy(a,0,b,0,a.length)
                        Array.Copy(bt, i * PACK_SIZE, _bts_write, 0, PACK_SIZE);
                        udpClient.SendTo(_bts_write, send_end_point);
                    }
                }
                else 
                {
					// Mathf.RoundToInt：四舍五入到整数
                    int m = Mathf.RoundToInt(bt.Length / PACK_SIZE) + 1;
                    for (int i = 0; i < m; i++)
                    {
                        if (i * PACK_SIZE < bt.Length)
                        {
                            Array.Copy(bt, i * PACK_SIZE, _bts_write, 0, PACK_SIZE);
                        }
                        else 
                        {
                            Array.Copy(bt, i * PACK_SIZE, _bts_write, 0, bt.Length - i * PACK_SIZE + 1);
                        }
                        udpClient.SendTo(_bts_write, send_end_point);
                    }
                }
            }
            
        }
    }

    public void OnDisable() 
    {
        isRuntime = false;
        udpClient.Close();
        socReceive.Close();
    }
}
