namespace Eruru.ChatRobotRPC {

	/// <summary>
	/// 群成员信息
	/// </summary>
	public class ChatRobotGroupMemberInformation {

		/// <summary>
		/// 群成员QQ
		/// </summary>
		public long QQ { get; set; }
		/// <summary>
		/// 昵称
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 群名片
		/// </summary>
		public string BusinessCard { get; set; }
		/// <summary>
		/// 群活跃积分
		/// </summary>
		public long ActiveIntegral { get; set; }
		/// <summary>
		/// 群活跃等级
		/// </summary>
		public long ActiveLevel { get; set; }
		/// <summary>
		/// 加群时间戳（10位）
		/// </summary>
		public long JoinTimeStamp { get; set; }
		/// <summary>
		/// 最后发言时间戳（10位）
		/// </summary>
		public long LastSpeakTimeStamp { get; set; }
		/// <summary>
		/// 禁言时间（距禁言结束时间,秒）
		/// </summary>
		public long BanSpeakSeconds { get; set; }
		/// <summary>
		/// 是否是好友
		/// </summary>
		public bool IsFriend { get; set; }

	}

}