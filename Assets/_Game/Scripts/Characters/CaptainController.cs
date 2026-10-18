using UnityEngine;

namespace WaveWarlord
{
    /// <summary>
    /// 船长控制器：把玩家输入转换为期望速度，交给 CharacterMotor 执行。
    /// 奔跑为切换模式：按一下进入、再按一下退出；停止输入超过宽限时间后自动退出。
    /// 不直接移动角色，也不处理 deltaTime。
    /// </summary>
    [RequireComponent(typeof(PlayerInputReader), typeof(CharacterMotor))]
    public class CaptainController : MonoBehaviour
    {
        /// <summary>输入长度的平方低于此值视为停止（容忍摇杆回中后的微小漂移）。</summary>
        private const float StopThresholdSqr = 0.01f;

        [SerializeField, Min(0f), Tooltip("行走速度（米/秒）")]
        private float moveSpeed = 5f;

        [SerializeField, Min(0f), Tooltip("奔跑速度（米/秒）")]
        private float sprintSpeed = 8f;

        [SerializeField, Min(0f), Tooltip("停止输入持续超过该时长（秒）才退出奔跑，容忍换向时的按键空隙")]
        private float sprintExitDelay = 0.2f;

        private float stopTimer;   // 输入持续为零的累计时长（秒）
        private bool isSprinting;

        private PlayerInputReader inputReader;
        private CharacterMotor motor;

        private void Awake()
        {
            inputReader = GetComponent<PlayerInputReader>();
            motor = GetComponent<CharacterMotor>();
        }

        private void OnEnable() => inputReader.SprintPressed += OnSprintPressed;

        private void OnDisable() => inputReader.SprintPressed -= OnSprintPressed;

        private void Update()
        {
            Vector2 moveInput = inputReader.Move;

            if (isSprinting && moveInput.sqrMagnitude < StopThresholdSqr)
            {
                stopTimer += Time.deltaTime;
                if (stopTimer >= sprintExitDelay)
                {
                    isSprinting = false;
                }
            }
            else
            {
                stopTimer = 0f;
            }

            // 输入平面 (x, y) 映射到世界水平面 (x, 0, z)。
            // 只限制长度不超过 1，不强制归一化，以保留摇杆轻推时的小幅值。
            Vector3 direction = Vector3.ClampMagnitude(new Vector3(moveInput.x, 0f, moveInput.y), 1f);
            float speed = isSprinting ? sprintSpeed : moveSpeed;

            motor.SetMoveVelocity(direction * speed);
        }

        private void OnSprintPressed()
        {
            isSprinting = !isSprinting;
            stopTimer = 0f;
        }
    }
}
