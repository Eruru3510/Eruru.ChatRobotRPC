namespace Eruru.ChatRobotRPC {

	/// <summary>
	/// 好友添加方式
	/// </summary>
	public enum ChatRobotFriendAddMethod {

		/// <summary>
		/// 允许任何人
		/// </summary>
		AllowAny,
		/// <summary>
		/// 需要验证
		/// </summary>
		NeedValidation,
		/// <summary>
		/// 需要正确答案
		/// </summary>
		NeedRightAnswer = 3,
		/// <summary>
		/// 需要回答问题
		/// </summary>
		NeedAnswerQuestion,
		/// <summary>
		/// 已经是好友
		/// </summary>
		AlreadyFriend = 99

	}

}