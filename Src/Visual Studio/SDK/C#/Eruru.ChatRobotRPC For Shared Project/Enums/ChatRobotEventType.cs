namespace Eruru.ChatRobotRPC {

	/// <summary>
	/// 消息类型
	/// </summary>
	public enum ChatRobotEventType {

		/// <summary>
		/// 收到自身消息
		/// </summary>
		ReceivedOwnMessage = 2099,
		/// <summary>
		/// 群公告改变
		/// </summary>
		GroupAnnouncementChanged = 2013,
		/// <summary>
		/// 好友签名改变
		/// </summary>
		FriendSignatureChanged = 1004,
		/// <summary>
		/// 说说被评论
		/// </summary>
		TalkWasCommented = 1005,
		/// <summary>
		/// 好友正在输入
		/// </summary>
		FriendIsTyping = 1006,
		/// <summary>
		/// 好友今天首次发起会话
		/// </summary>
		FriendFirstChatToday = 1007,
		/// <summary>
		/// 被好友抖动
		/// </summary>
		WasJitterByFriend = 1008,
		/// <summary>
		/// 收到财付通转账
		/// </summary>
		ReceivedTenPayTransfer = 80001,
		/// <summary>
		/// 添加了新账号
		/// </summary>
		AddedNewAccount = 11000,
		/// <summary>
		/// QQ登录完成
		/// </summary>
		QQloggedIn = 11001,
		/// <summary>
		/// QQ被手动离线
		/// </summary>
		QQWasOfflineByManual = 11002,
		/// <summary>
		/// QQ被强制离线
		/// </summary>
		QQWasOfflineByForce = 11003,
		/// <summary>
		/// QQ长时间无响应或掉线
		/// </summary>
		QQNoResponseForLongTimeOrOffline = 11004

	}

}