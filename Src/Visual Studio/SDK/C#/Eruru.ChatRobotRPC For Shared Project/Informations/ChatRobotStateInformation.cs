namespace Eruru.ChatRobotRPC {

	/// <summary>
	/// 状态信息
	/// </summary>
	public class ChatRobotStateInformation {

		/// <summary>
		/// 机器人QQ
		/// </summary>
		public long QQ { get; set; }
		/// <summary>
		/// 昵称
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 在线状态
		/// </summary>
		public string State { get; set; }
		/// <summary>
		/// 消息速度
		/// </summary>
		public string MessageSpeed { get; set; }
		/// <summary>
		/// 接收消息数
		/// </summary>
		public int ReceiveMessageNumber { get; set; }
		/// <summary>
		/// 发送消息数
		/// </summary>
		public int SendMessageNumber { get; set; }
		/// <summary>
		/// 在线时间
		/// </summary>
		public string OnlineTime { get; set; }

	}

}