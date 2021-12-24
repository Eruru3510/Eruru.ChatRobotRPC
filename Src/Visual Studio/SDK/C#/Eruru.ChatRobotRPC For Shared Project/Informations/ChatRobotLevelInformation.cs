namespace Eruru.ChatRobotRPC {

	/// <summary>
	/// 等级信息
	/// </summary>
	public class ChatRobotLevelInformation {

		/// <summary>
		/// 会员
		/// </summary>
		public string VIP { get; set; }
		/// <summary>
		/// 等级
		/// </summary>
		public int Level { get; set; }
		/// <summary>
		/// 活跃天数
		/// </summary>
		public int ActiveDays { get; set; }
		/// <summary>
		/// 升级剩余天数
		/// </summary>
		public int DaysRemainingForUpgrade { get; set; }

	}

}