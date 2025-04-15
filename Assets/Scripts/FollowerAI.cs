using UnityEngine;

public class FollowerAI : MonoBehaviour
{
    public Transform target;
    public float followSpeed = 3f;
    public float followDistance = 0.5f; // 修改为0.5像素的跟随距离



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
    }
}