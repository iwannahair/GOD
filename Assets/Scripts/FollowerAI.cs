using UnityEngine;

public class FollowerAI : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 3f;
    public float followDistance = 0.5f; // 修改为0.5像素的跟随距离
    public bool isFirstFollower = true; // 是否是第一个跟随者

    void Start()
    {
        if(target == null)
        {
            if(isFirstFollower)
            {
                // 第一个跟随者寻找玩家
                GameObject player = GameObject.FindWithTag("Player");
                if(player != null) target = player.transform;
            }
            else
            {
                // 非第一个跟随者寻找前一个跟随者
                FollowerAI[] followers = FindObjectsOfType<FollowerAI>();
                foreach(FollowerAI follower in followers)
                {
                    if(follower != this && follower.gameObject != gameObject)
                    {
                        target = follower.transform;
                        break;
                    }
                }
            }
        }
        
        // 初始生成时立即定位到目标正后方
        if(target != null)
        {
            Vector2 initialPos = (Vector2)target.position - (Vector2)(target.up * followDistance);
            transform.position = initialPos;
            transform.rotation = target.rotation;
        }
    }

    void Update()
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
    }
}