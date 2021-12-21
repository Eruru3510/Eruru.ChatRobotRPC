namespace Eruru.ChatRobotRPC {

	/// <summary>
	/// 群信息
	/// </summary>
	public class ChatRobotGroupInformation {

		/// <summary>
		/// 群号
		/// </summary>
		public long Group { get; set; }
		/// <summary>
		/// 群名
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 群主QQ
		/// </summary>
		public long Master { get; set; }
		/// <summary>
		/// 是否是管理员（群主时也为真,可通过群主QQ区分）
		/// </summary>
		public bool IsAdministrator { get; set; }

	}

}