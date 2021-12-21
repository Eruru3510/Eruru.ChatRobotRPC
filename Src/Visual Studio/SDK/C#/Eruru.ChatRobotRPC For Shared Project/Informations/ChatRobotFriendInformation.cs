namespace Eruru.ChatRobotRPC {

	/// <summary>
	/// 好友信息
	/// </summary>
	public class ChatRobotFriendInformation {

		public long QQ { get; set; }
		/// <summary>
		/// 昵称
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 性别（255隐藏 0男 1女）
		/// </summary>
		public ChatRobotGender Gender { get; set; }
		/// <summary>
		/// 年龄
		/// </summary>
		public long Age { get; set; }
		/// <summary>
		/// 国家
		/// </summary>
		public string Country { get; set; }
		/// <summary>
		/// 省份
		/// </summary>
		public string Province { get; set; }
		/// <summary>
		/// 城市
		/// </summary>
		public string City { get; set; }
		/// <summary>
		/// 头像URL
		/// </summary>
		public string AvatarURL { get; set; }

	}

}