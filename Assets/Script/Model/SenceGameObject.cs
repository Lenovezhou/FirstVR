using UnityEngine;
using System.Collections;
using Datas;

public class SenceGameObject : MonoBehaviour
{
    public const string PLAYER_TAG = "Player";
    public const string AI_TAG = "Bullet";
    public const string AI_BULLET_TAG = "Bullet";
    public const string PLAYER_BULLET_TAG = "PlayerBullet";

    private AI _pc_man;
    private Role _player;
    private Bullet _bullet;
    private ObjectType type;
    public RootObject SenceObject 
    {
        set 
        {
            transform.position = value.Position;
            transform.rotation = value.Rotation;
            switch (value.type)
            {
                case ObjectType.Player:
                    _player = value as Role;
                    break;
                case ObjectType.AI:
                    _pc_man = value as AI;
                    break;
                case ObjectType.Bullet:
                    _bullet = value as Bullet;
                    break;
                default:
                    break;
            }
            type = value.type;
        }
        get 
        {
            switch (type)
            {
                case ObjectType.Player:
                    return _player;
                    break;
                case ObjectType.AI:
                    return _pc_man;
                    break;
                case ObjectType.Bullet:
                    return _bullet;
                    break;
                default:
                    return null;
                    break;
            }
        }
    }

    public void OnTriggerEnter(Collider otherColl) 
    {
        SenceGameObject obj_data;
        switch (type)
        {
            case ObjectType.Player:
                if (otherColl.gameObject.tag == AI_BULLET_TAG || otherColl.gameObject.tag == AI_TAG)
                {
                    obj_data = otherColl.gameObject.GetComponent<SenceGameObject>();
                    _player.HP -= obj_data.SenceObject.Attack;
                }
                break;
            case ObjectType.AI:
                if (otherColl.gameObject.tag == PLAYER_BULLET_TAG)
                {
                    obj_data = otherColl.gameObject.GetComponent<SenceGameObject>();
                    _pc_man.HP -= obj_data.SenceObject.Attack;
                }
                break;
            case ObjectType.Bullet:
                
                break;
            case ObjectType.PlayerBullet:
                
                break;
        }
    }
}
