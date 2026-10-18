using UnityEngine;

namespace WaveWarlord
{
    /// <summary>
    /// 角色移动执行器：接收期望速度（米/秒），每帧换算为位移并交给 CharacterController。
    /// 碰撞阻挡与沿墙滑动由 CharacterController.Move 完成；本组件不关心速度来自玩家还是 AI。
    /// 执行顺序排在默认脚本之后，保证同一帧内先收到速度、再移动。
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    [DefaultExecutionOrder(100)]
    public class CharacterMotor : MonoBehaviour
    {
        private CharacterController controller;
        private Vector3 desiredVelocity;

        private void Awake() => controller = GetComponent<CharacterController>();

        /// <summary>设置期望速度（米/秒），在本帧的 Update 中生效。</summary>
        public void SetMoveVelocity(Vector3 velocity) => desiredVelocity = velocity;

        private void Update()
        {
            if (desiredVelocity == Vector3.zero)
            {
                return;
            }

            // 每帧只调用一次 Move：所有位移来源应先合并，再一次性移动。
            controller.Move(desiredVelocity * Time.deltaTime);
        }
    }
}
