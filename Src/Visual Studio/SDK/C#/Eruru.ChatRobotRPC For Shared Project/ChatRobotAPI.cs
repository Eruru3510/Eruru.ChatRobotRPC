using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Eruru.ChatRobotRPC {

	public delegate void ChatRobotAction ();
	/// <summary>
	/// 群添加请求
	/// </summary>
	/// <param name="type">请求类型</param>
	/// <param name="robot">机器人QQ</param>
	/// <param name="group">群</param>
	/// <param name="qq">申请者QQ</param>
	/// <param name="inviterQQ">邀请者QQ</param>
	/// <param name="sign">请求标记</param>
	/// <param name="message">附加消息</param>
	public delegate void ChatRobotGroupAddRequestEventHandler (ChatRobotGroupAddRequestType type, long robot, long group, long qq, long inviterQQ, long sign, string message);
	public delegate void ChatRobotFriendAddResponseEventHandler (bool agree, long robot, long qq, string message);
	/// <summary>
	/// 好友添加请求
	/// </summary>
	/// <param name="robot">机器人QQ</param>
	/// <param name="qq">申请者QQ</param>
	/// <param name="message">附加消息</param>
	public delegate void ChatRobotFriendAddRequestEventHandler (long robot, long qq, string message);
	public delegate void ChatRobotGroupMessageRevokedEventHandler (long robot, long group, long qq, long messageNumber, long messageID);
	public delegate void ChatRobotGroupAnonymousSwitchedEventHandler (bool enable, long robot, long group, long qq);
	public delegate void ChatRobotGroupBannedSpeakEventHandler (bool enable, long robot, long group, long qq);
	public delegate void ChatRobotGroupNameChangedEventHandler (long robot, long group, long qq, string name);
	public delegate void ChatRobotGroupAdministratorChangedEventHandler (bool enable, long robot, long group, long qq);
	public delegate void ChatRobotGroupMemberBusinessCardChangedEventHandler (long robot, long group, long qq, string businessCard);
	public delegate void ChatRobotGroupMemberLeavedEventHandler (bool kick, long robot, long group, long qq, long operatorQQ);
	public delegate void ChatRobotGroupMemberBannedSpeakEventHandler (bool enable, long robot, long group, long qq, long operatorQQ, long seconds);
	public delegate void ChatRobotGroupMemberJoinedEventHandler (ChatRobotGroupMemberJoinType type, long robot, long group, long qq, long operatorQQ);
	public delegate void ChatRobotGroupDisbandedEventHandler (long robot, long group, long qq);
	public delegate void ChatRobotFriendStateChangedEventHandler (long robot, long qq, string state);
	public delegate void ChatRobotOnWasRemoveByFriendEventHandler (long robot, long qq);

	public class ChatRobotAPI {

		public static Action<string> OnReceived { get; set; }
		public static Action<string> OnSent { get; set; }
		public static Action<ChatRobotMessage> OnReceivedMessage { get; set; }
		/// <summary>
		/// 收到群添加请求（使用ChatRobotAPI.HandleGroupAddRequest处理）
		/// </summary>
		public static ChatRobotGroupAddRequestEventHandler OnReceivedGroupAddRequest { get; set; }
		public static ChatRobotFriendAddResponseEventHandler OnReceivedFriendAddResponse { get; set; }
		/// <summary>
		/// 收到好友添加请求（使用ChatRobotAPI.HandleFriendAddRequest处理）
		/// </summary>
		public static ChatRobotFriendAddRequestEventHandler OnReceivedFriendAddRequest { get; set; }
		public static ChatRobotGroupMessageRevokedEventHandler OnGroupMessageRevoked { get; set; }
		public static ChatRobotGroupAnonymousSwitchedEventHandler OnGroupAnonymousSwitched { get; set; }
		public static ChatRobotGroupNameChangedEventHandler OnGroupNameChanged { get; set; }
		public static ChatRobotGroupBannedSpeakEventHandler OnGroupBannedSpeak { get; set; }
		public static ChatRobotGroupAdministratorChangedEventHandler OnGroupAdministratorChanged { get; set; }
		public static ChatRobotGroupMemberBusinessCardChangedEventHandler OnGroupMemberBusinessCardChanged { get; set; }
		public static ChatRobotGroupMemberLeavedEventHandler OnGroupMemberLeaved { get; set; }
		public static ChatRobotGroupMemberBannedSpeakEventHandler OnGroupMemberBannedSpeak { get; set; }
		public static ChatRobotGroupMemberJoinedEventHandler OnGroupMemberJoined { get; set; }
		public static ChatRobotGroupDisbandedEventHandler OnGroupDisbanded { get; set; }
		public static ChatRobotFriendStateChangedEventHandler OnFriendStateChanged { get; set; }
		public static ChatRobotOnWasRemoveByFriendEventHandler OnWasRemoveByFriend { get; set; }
		public static ChatRobotAction OnDisconnected { get; set; }
		public static long HeartbeatPacketSendIntervalBySeconds {

			get => Client.HeartbeatPacketSendIntervalBySeconds;

			set => Client.HeartbeatPacketSendIntervalBySeconds = value;

		}
		public static string ProtocolVersision { get; } = "1.0.0.0";

		static readonly WaitSystem WaitSystem = new WaitSystem ();
		static readonly Client Client = new Client () {
			OnReceived = Received,
			OnSent = Sent,
			OnDisconnected = () => OnDisconnected?.Invoke ()
		};

		public static void Connect (string ip, int port, string account, string password) {
			Client.Connect (ip, port);
			ClientBeginSend (new JObject () {
				{ "Type", "Login" },
				{ "Account", account },
				{ "Password", password }
			});
		}

#if DEBUG
		public static void Test (long robot) {
			ClientBeginSend (new JObject () {
				{ "Type", "Test" },
				{ "Robot",robot  }
			});
		}
#endif

		public static string TEAEncryption (string content, string key) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (TEAEncryption) },
				{ "Content", content },
				{ "Key", key }
			});
		}

		public static string TEADecryption (string content, string key) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (TEADecryption) },
				{ "Content", content },
				{ "Key", key }
			});
		}

		public static ChatRobotGiftInformation[] QueryGroupGiftInformations (long robot) {
			return WaitSystemGet<ChatRobotGiftInformation[]> (new JObject () {
				{ "Type", nameof (QueryGroupGiftInformations) },
				{ "Robot", robot }
			});
		}

		public static string RevokeGroupMessage (long robot, long group, long messageNumber, long messageID) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (RevokeGroupMessage) },
				{ "Robot", robot },
				{ "Group", group },
				{ "MessageNumber", messageNumber },
				{ "MessageID", messageID }
			});
		}

		public static long DrawGroupGift (long robot, long group) {
			return WaitSystemGet<long> (new JObject () {
				{ "Type", nameof (DrawGroupGift) },
				{ "Robot", robot },
				{ "Group", group }
			});
		}

		/// <summary>
		/// 处理好友添加请求
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">申请者QQ</param>
		/// <param name="treatmentMethod">处理方式</param>
		/// <param name="information">拒绝时的附加信息</param>
		public static void HandleFriendAddRequest (long robot, long qq, ChatRobotRequestType treatmentMethod, string information) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (HandleFriendAddRequest) },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "TreatmentMethod", (int)treatmentMethod },
				{ "Information", information }
			});
		}

		/// <summary>
		/// 处理群添加请求
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="requestType">请求类型</param>
		/// <param name="qq">申请者QQ</param>
		/// <param name="group">群</param>
		/// <param name="sign">请求标记</param>
		/// <param name="treatmentMethod">处理方式</param>
		/// <param name="information">拒绝时的附加信息</param>
		public static void HandleGroupAddRequest (long robot, ChatRobotGroupAddRequestType requestType, long qq, long group, long sign,
			ChatRobotRequestType treatmentMethod, string information
		) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (HandleGroupAddRequest) },
				{ "Robot", robot },
				{ "RequestType", (int)requestType },
				{ "QQ", qq },
				{ "Group", group },
				{ "Tag", sign },
				{ "TreatmentMethod", (int)treatmentMethod },
				{ "Information", information }
			});
		}

		public static string CreateDiscuss (long robot, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (CreateDiscuss) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public static void LoginRobot (long robot) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (LoginRobot) },
				{ "Robot", robot }
			});
		}

		public static string Like (long robot, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (Like) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public static string PublishGroupAnnouncement (long robot, long group, string title, string content) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (PublishGroupAnnouncement) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Title", title },
				{ "Content", content }
			});
		}

		public static string PublishGroupJob (long robot, long group, string name, string title, string content) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (PublishGroupJob) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Name", name },
				{ "Title", title },
				{ "Content", content }
			});
		}

		public static void SendFriendJsonMessage (long robot, long qq, string message) {
			SendFriendMessage ("SendFriendJsonMessage", robot, qq, message);
		}

		public static void SendFriendXmlMessage (long robot, long qq, string message) {
			SendFriendMessage ("SendFriendXmlMessage", robot, qq, message);
		}

		public static bool SendFriendWindowJitter (long robot, long qq) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SendFriendWindowJitter) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public static void SendFriendMessage (long robot, long qq, string message) {
			SendFriendMessage ("SendFriendMessage", robot, qq, message);
		}

		public static void SendFriendVerificationReplyJsonMessage (long robot, long qq, string message) {
			SendFriendMessage ("SendFriendVerificationReplyJsonMessage", robot, qq, message);
		}

		public static void SendFriendVerificationReplyXmlMessage (long robot, long qq, string message) {
			SendFriendMessage ("SendFriendVerificationReplyXmlMessage", robot, qq, message);
		}

		public static void SendFriendVerificationReplyMessage (long robot, long qq, string message) {
			SendFriendMessage ("SendFriendVerificationReplyMessage", robot, qq, message);
		}

		public static bool SendFriendVoice (long robot, long qq, byte[] data) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SendFriendVoice) },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "Data", data }
			});
		}

		public static void SendGroupJsonMessage (long robot, long group, string message) {
			SendGroupMessage ("SendGroupJsonMessage", robot, group, message);
		}

		public static void SendGroupXmlMessage (long robot, long group, string message) {
			SendGroupMessage ("SendGroupXmlMessage", robot, group, message);
		}

		public static bool SendGroupGift (long robot, long group, long qq, long gift) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SendGroupGift) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq },
				{ "Gift", gift }
			});
		}

		public static void SendGroupTempJsonMessage (long robot, long group, long qq, string message) {
			SendGroupTempMessage ("SendGroupTempJsonMessage", robot, group, qq, message);
		}

		public static void SendGroupTempXmlMessage (long robot, long group, long qq, string message) {
			SendGroupTempMessage ("SendGroupTempXmlMessage", robot, group, qq, message);
		}

		public static void SendGroupTempMessage (long robot, long group, long qq, string message) {
			SendGroupTempMessage ("SendGroupTempMessage", robot, group, qq, message);
		}

		public static bool SendGroupSignIn (long robot, long group, string place, string content) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SendGroupSignIn) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Place", place },
				{ "Content", content }
			});
		}

		public static void SendGroupMessage (long robot, long group, string message) {
			SendGroupMessage ("SendGroupMessage", robot, group, message);
		}

		public static string SendData (long robot, string data) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (SendData) },
				{ "Robot", robot },
				{ "Data", data }
			});
		}

		public static void SendDiscussTempJsonMessage (long robot, long group, long qq, string message) {
			SendGroupTempMessage ("SendDiscussTempJsonMessage", robot, group, qq, message);
		}

		public static void SendDiscussJsonMessage (long robot, long group, string message) {
			SendGroupMessage ("SendDiscussJsonMessage", robot, group, message);
		}

		public static void SendDiscussTempXmlMessage (long robot, long group, long qq, string message) {
			SendGroupTempMessage ("SendDiscussTempXmlMessage", robot, group, qq, message);
		}

		public static void SendDiscussXmlMessage (long robot, long group, string message) {
			SendGroupMessage ("SendDiscussXmlMessage", robot, group, message);
		}

		public static void SendDiscussTempMessage (long robot, long group, long qq, string message) {
			SendGroupTempMessage ("SendDiscussTempMessage", robot, group, qq, message);
		}

		public static void SendDiscussMessage (long robot, long group, string message) {
			SendGroupMessage ("SendDiscussMessage", robot, group, message);
		}

		public static void SendWebpageTempJsonMessage (long robot, long qq, string message) {
			SendFriendMessage ("SendWebpageTempJsonMessage", robot, qq, message);
		}

		public static void SendWebpageTempXmlMessage (long robot, long qq, string message) {
			SendFriendMessage ("SendWebpageTempXmlMessage", robot, qq, message);
		}

		public static void SendWebpageTempMessage (long robot, long qq, string message) {
			SendFriendMessage ("SendWebpageTempMessage", robot, qq, message);
		}

		public static string JoinDiscussByUrl (long robot, string url) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (JoinDiscussByUrl) },
				{ "Robot", robot },
				{ "Url", url }
			});
		}

		public static void DisablePlugin () {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (DisablePlugin) }
			});
		}

		public static string GetBkn (long robot) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetBkn) },
				{ "Robot", robot }
			});
		}

		public static string GetClientKey (long robot) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetClientKey) },
				{ "Robot", robot }
			});
		}

		public static string GetCookies (long robot) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetCookies) },
				{ "Robot", robot }
			});
		}

		public static string GetPSKey (long robot, string domainName) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetPSKey) },
				{ "Robot", robot },
				{ "DomainName", domainName }
			});
		}

		public static string GetSessionKey (long robot) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetSessionKey) },
				{ "Robot", robot }
			});
		}

		public static string GetLongBkn (long robot) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetLongBkn) },
				{ "Robot", robot }
			});
		}

		public static string GetLongClientKey (long robot) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetLongClientKey) },
				{ "Robot", robot }
			});
		}

		public static string GetLongLdw (long robot) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetLongLdw) },
				{ "Robot", robot }
			});
		}

		public static string GetMemberGroupChatLevel (long robot, long group, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetMemberGroupChatLevel) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq }
			});
		}

		public static long GetCurrentTimeStamp () {
			return WaitSystemGet<long> (new JObject () {
				{ "Type", nameof (GetCurrentTimeStamp) }
			});
		}

		public static bool TryGetLevel (long robot, out ChatRobotLevelInformation levelInformation) {
			return WaitSystemGet<bool, ChatRobotLevelInformation> (new JObject () {
				{ "Type", nameof (TryGetLevel) },
				{ "Robot", robot }
			}, out levelInformation);
		}

		public static long GetFriendQQMasterDays (long robot, long qq) {
			return WaitSystemGet<long> (new JObject () {
				{ "Type", nameof (GetFriendQQMasterDays) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public static long GetFriendQAge (long robot, long qq) {
			return WaitSystemGet<long> (new JObject () {
				{ "Type", nameof (GetFriendQAge) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public static string GetFriendNotes (long robot, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetFriendNotes) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public static long GetFriendLevel (long robot, long qq) {
			return WaitSystemGet<long> (new JObject () {
				{ "Type", nameof (GetFriendLevel) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public static string GetFriendPersonalDescription (long robot, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetFriendPersonalDescription) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public static string GetFriendPersonalSignature (long robot, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetFriendPersonalSignature) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public static ChatRobotFriendListInformation[] GetFriendListInformations (long robot) {
			return WaitSystemGet<ChatRobotFriendListInformation[]> (new JObject () {
				{ "Type", nameof (GetFriendListInformations) },
				{ "Robot", robot }
			});
		}

		public static long GetFriendAge (long robot, long qq) {
			return WaitSystemGet<long> (new JObject () {
				{ "Type", nameof (GetFriendAge) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public static bool IsFriendOnline (long robot, long qq) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (IsFriendOnline) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public static bool TryGetFriendInformation (long robot, long qq, out ChatRobotFriendInformation friendInformation) {
			return WaitSystemGet<bool, ChatRobotFriendInformation> (new JObject () {
				{ "Type", nameof (TryGetFriendInformation) },
				{ "Robot", robot },
				{ "QQ", qq }
			}, out friendInformation);
		}

		public static long GetFriendGender (long robot, long qq) {
			return WaitSystemGet<long> (new JObject () {
				{ "Type", nameof (GetFriendGender) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public static string GetFriendEmail (long robot, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetFriendEmail) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public static string GetFriendOnlineState (long robot, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetFriendOnlineState) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public static long[] GetFriends (long robot) {
			return WaitSystemGet<long[]> (new JObject () {
				{ "Type", nameof (GetFriends) },
				{ "Robot", robot }
			});
		}

		public static bool TryGetRobotStateInformation (long robot, out ChatRobotStateInformation robotStateInformation) {
			return WaitSystemGet<bool, ChatRobotStateInformation> (new JObject () {
				{ "Type", nameof (TryGetRobotStateInformation) },
				{ "Robot", robot }
			}, out robotStateInformation);
		}

		public static string GetFrameVersionNumber () {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetFrameVersionNumber) }
			});
		}

		public static string GetFrameVersionName () {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetFrameVersionName) }
			});
		}

		public static long[] GetOfflineRobots () {
			return WaitSystemGet<long[]> (new JObject () {
				{ "Type", nameof (GetOfflineRobots) }
			});
		}

		public static long[] GetRobots () {
			return WaitSystemGet<long[]> (new JObject () {
				{ "Type", nameof (GetRobots) }
			});
		}

		public static string GetFrameLog () {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetFrameLog) }
			});
		}

		public static long[] GetOnlineRobots () {
			return WaitSystemGet<long[]> (new JObject () {
				{ "Type", nameof (GetOnlineRobots) }
			});
		}

		public static long GetTargetFriendAddMethod (long robot, long qq) {
			return WaitSystemGet<long> (new JObject () {
				{ "Type", nameof (GetTargetFriendAddMethod) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public static long GetGroupID (long group) {
			return WaitSystemGet<long> (new JObject () {
				{ "Type", nameof (GetGroupID) },
				{ "Group", group }
			});
		}

		public static ChatRobotGroupMemberListInformation GetGroupMemberListInformation (long robot, long group,
			out ChatRobotGroupMemberInformation[] groupMemberInformations
		) {
			return WaitSystemGet<ChatRobotGroupMemberListInformation, ChatRobotGroupMemberInformation[]> (new JObject () {
				{ "Type", nameof (GetGroupMemberListInformation) },
				{ "Robot", robot },
				{ "Group", group }
			}, out groupMemberInformations);
		}

		public static string GetGroupMemberBusinessCard (long robot, long group, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetGroupMemberBusinessCard) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq }
			});
		}

		public static bool IsGroupMemberBanSpeak (long robot, long group, long qq) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (IsGroupMemberBanSpeak) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq }
			});
		}

		public static long[] GetGroupMembers (long robot, long group) {
			return WaitSystemGet<long[]> (new JObject () {
				{ "Type", nameof (GetGroupMembers) },
				{ "Robot", robot },
				{ "Group", group }
			});
		}

		public static string GetGroupAnnouncement (long robot, long group) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetGroupAnnouncement) },
				{ "Robot", robot },
				{ "Group", group }
			});
		}

		public static long[] GetGroupAdministrators (long robot, long group) {
			return WaitSystemGet<long[]> (new JObject () {
				{ "Type", nameof (GetGroupAdministrators) },
				{ "Robot", robot },
				{ "Group", group }
			});
		}

		public static long GetGroupQQ (long id) {
			return WaitSystemGet<long> (new JObject () {
				{ "Type", nameof (GetGroupQQ) },
				{ "GroupID", id }
			});
		}

		public static long[] GetGroups (long robot) {
			return WaitSystemGet<long[]> (new JObject () {
				{ "Type", nameof (GetGroups) },
				{ "Robot", robot }
			});
		}

		public static ChatRobotGroupInformation[] GetGroupInformations (long robot) {
			return WaitSystemGet<ChatRobotGroupInformation[]> (new JObject () {
				{ "Type", nameof (GetGroupInformations) },
				{ "Robot", robot }
			});
		}

		public static string GetGroupName (long robot, long group) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetGroupName) },
				{ "Robot", robot },
				{ "Group", group }
			});
		}

		public static long GetGroupMemberNumber (long robot, long group, out long maxMemberNumber) {
			return WaitSystemGet<long, long> (new JObject () {
				{ "Type", nameof (GetGroupMemberNumber) },
				{ "Robot", robot },
				{ "Group", group }
			}, out maxMemberNumber);
		}

		public static long[] GetDiscussMembers (long robot, long discuss) {
			return WaitSystemGet<long[]> (new JObject () {
				{ "Type", nameof (GetDiscussMembers) },
				{ "Robot", robot },
				{ "Discuss", discuss }
			});
		}

		public static long[] GetDiscusss (long robot) {
			return WaitSystemGet<long[]> (new JObject () {
				{ "Type", nameof (GetDiscusss) },
				{ "Robot", robot }
			});
		}

		public static string GetDiscussURL (long robot, long discuss) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetDiscussURL) },
				{ "Robot", robot },
				{ "Discuss", discuss }
			});
		}

		public static string GetDiscussName (long robot, long discuss) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetDiscussName) },
				{ "Robot", robot },
				{ "Discuss", discuss }
			});
		}

		public static string GetImageURL (long robot, long group, string code) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetImageURL) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Code", code }
			});
		}

		public static string GetVoiceURL (long robot, string code) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetVoiceURL) },
				{ "Robot", robot },
				{ "Code", code }
			});
		}

		public static string GetName (long robot, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetName) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public static bool RemoveFriend (long robot, long qq) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (RemoveFriend) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public static bool RemoveFriendByOneWay (long robot, long qq, long operatorType) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (RemoveFriendByOneWay) },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "OperatorType", operatorType }
			});
		}

		public static void RemoveRobot (long robot) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (RemoveRobot) },
				{ "Robot", robot }
			});
		}

		public static string UploadGroupChatImage (long robot, long group, string base64) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (UploadGroupChatImage) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Data", base64 }
			});
		}
		public static string UploadGroupChatImage (long robot, long group, byte[] data) {
			return UploadGroupChatImage (robot, group, Convert.ToBase64String (data));
		}

		public static string UploadGroupChatVoice (long robot, long group, string base64) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (UploadGroupChatVoice) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Data", base64 }
			});
		}
		public static string UploadGroupChatVoice (long robot, long group, byte[] data) {
			return UploadGroupChatVoice (robot, group, Convert.ToBase64String (data));
		}

		public static string UploadPrivateChatImage (long robot, long qq, string base64) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (UploadPrivateChatImage) },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "Data",base64 }
			});
		}
		public static string UploadPrivateChatImage (long robot, long qq, byte[] data) {
			return UploadPrivateChatImage (robot, qq, Convert.ToBase64String (data));
		}

		public static string RequestAddFriend (long robot, long qq, string message) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (RequestAddFriend) },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "Message", message }
			});
		}

		public static string RequestAddGroup (long robot, long group, string message) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (RequestAddGroup) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Message", message }
			});
		}

		public static bool IsMaskSendMessage (long robot) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (IsMaskSendMessage) },
				{ "Robot", robot }
			});
		}

		public static bool IsEnable () {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (IsEnable) }
			});
		}

		public static bool IsFriend (long robot, long qq) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (IsFriend) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public static bool IsAllowGroupTempMessage (long robot, long group) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (IsAllowGroupTempMessage) },
				{ "Robot", robot },
				{ "Group", group }
			});
		}

		public static bool IsAllowWebpageTempMessage (long robot, long qq) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (IsAllowWebpageTempMessage) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public static bool AddRobot (long robot, string password, bool autoLogin) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (AddRobot) },
				{ "Robot", robot },
				{ "Password", password },
				{ "AutoLogin", autoLogin }
			});
		}

		public static void RemoveGroup (long robot, long group) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (RemoveGroup) },
				{ "Robot", robot },
				{ "Group", group }
			});
		}

		public static void RemoveDiscuss (long robot, long discuss) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (RemoveDiscuss) },
				{ "Robot", robot },
				{ "Discuss", discuss }
			});
		}

		public static void LogoutRobot (long robot) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (LogoutRobot) },
				{ "Robot", robot }
			});
		}

		public static bool InviteFriendJoinDiscuss (long robot, long discuss, long qq) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (InviteFriendJoinDiscuss) },
				{ "Robot", robot },
				{ "Discuss", discuss },
				{ "QQ", qq }
			});
		}

		public static void InviteFriendJoinGroupByAdministrator (long robot, long group, long qq) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (InviteFriendJoinGroupByAdministrator) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq }
			});
		}

		public static void InviteFriendJoinGroupNonAdministrator (long robot, long group, long qq) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (InviteFriendJoinGroupNonAdministrator) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq }
			});
		}

		public static bool SetCover (long robot, string base64) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetCover) },
				{ "Robot", robot },
				{ "Data", base64 }
			});
		}
		public static bool SetCover (long robot, byte[] data) {
			return SetCover (robot, Convert.ToBase64String (data));
		}

		public static void SetPersonalSignature (long robot, string personalSignature) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (SetPersonalSignature) },
				{ "Robot", robot },
				{ "PersonalSignature", personalSignature }
			});
		}

		public static void SetFriendNotes (long robot, long qq, string notes) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (SetFriendNotes) },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "Notes", notes }
			});
		}

		public static void SetFriendBlacklist (long robot, long qq, bool enable) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (SetFriendBlacklist) },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "Enable", enable }
			});
		}

		public static bool SetFriendAuthenticationMethod (long robot, long VerificationMethod, string question, string answer) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetFriendAuthenticationMethod) },
				{ "Robot", robot },
				{ "VerificationMethod", VerificationMethod },
				{ "Question", question },
				{ "Answer", answer }
			});
		}

		public static void SetRobotGender (long robot, long gender) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (SetRobotGender) },
				{ "Robot", robot },
				{ "Gender", gender }
			});
		}

		public static void SetRobotState (long robot, long state, string information) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (SetRobotState) },
				{ "Robot", robot },
				{ "State", state },
				{ "Information", information }
			});
		}

		public static void SetRobotName (long robot, string name) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (SetRobotName) },
				{ "Robot", robot },
				{ "Name", name },
			});
		}

		public static bool SetGroupBanSpeak (long robot, long group, bool enable) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetGroupBanSpeak) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Enable", enable }
			});
		}

		public static bool SetGroupMemberBanSpeak (long robot, long group, long qq, long seconds) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetGroupMemberBanSpeak) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq },
				{ "Seconds", seconds }
			});
		}

		public static bool SetGroupMemberBusinessCard (long robot, long group, long qq, string businessCard) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetGroupMemberBusinessCard) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq },
				{ "BusinessCard", businessCard }
			});
		}

		public static void KickGroupMember (long robot, long group, long qq, bool noLongerAccept) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (KickGroupMember) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq },
				{ "NoLongerAccept", noLongerAccept }
			});
		}

		public static bool SetGroupAdministrator (long robot, long group, long qq, bool enable) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetGroupAdministrator) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq },
				{ "Enable", enable }
			});
		}

		public static bool SetGroupAnonymousEnable (long robot, long group, bool enable) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetGroupAnonymousEnable) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Enable", enable }
			});
		}

		public static void SetMaskGroupMessage (long robot, long group, bool enable) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (SetMaskGroupMessage) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Enable", enable }
			});
		}

		public static void Log (string content) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (Log) },
				{ "Content", content }
			});
		}

		public static void SetInputting (long robot, long qq) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (SetInputting) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public static void KickDiscussMember (long robot, long discuss, long qq) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (KickDiscussMember) },
				{ "Robot", robot },
				{ "Discuss", discuss },
				{ "QQ", qq }
			});
		}

		public static void SetDiscussName (long robot, long discuss, string name) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (SetDiscussName) },
				{ "Robot", robot },
				{ "Discuss", discuss },
				{ "Name", name }
			});
		}

		public static void SetAvatar (long robot, string base64) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (SetAvatar) },
				{ "Robot", robot },
				{ "Data", base64 }
			});
		}
		public static void SetAvatar (long robot, byte[] data) {
			SetAvatar (robot, Convert.ToBase64String (data));
		}

		public static void Disconnect () {
			Client.Disconnect ();
			WaitSystem.Dispose ();
		}

		static void Received (byte[] data) {
			string text = Encoding.UTF8.GetString (data);
			OnReceived?.Invoke (text);
			JObject jObject = JObject.Parse (text);
			switch (jObject.Value<string> ("Type")) {
				case "Protocol": {
					string targetProtocolVersion = jObject.Value<string> ("Version");
					if (targetProtocolVersion != ProtocolVersision) {
						throw new Exception ($"SDK协议版本：{ProtocolVersision} 与机器人框架插件的协议版本：{targetProtocolVersion}不符");
					}
					break;
				}
				case "Return":
					WaitSystem.Set (jObject.Value<long> ("ID"), jObject.Value<string> ("Result"));
					break;
				case "Message":
					OnReceivedMessage?.BeginInvoke (new ChatRobotMessage (
						EnumParse<ChatRobotMessageType> (jObject.Value<string> ("SubType")),
						jObject.Value<long> ("Robot"),
						jObject.Value<long> ("Group"),
						jObject.Value<long> ("QQ"),
						jObject.Value<string> ("Message"),
						jObject.Value<long> ("MessageNumber"),
						jObject.Value<long> ("MessageID")
					), asyncResult => OnReceivedMessage.EndInvoke (asyncResult), null);
					break;
				case "GroupAddRequest":
					OnReceivedGroupAddRequest?.BeginInvoke (
						EnumParse<ChatRobotGroupAddRequestType> (jObject.Value<string> ("SubType")),
						jObject.Value<long> ("Robot"),
						jObject.Value<long> ("Group"),
						jObject.Value<long> ("QQ"),
						jObject.Value<long> ("InviterQQ"),
						jObject.Value<long> ("Sign"),
						jObject.Value<string> ("Message"),
					asyncResult => OnReceivedGroupAddRequest.EndInvoke (asyncResult), null);
					break;
				case "FriendAddResponse":
					OnReceivedFriendAddResponse?.BeginInvoke (
						jObject.Value<bool> ("Agree"),
						jObject.Value<long> ("Robot"),
						jObject.Value<long> ("QQ"),
						jObject.Value<string> ("Message"),
					asyncResult => OnReceivedFriendAddResponse.EndInvoke (asyncResult), null);
					break;
				case "FriendAddRequest":
					OnReceivedFriendAddRequest?.BeginInvoke (
						jObject.Value<long> ("Robot"),
						jObject.Value<long> ("QQ"),
						jObject.Value<string> ("Message"),
					asyncResult => OnReceivedFriendAddRequest.EndInvoke (asyncResult), null);
					break;
				case "GroupMessageRevoke":
					OnGroupMessageRevoked?.BeginInvoke (
						jObject.Value<long> ("Robot"),
						jObject.Value<long> ("Group"),
						jObject.Value<long> ("QQ"),
						jObject.Value<long> ("MessageNumber"),
						jObject.Value<long> ("MessageID"),
					asyncResult => OnGroupMessageRevoked.EndInvoke (asyncResult), null);
					break;
				case "GroupAnonymousSwitch":
					OnGroupAnonymousSwitched?.BeginInvoke (
						jObject.Value<bool> ("Enable"),
						jObject.Value<long> ("Robot"),
						jObject.Value<long> ("Group"),
						jObject.Value<long> ("QQ"),
					asyncResult => OnGroupAnonymousSwitched.EndInvoke (asyncResult), null);
					break;
				case "GroupNameChange":
					OnGroupNameChanged?.BeginInvoke (
						jObject.Value<long> ("Robot"),
						jObject.Value<long> ("Group"),
						jObject.Value<long> ("QQ"),
						jObject.Value<string> ("Name"),
					asyncResult => OnGroupNameChanged.EndInvoke (asyncResult), null);
					break;
				case "GroupBanSpeak":
					OnGroupBannedSpeak?.BeginInvoke (
						jObject.Value<bool> ("Enable"),
						jObject.Value<long> ("Robot"),
						jObject.Value<long> ("Group"),
						jObject.Value<long> ("QQ"),
					asyncResult => OnGroupBannedSpeak.EndInvoke (asyncResult), null);
					break;
				case "GroupAdminChange":
					OnGroupAdministratorChanged?.BeginInvoke (
						jObject.Value<bool> ("Enable"),
						jObject.Value<long> ("Robot"),
						jObject.Value<long> ("Group"),
						jObject.Value<long> ("QQ"),
					asyncResult => OnGroupAdministratorChanged.EndInvoke (asyncResult), null);
					break;
				case "GroupMemberBusinessCardChange":
					OnGroupMemberBusinessCardChanged?.BeginInvoke (
						jObject.Value<long> ("Robot"),
						jObject.Value<long> ("Group"),
						jObject.Value<long> ("QQ"),
						jObject.Value<string> ("BusinessCard"),
					asyncResult => OnGroupMemberBusinessCardChanged.EndInvoke (asyncResult), null);
					break;
				case "GroupMemberLeave":
					OnGroupMemberLeaved?.BeginInvoke (
						jObject.Value<bool> ("Kick"),
						jObject.Value<long> ("Robot"),
						jObject.Value<long> ("Group"),
						jObject.Value<long> ("QQ"),
						jObject.Value<long> ("OperatorQQ"),
					asyncResult => OnGroupMemberLeaved.EndInvoke (asyncResult), null);
					break;
				case "GroupMemberBanSpeak":
					OnGroupMemberBannedSpeak?.BeginInvoke (
						jObject.Value<bool> ("Enable"),
						jObject.Value<long> ("Robot"),
						jObject.Value<long> ("Group"),
						jObject.Value<long> ("QQ"),
						jObject.Value<long> ("OperatorQQ"),
						jObject.Value<long> ("Seconds"),
					asyncResult => OnGroupMemberBannedSpeak.EndInvoke (asyncResult), null);
					break;
				case "GroupMemberJoin":
					OnGroupMemberJoined?.BeginInvoke (
						EnumParse<ChatRobotGroupMemberJoinType> (jObject.Value<string> ("SubType")),
						jObject.Value<long> ("Robot"),
						jObject.Value<long> ("Group"),
						jObject.Value<long> ("QQ"),
						jObject.Value<long> ("OperatorQQ"),
					asyncResult => OnGroupMemberJoined.EndInvoke (asyncResult), null);
					break;
				case "GroupDisband":
					OnGroupDisbanded?.BeginInvoke (
						jObject.Value<long> ("Robot"),
						jObject.Value<long> ("Group"),
						jObject.Value<long> ("QQ"),
					asyncResult => OnGroupDisbanded.EndInvoke (asyncResult), null);
					break;
				case "FriendStateChange":
					OnFriendStateChanged?.BeginInvoke (
						jObject.Value<long> ("Robot"),
						jObject.Value<long> ("QQ"),
						jObject.Value<string> ("State"),
					asyncResult => OnFriendStateChanged.EndInvoke (asyncResult), null);
					break;
				case "WasRemoveByFriend":
					OnWasRemoveByFriend?.BeginInvoke (
						jObject.Value<long> ("Robot"),
						jObject.Value<long> ("QQ"),
					asyncResult => OnWasRemoveByFriend.EndInvoke (asyncResult), null);
					break;
				default:
					Console.WriteLine ($"未知的消息类型：{jObject.Value<string> ("Type")}");
					break;
			}
		}

		static void Sent (string text) {
			OnSent?.Invoke (text);
		}

		static void SendGroupMessage (string type, long robot, long group, string message) {
			ClientBeginSend (new JObject () {
				{ "Type", type },
				{ "Robot", robot },
				{ "Group", group },
				{ "Message", message }
			});
		}

		static void SendFriendMessage (string type, long robot, long qq, string message) {
			ClientBeginSend (new JObject () {
				{ "Type", type },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "Message", message }
			});
		}

		static void SendGroupTempMessage (string type, long robot, long group, long qq, string message) {
			ClientBeginSend (new JObject () {
				{ "Type", type },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq },
				{ "Message", message }
			});
		}

		static T EnumParse<T> (string value) {
			return (T)Enum.Parse (typeof (T), value);
		}

		static long WaitSystemSend (JObject jObject) {
			long id = WaitSystem.GetID ();
			jObject["ID"] = id;
			Client.BeginSend (jObject.ToString (Formatting.None));
			return id;
		}

		static T WaitSystemConvert<T> (string result) {
			if (typeof (T).IsValueType || typeof (T) == typeof (string)) {
				if (typeof (T) == typeof (bool)) {
					if (result == "真") {
						result = "true";
					} else if (result == "假") {
						result = "false";
					}
				}
				return (T)Convert.ChangeType (result, typeof (T));
			}
			return JsonConvert.DeserializeObject<T> (result);
		}

		static T WaitSystemGet<T> (JObject jObject) {
			return WaitSystemConvert<T> (WaitSystem.Get (WaitSystemSend (jObject)));
		}
		static Result WaitSystemGet<Result, T> (JObject jObject, out T arg) {
			JArray result = JArray.Parse (WaitSystem.Get (WaitSystemSend (jObject)));
			arg = WaitSystemConvert<T> (result[1].Value<string> ());
			return WaitSystemConvert<Result> (result[0].Value<string> ());
		}
		static Result WaitSystemGet<Result, T1, T2> (JObject jObject, out T1 arg1, out T2 arg2) {
			JArray result = JArray.Parse (WaitSystem.Get (WaitSystemSend (jObject)));
			arg1 = WaitSystemConvert<T1> (result[1].Value<string> ());
			arg2 = WaitSystemConvert<T2> (result[2].Value<string> ());
			return WaitSystemConvert<Result> (result[0].Value<string> ());
		}

		static void ClientBeginSend (JObject jObject) {
			Client.BeginSend (jObject.ToString (Formatting.None));
		}

	}

}