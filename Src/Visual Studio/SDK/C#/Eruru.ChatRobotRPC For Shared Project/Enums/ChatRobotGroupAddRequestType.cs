namespace Eruru.ChatRobotRPC {

	/// <summary>
	/// 群添加请求类型
	/// </summary>
	public enum ChatRobotGroupAddRequestType {

		/// <summary>
		/// 有人申请加群
		/// </summary>
		Request = 1,
		/// <summary>
		/// 某人邀请我加群
		/// </summary>
		InviteMe = 2,
		/// <summary>
		/// 群员邀请某人加群
		/// </summary>
		MemberInvite = 3

	}

}