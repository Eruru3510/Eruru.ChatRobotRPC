namespace Eruru.ChatRobotRPC {

	/// <summary>
	/// 礼物信息
	/// </summary>
	public class ChatRobotGiftInformation {

		/// <summary>
		/// 礼物ID
		/// </summary>
		public int ID { get; set; }
		/// <summary>
		/// 礼物名称
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 礼物价值
		/// </summary>
		public int Value { get; set; }
		/// <summary>
		/// 礼物获得时间戳
		/// </summary>
		public int GetTimeStamp { get; set; }
		/// <summary>
		/// 礼物过期时间戳
		/// </summary>
		public int ExpirationTimeStamp { get; set; }

	}

}