using UnityEngine;

namespace SupanthaPaul
{
	public class InputSystem : MonoBehaviour
	{
		// input string caching
		static readonly string HorizontalInput = "Horizontal";
		static readonly string JumpInput = "Jump";
		static readonly string DashInput = "Dash";

		//设置的是静态类所以可以直接使用（类名.函数）
		//并且在动画层面，会使动画没有延迟感
		public static float HorizontalRaw()
		{

            //相比Input.GetAxis,Raw输入更加灵敏，移动时更加流畅
            return Input.GetAxisRaw(HorizontalInput);
			
		}
        public static bool Jump()
		{
			return Input.GetButtonDown(JumpInput);
		}

		//检测冲刺输入
		public static bool Dash()
		{
			return Input.GetButtonDown(DashInput);
		}

	}
}
