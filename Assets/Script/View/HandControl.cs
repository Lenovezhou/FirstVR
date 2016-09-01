using UnityEngine;
using System.Collections;


[RequireComponent(typeof(SteamVR_TrackedObject))]

public class HandControl : MonoBehaviour
{


    SteamVR_TrackedObject TransfromObj;
    public Transform Hands;
    public Transform RayTip;
    public float Radio = 1f;
    public GameObject target;
    public GameObject HasTouch;
    public GameObject HasRayObj;
    public Animator ani;
    private bool IsCatching;
    public LineRenderer line;
    public bool IsOnce;

    //private bool has

    void Start()
    {
        TransfromObj = GetComponent<SteamVR_TrackedObject>();

        //RayTip = TransfromObj.transform.FindChild("Model/tip");
  //      ani = TransfromObj.GetComponentInChildren<Animator>();
    }

    void OnEnable()
    {
        //RayTip = TransfromObj.transform.FindChild("Model/tip");
    }

    void FixedUpdate()
    {
       
        var device = SteamVR_Controller.Input((int)TransfromObj.index);

        #region 射线检测
        RaycastHit hit;
        if (RayTip)
        {
            if (Physics.Raycast(RayTip.position, RayTip.transform.forward, out hit, 100, 1 << 5))
            {
                Debug.Log("检测到物体！");
                if (HasRayObj == null)
                {
                    HasRayObj = hit.transform.gameObject;
                    hit.transform.GetComponent<Renderer>().material.color = Color.green;
                }
                else
                {
                    HasRayObj.transform.GetComponent<Renderer>().material.color = Color.white;
                    HasRayObj = hit.transform.gameObject;
                }
                Debug.DrawLine(RayTip.transform.position, hit.point, Color.red);
                line.enabled = true;
                line.SetVertexCount(2);
                line.SetPosition(0, RayTip.transform.position);
                line.SetPosition(1, hit.transform.position);
            }
            else
            {
                line.enabled = false;
                if (HasRayObj != null)
                {
                    HasRayObj.transform.GetComponent<Renderer>().material.color = Color.white;
                    HasRayObj = null;
                }
            }
        }
        #endregion
        
        //switch (msgCenter._instance.operate)
        //{
        //    case Operation.Operating:

                #region 扳机键
                //扳机键
       
                if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger))
                {
                    Debug.Log("你按下了扳机键");

                    //if (HasRayObj != null)
                    //{
                    //    //触发UI事件
                    //    if (IsOnce == false)
                    //    {
                    //        HasRayObj.GetComponent<UIInfomation>().Clickbutton();
                    //        IsOnce = true;
                    //    }

                    //}

                }
                if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger))
                {
                    IsOnce = false;
                    Debug.Log("你按松开扳机键");
                    //if (HasRayObj)
                    //{
                    //    HasRayObj.transform.GetComponent<Renderer>().material.color = Color.white;
                    //    HasRayObj = null;
                    //}
                    ////左手震动  
                    //var deviceIndex = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Leftmost);
                    //SteamVR_Controller.Input(deviceIndex).TriggerHapticPulse(3000);

                    ////右手震动  
                    //var deviceIndex1 = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
                    //SteamVR_Controller.Input(deviceIndex1).TriggerHapticPulse(3000);
                }
                if (device.GetTouch(SteamVR_Controller.ButtonMask.Trigger))
                {

                    Debug.Log("你正按着扳机键");
                    //右手震动  
                    //拉弓类似操作应该就是按住trigger（扳机）gettouch时持续调用震动方法模拟弓弦绷紧的感觉。  
                    //var deviceIndex2 = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
                    //SteamVR_Controller.Input(deviceIndex2).TriggerHapticPulse(500);
                }
                #endregion

                #region 按下PressDown
                //这三种也能检测到 后面不做赘述  
                /*if(device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
                {  
                    Debug.Log("用press按下了 “trigger” “扳机键”");  
                }  
                if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))  
                {  
                    Debug.Log("用press按了 “trigger” “扳机键”");  
                }  
                if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))  
                {  
                    Debug.Log("用press松开了 “trigger” “扳机键”");  
                }  */
                #endregion

                #region 按下system键
                //system键 圆盘下面那个键   
                // reserved 为Steam系统保留,用来调出Steam系统菜单 因此貌似自己加的功能没啥用  
                //if (device.GetTouchDown(SteamVR_Controller.ButtonMask.System))
                //{
                //    Debug.Log("按下了 “system” “系统按钮/Steam”");
                //}
                //if (device.GetPressDown(SteamVR_Controller.ButtonMask.System))
                //{
                //    Debug.Log("用press按下了 “System” “系统按钮/Steam”");
                //}
                #endregion

                #region 按下菜单键
                //菜单键
                //if (device.GetTouchDown(SteamVR_Controller.ButtonMask.ApplicationMenu))
                //{
                //    Debug.Log("你按下了菜单键");
                //}
                #endregion

                #region 按下左右拾取键
                //抓握按键手柄左右两侧的抓握按键为一个按键
                if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Grip))
                {
                    Debug.Log("按下侧键");
                    if (ani)
                    {
                        ani.CrossFade("forward", 0.2f);
                    }
                    //Hands = TransfromObj.transform.FindChild("Model/tip/attach");

                   // Collider[] colliderArray = Physics.OverlapSphere(Hands.transform.position, 0.1f, 1 << 8);
                    //if (colliderArray.Length != 0)
                    //{
                    //    target = colliderArray[0].gameObject;
                    //    float distance = Vector3.Distance(target.transform.position, Hands.transform.position);
                    //    foreach (Collider item in colliderArray)
                    //    {
                    //        float temp = Vector3.Distance(item.gameObject.transform.position, Hands.transform.position);
                    //        if (temp < distance)
                    //        {
                    //            distance = temp;
                    //            target = item.gameObject;
                    //        }
                    //    }
                    //}

                    if (Hands != null && target != null)
                    {
                        if (Vector3.Distance(Hands.transform.position, target.transform.position) < 0.15f)
                        {
                            Debug.Log("抓到了物体！");
                            IsCatching = true;

                            //HasTouch = target;
                            //HasTouch.transform.parent = Hands;
                            //HasTouch.transform.localPosition = Vector3.zero;
                            //HasTouch.GetComponent<ObjInfo>().HasCatching();
                            //HasTouch.GetComponent<Rigidbody>().isKinematic = true;
                            //target.transform.GetComponent<Renderer>().material.color = Color.red;
                        }
                        else
                        {
                            Debug.Log("距离太远了");
                        }
                    }
                    Debug.Log("你正紧握");
                }
                if (device.GetTouch(SteamVR_Controller.ButtonMask.Grip))
                {
                    if (IsCatching)
                    {
                        var deviceIndex2 = SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost);
                        SteamVR_Controller.Input(deviceIndex2).TriggerHapticPulse(3000);
                    }
                }
                if (device.GetTouchUp(SteamVR_Controller.ButtonMask.Grip))
                {
                    if (ani)
                    {
                        ani.CrossFade("back", 0.2f);
                    }
                    Debug.Log("你松开了");
                    //if (HasTouch != null)
                    //{
                    //    IsCatching = false;
                    //    Hands.DetachChildren();
                    //    HasTouch.GetComponent<Rigidbody>().isKinematic = false;
                    //    HasTouch.GetComponent<ObjInfo>().HasDetch();
                    //    target.transform.GetComponent<Renderer>().material.color = Color.white;
                    //    HasTouch = null;
                    //}
                }
                #endregion

                #region 圆盘操作
                //下面是触摸板事件 我们通过判断坐标是否超过0.5来判断按了哪个
                if (device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
                {
                    if (device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y > 0.5f)
                    {
                        Debug.Log("你按下了触摸板的上");
                    }
                    else if (device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y < -0.5)
                    {
                        Debug.Log("你按下了触摸板的下");
                    }

                    if (device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x > 0.5f)
                    {
                        Debug.Log("你按下了触摸板的右");
                    }
                    else if (device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x < -0.5f)
                    {
                        Debug.Log("你按下了触摸板的左");
                    }
                }

                if (device.GetTouchDown(SteamVR_Controller.ButtonMask.Touchpad))
                {
                    if (device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y > 0.5f)
                    {
                        Debug.Log("你触摸了触摸板的上");
                    }
                    else if (device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).y < -0.5)
                    {
                        Debug.Log("你触摸了触摸板的下");
                    }

                    if (device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x > 0.5f)
                    {
                        Debug.Log("你触摸了触摸板的右");
                    }
                    else if (device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad).x < -0.5f)
                    {
                        Debug.Log("你触摸了触摸板的左");

                    }
                }
                #endregion

                #region 圆盘操作
                /*
		//Axis0键 与圆盘有交互 与圆盘有关  
		//触摸触发  
		if (device.GetTouchDown (SteamVR_Controller.ButtonMask.Axis0)) {  
			Debug.Log ("按下了 “Axis0” “方向 ”");  
		}  
		//按动触发  
		if (device.GetPressDown (SteamVR_Controller.ButtonMask.Axis0)) {  
			Debug.Log ("用press按下了 “Axis0” “方向 ”");  
		}  

		//Axis1键  目前未发现按键位置  
		//触摸触发  
		if (device.GetTouchDown (SteamVR_Controller.ButtonMask.Axis1)) {  
			Debug.Log ("按下了 “Axis1” “ ”");  
		}  
		//按动触发   
		if (device.GetPressDown (SteamVR_Controller.ButtonMask.Axis1)) {  
			Debug.Log ("用press按下了 “Axis1” “ ”");  
		}  

		//Axis2键 目前未发现按键位置  
		//触摸触发  
		if (device.GetTouchDown (SteamVR_Controller.ButtonMask.Axis2)) {  
			Debug.Log ("按下了 “Axis2” “ ”");  
		}  
		//按动触发  
		if (device.GetPressDown (SteamVR_Controller.ButtonMask.Axis2)) {  
			Debug.Log ("用press按下了 “Axis2” “ ”");  
		}  


		//Axis3键  未目前未发现按键位置  
		//触摸触发  
		if (device.GetTouchDown (SteamVR_Controller.ButtonMask.Axis3)) {  
			Debug.Log ("按下了 “Axis3” “ ”");  
		}  
		//按动触发  
		if (device.GetPressDown (SteamVR_Controller.ButtonMask.Axis3)) {  
			Debug.Log ("用press按下了 “Axis3” “ ”");  
		}  

		//Axis4键  目前未发现按键位置  
		//触摸触发  
		if (device.GetTouchDown (SteamVR_Controller.ButtonMask.Axis4)) {  
			Debug.Log ("按下了 “Axis4” “ ”");  
		}  
		//按动触发  
		if (device.GetPressDown (SteamVR_Controller.ButtonMask.Axis4)) {  
			Debug.Log ("用press按下了 “Axis4” “ ”");  
		}  
		*/
                #endregion

                //break;
        //}



    }

    //void OnDrawGizmos()
    //{
    //    if (RayTip != null && HasRayObj != null)
    //    {
    //        Debug.Log("执行了");
    //        Gizmos.color = Color.black;
    //        Gizmos.DrawLine(RayTip.transform.position, HasRayObj.transform.position);
    //    }
    //}

}
