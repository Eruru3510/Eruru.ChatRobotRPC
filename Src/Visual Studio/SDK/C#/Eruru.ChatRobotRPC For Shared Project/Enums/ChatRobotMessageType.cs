namespace Eruru.ChatRobotRPC {

	/// <summary>
	/// 消息类型
	/// </summary>
	public enum ChatRobotMessageType {

		/// <summary>
		/// 好友
		/// </summary>
		Friend = 1,
		/// <summary>
		/// 群临时
		/// </summary>
		GroupTemp = 2,
		/// <summary>
		/// 讨论组临时
		/// </summary>
		DiscussTemp = 3,
		/// <summary>
		/// 网页临时
		/// </summary>
		WebpageTemp = 4,
		/// <summary>
		/// 好友验证回复
		/// </summary>
		FriendVerificationReply = 5,
		/// <summary>
		/// 群
		/// </summary>
		Group = 6,
		/// <summary>
		/// 讨论组
		/// </summary>
		Discuss = 7,

	}

}