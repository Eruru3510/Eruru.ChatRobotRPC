using System;
using System.Collections.Generic;
using System.Text;

namespace Eruru.ChatRobotRPC {

	/// <summary>
	/// 在线状态
	/// </summary>
	public enum ChatRobotState {

		/// <summary>
		/// 在线
		/// </summary>
		Online = 1,
		/// <summary>
		/// Q我吧
		/// </summary>
		QMe,
		/// <summary>
		/// 离开
		/// </summary>
		Leave,
		/// <summary>
		/// 忙碌
		/// </summary>
		Busy,
		/// <summary>
		/// 勿扰
		/// </summary>
		DoNotDisturb,
		/// <summary>
		/// 隐身
		/// </summary>
		Invisible

	}

}