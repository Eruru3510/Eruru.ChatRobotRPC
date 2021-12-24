namespace Eruru.ChatRobotRPC {

	/// <summary>
	/// 群成员列表信息
	/// </summary>
	public class ChatRobotGroupMemberListInformation {

		/// <summary>
		/// 群人数
		/// </summary>
		public int MemberNumber { get; set; }
		/// <summary>
		/// 群人数上限
		/// </summary>
		public int MaxMemberNumber { get; set; }
		/// <summary>
		/// 群主QQ
		/// </summary>
		public long Master { get; set; }
		/// <summary>
		/// 群管理员
		/// </summary>
		public long[] Administrators { get; set; }

	}

}