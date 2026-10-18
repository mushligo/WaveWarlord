using UnityEngine;

namespace WaveWarlord
{
    /// <summary>
    /// 船长控制器：把玩家输入转换为期望速度，交给 CharacterMotor 执行。
    /// 不直接移动角色，也不处理 deltaTime。
    /// </summary>
    [RequireComponent(typeof(PlayerInputReader), typeof(CharacterMotor))]
    public class CaptainController : MonoBehaviour
    {
        [SerializeField, Min(0f), Tooltip("移动速度（米/秒）")]
        private float moveSpeed = 5f;

        private PlayerInputReader inputReader;
        private CharacterMotor motor;

        private void Awake()
        {
            inputReader = GetComponent<PlayerInputReader>();
            motor = GetComponent<CharacterMotor>();
        }

        private void Update()
        {
            Vector2 input = inputReader.Move;

            // 输入平面 (x, y) 映射到世界水平面 (x, 0, z)。
            // 只限制长度不超过 1，不强制归一化，以保留摇杆轻推时的小幅值。
            Vector3 direction = Vector3.ClampMagnitude(new Vector3(input.x, 0f, input.y), 1f);

            motor.SetMoveVelocity(direction * moveSpeed);
        }
    }
}
