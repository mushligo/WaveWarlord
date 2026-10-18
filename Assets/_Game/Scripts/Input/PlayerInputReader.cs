using UnityEngine;

namespace WaveWarlord
{
    /// <summary>
    /// 玩家输入读取器：唯一直接访问 Input System 的组件。
    /// 管理 GameInput 的生命周期，并以轮询方式对外提供当前输入值。
    /// </summary>
    public class PlayerInputReader : MonoBehaviour
    {
        private GameInput input;

        /// <summary>当前移动输入，分量范围 [-1, 1]；键盘斜向已由组合绑定归一化。</summary>
        public Vector2 Move => input.Player.Move.ReadValue<Vector2>();

        private void Awake() => input = new GameInput();

        private void OnEnable() => input.Player.Enable();

        private void OnDisable() => input.Player.Disable();

        private void OnDestroy() => input.Dispose();
    }
}
