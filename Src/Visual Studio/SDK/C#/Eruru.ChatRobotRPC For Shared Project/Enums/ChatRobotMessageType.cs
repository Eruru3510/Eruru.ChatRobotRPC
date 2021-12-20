namespace Eruru.ChatRobotRPC {

	public enum ChatRobotMessageType {

		/// <summary>
		/// 好友私聊
		/// </summary>
		Friend = 1,
		/// <summary>
		/// 群临时会话
		/// </summary>
		GroupTemp,
		/// <summary>
		/// 讨论组临时会话
		/// </summary>
		DiscussTemp,
		/// <summary>
		/// 网页临时会话
		/// </summary>
		WebpageTemp,
		/// <summary>
		/// 好友验证回复
		/// </summary>
		FriendVerificationReply,
		/// <summary>
		/// 群消息
		/// </summary>
		Group,
		/// <summary>
		/// 讨论组消息
		/// </summary>
		Discuss,

	}

}