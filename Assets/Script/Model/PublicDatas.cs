using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace Datas
{
    public enum ObjectState
    {
        None
    }

    public enum ObjectType 
    {
        Player,  // 玩家
        AI,   //  机器人
        Bullet,   //  电脑人的子弹
        PlayerBullet  // 玩家的子弹
    }

    public class Connection
    {
        public string Host;
        public int Port;
        public IPEndPoint ip;
        public Connection(string p_Host, int p_Port) 
        {
            Host = p_Host;
            Port = p_Port;
            ip = new IPEndPoint(IPAddress.Parse(Host), Port);

        }
    }

    public class RootObject 
    {
        //  物体的ID
        public int ID;
        //  物体名称
        public string Name;
        //  位置
        public float v3_p_x;
        public float v3_p_y;
        public float v3_p_z;
        //  旋转角
        public float v3_r_x;
        public float v3_r_y;
        public float v3_r_z;
        public float v3_r_w;
        //  物体状态
        public ObjectState state;
        //  物体的类型
        public ObjectType type;

        //  物体能够造成的伤害
        public int Attack;
        [JsonIgnore]
        private Vector3 _position;
        [JsonIgnore]
        public Vector3 Position 
        {
            set 
            {
                _position = value;
                v3_p_x = _position.x;
                v3_p_y = _position.y;
                v3_p_z = _position.z;
            }
            get 
            {
                return _position;
            }
        }
        [JsonIgnore]
        private Quaternion _rotation;
        [JsonIgnore]
        public Quaternion Rotation 
        {
            set 
            {
                _rotation = value;
                v3_r_x = _rotation.x;
                v3_r_y = _rotation.y;
                v3_r_z = _rotation.z;
                v3_r_w = _rotation.w;
            }
            get 
            {
                return _rotation;
            }
        }

        public RootObject(int p_ID, string p_Name, Vector3 position,
            Quaternion rotation, ObjectState p_state, ObjectType p_type) 
        {
            ID = p_ID;
            Name = p_Name;
            Position = position;
            Rotation = rotation;
            state = p_state;
            type = p_type;
        }
    }

    public class Role : RootObject 
    {
        // 血量
        public int HP;
        // 子弹发射速度(颗/秒)
        public float LaunchSpeed;
        
        //  子弹夹上限
        public int BulletLimit;
        //  当前拥有的子弹数
        public int BulletCount;

        //  消灭敌人数，即分数
        public int Grade;

        //  角色当前使用的武器
        public string useWeapon;

        //  角色购买拥有的武器
        [JsonIgnore]
        public Dictionary<string, object> Weapons;

        //  角色拥有的金币数
        public int Gold;

        //  玩家在预设列表中的索引
        public int Index = -1;
        /// <summary>
        /// 玩家初始化
        /// </summary>
        /// <param name="p_Name">玩家名字</param>
        /// <param name="p_Position">实例玩家地点</param>
        /// <param name="p_Rotation">实例玩家角度</param>
        /// <param name="p_HP">玩家初始最大血量</param>
        /// <param name="p_LaunchSpeed">子弹速度</param>
        /// <param name="p_BulletLimit">子弹数量</param>
        /// <param name="p_BulletCount">单价数量</param>

        public Role(int id,string p_Name, Vector3 p_Position, Quaternion p_Rotation,
            int p_HP, float p_LaunchSpeed, int p_BulletLimit, int p_BulletCount, ObjectState p_State,ObjectType p_type,
            int p_Grade, string p_Weapon, object weapon_obj, int p_Attack, int p_Gold,int p_index)
            :base(id,p_Name,p_Position,p_Rotation,p_State,p_type)
        {
            HP = p_HP;
            LaunchSpeed = p_LaunchSpeed;
            BulletCount = p_BulletCount;
            BulletLimit = p_BulletLimit;
            Grade = p_Grade;
            useWeapon = p_Weapon;
            Weapons = new Dictionary<string, object>();
            Weapons.Add(useWeapon, weapon_obj);
            Gold = p_Gold;
            Attack = p_Attack;
            Index = p_index;
        }
    }

    public class AI:Role
    {
        public float next_create_time = -1;
        public float launch_offset;
        
        public AI(int id, string p_Name, Vector3 p_Position, Quaternion p_Rotation,
            int p_HP, float p_LaunchSpeed, int p_BulletLimit, int p_BulletCount, ObjectState p_State, ObjectType p_type,
            int p_Grade, string p_Weapon, object weapon_obj, int p_Attack, int p_Gold, float p_next_create_time,
            float p_launch_offset, int p_Index = -1)
            :base(id,p_Name,p_Position,p_Rotation,p_HP,p_LaunchSpeed,p_BulletLimit,p_BulletCount,
            p_State, p_type, p_Grade, p_Weapon, weapon_obj, p_Attack, p_Gold, p_Index)
        {
            next_create_time = p_next_create_time;
            launch_offset = p_launch_offset;
        }
    }

    public class Bullet : RootObject 
    {
        public int Attack;

        public Bullet(int p_ID, string p_Name, int p_Attack,Vector3 position,
            Quaternion rotation, ObjectState p_state, ObjectType p_type)
            : base(p_ID, p_Name, position, rotation, p_state, p_type)
        {
            Attack = p_Attack;
        }
    }
}


