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
		GroupTemp,
		/// <summary>
		/// 讨论组临时
		/// </summary>
		DiscussTemp,
		/// <summary>
		/// 网页临时
		/// </summary>
		WebpageTemp,
		/// <summary>
		/// 好友验证回复
		/// </summary>
		FriendVerificationReply,
		/// <summary>
		/// 群
		/// </summary>
		Group,
		/// <summary>
		/// 讨论组
		/// </summary>
		Discuss,

	}

}