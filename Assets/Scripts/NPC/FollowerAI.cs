using System;
using UnityEngine;

public class FollowerAI : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 3f;
    public float followDistance = 0.5f; // 修改为0.5像素的跟随距离
    private Transform _nextFollower;
    bool hadFollower = false;
    public Action OnDeath;
    public Transform nextFollower{ get =>_nextFollower; set
        {
            _nextFollower = value;
            hadFollower = true;
        }
    }

    public void DetachFollower()
    {
        if (target.TryGetComponent(out FollowerAI follower))
        {
            follower._nextFollower = _nextFollower; 
        }

        if (_nextFollower)
        {
            if (_nextFollower.TryGetComponent(out FollowerAI nextFollower))
            {
                nextFollower.target = target;
            }
        }
        else
        {
            GameManager.instance.HumanFollowerTail = null;
        }
    }

    void FixedUpdate()
    {
        if(target != null)
        {
            // 计算目标后方位置
            Vector2 targetBackPos = (Vector2)target.position - (Vector2)(target.up * followDistance);
            
            // 平滑移动到后方位置
            transform.position = Vector2.Lerp(
                (Vector2)transform.position,
                targetBackPos,
                followSpeed * Time.deltaTime
            );
            
            // 保持与目标相同朝向
            //transform.rotation = Quaternion.Slerp(
            //    transform.rotation,
            //    target.rotation,
            //    followSpeed * Time.deltaTime
            //);
        }
        else
        {
            target = GameManager.instance.HumanFollowerTail ? GameManager.instance.HumanFollowerTail : GameManager.instance.PlayerTran;
            if (nextFollower) target = GameManager.instance.PlayerTran;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            if (nextFollower != null)
            {
                nextFollower.GetComponent<FollowerAI>().target = target;
                if (target.TryGetComponent(out FollowerAI targetFollower))
                {
                    targetFollower.nextFollower = nextFollower;
                }
            }
            else
            {
                GameManager.instance.HumanFollowerTail = target.TryGetComponent(out FollowerAI follower) ? target : null;
            }
            GameManager.instance.CurrentFollowerNumber--;
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }
}