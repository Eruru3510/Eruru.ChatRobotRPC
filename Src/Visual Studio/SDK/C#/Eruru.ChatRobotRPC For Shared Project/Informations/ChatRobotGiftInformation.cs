namespace Eruru.ChatRobotRPC {

	/// <summary>
	/// 礼物信息
	/// </summary>
	public class ChatRobotGiftInformation {

		/// <summary>
		/// 礼物ID
		/// </summary>
		public long ID { get; set; }
		/// <summary>
		/// 礼物名称
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		/// 礼物价值
		/// </summary>
		public long Value { get; set; }
		/// <summary>
		/// 礼物获得时间戳
		/// </summary>
		public long GetTimeStamp { get; set; }
		/// <summary>
		/// 礼物过期时间戳
		/// </summary>
		public long ExpirationTimeStamp { get; set; }

	}

}