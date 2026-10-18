using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WaveWarlord
{
    /// <summary>
    /// 玩家输入读取器：唯一直接访问 Input System 的组件。
    /// 管理 GameInput 的生命周期；连续输入以属性轮询提供，离散按键以事件通知。
    /// 只报告输入事实，不解释其玩法含义。
    /// </summary>
    public class PlayerInputReader : MonoBehaviour
    {
        private GameInput input;

        /// <summary>奔跑键按下时触发（每次按下触发一次，按住不重复）。</summary>
        public event Action SprintPressed;

        /// <summary>当前移动输入，分量范围 [-1, 1]；键盘斜向已由组合绑定归一化。</summary>
        public Vector2 Move => input.Player.Move.ReadValue<Vector2>();

        private void Awake() => input = new GameInput();

        private void OnEnable()
        {
            input.Player.Sprint.performed += OnSprintPerformed;
            input.Player.Enable();
        }

        private void OnDisable()
        {
            input.Player.Sprint.performed -= OnSprintPerformed;
            input.Player.Disable();
        }

        private void OnDestroy() => input.Dispose();

        private void OnSprintPerformed(InputAction.CallbackContext context) => SprintPressed?.Invoke();
    }
}
