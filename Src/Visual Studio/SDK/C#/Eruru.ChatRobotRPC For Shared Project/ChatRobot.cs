using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Authentication;
using System.Text;

namespace Eruru.ChatRobotRPC {

	/// <summary>
	/// 聊天机器人
	/// </summary>
	public class ChatRobot {

		/// <summary>
		/// 协议版本
		/// </summary>
		public string ProtocolVersision { get; } = "1.0.0.2";
		/// <summary>
		/// 收到消息（底层协议消息）
		/// </summary>
		public Action<string> OnReceived { get; set; }
		/// <summary>
		/// 发送消息（底层协议消息）
		/// </summary>
		public Action<string> OnSend { get; set; }
		/// <summary>
		/// 收到消息（好友或群等，统一入口，可用ChatRobot.UseAsyncReceive决定是否异步处理所有消息）
		/// </summary>
		public Action<ChatRobotMessage> OnReceivedMessage { get; set; }
		/// <summary>
		/// 收到群添加请求（使用ChatRobot.HandleGroupAddRequest处理）
		/// </summary>
		public ChatRobotGroupAddRequestEventHandler OnReceivedGroupAddRequest { get; set; }
		/// <summary>
		/// 收到好友添加响应
		/// </summary>
		public ChatRobotFriendAddResponsedEventHandler OnReceivedFriendAddResponse { get; set; }
		/// <summary>
		/// 收到好友添加请求（使用ChatRobot.HandleFriendAddRequest处理）
		/// </summary>
		public ChatRobotFriendAddRequestedEventHandler OnReceivedFriendAddRequest { get; set; }
		/// <summary>
		/// 群消息撤回
		/// </summary>
		public ChatRobotGroupMessageRevokedEventHandler OnGroupMessageRevoked { get; set; }
		/// <summary>
		/// 群匿名开关
		/// </summary>
		public ChatRobotGroupAnonymousSwitchedEventHandler OnGroupAnonymousSwitched { get; set; }
		/// <summary>
		/// 群名改变
		/// </summary>
		public ChatRobotGroupNameChangedEventHandler OnGroupNameChanged { get; set; }
		/// <summary>
		/// 群禁言
		/// </summary>
		public ChatRobotGroupBannedSpeakEventHandler OnGroupBannedSpeak { get; set; }
		/// <summary>
		/// 群管理员改变
		/// </summary>
		public ChatRobotGroupAdministratorChangedEventHandler OnGroupAdministratorChanged { get; set; }
		/// <summary>
		/// 群成员名片改变
		/// </summary>
		public ChatRobotGroupMemberBusinessCardChangedEventHandler OnGroupMemberBusinessCardChanged { get; set; }
		/// <summary>
		/// 群成员离开
		/// </summary>
		public ChatRobotGroupMemberLeftEventHandler OnGroupMemberLeft { get; set; }
		/// <summary>
		/// 群成员禁言
		/// </summary>
		public ChatRobotGroupMemberBannedSpeakEventHandler OnGroupMemberBannedSpeak { get; set; }
		/// <summary>
		/// 群成员加入
		/// </summary>
		public ChatRobotGroupMemberJoinedEventHandler OnGroupMemberJoined { get; set; }
		/// <summary>
		/// 群被解散
		/// </summary>
		public ChatRobotGroupDisbandedEventHandler OnGroupDisbanded { get; set; }
		/// <summary>
		/// 好友状态改变
		/// </summary>
		public ChatRobotFriendStateChangedEventHandler OnFriendStateChanged { get; set; }
		/// <summary>
		/// 被好友删除
		/// </summary>
		public ChatRobotWasRemovedByFriendEventHandler OnWasRemovedByFriend { get; set; }
		/// <summary>
		/// 与机器人框架RPC插件断开了连接
		/// </summary>
		public ChatRobotAction OnDisconnected { get; set; }
		/// <summary>
		/// 心跳包发送间隔（秒）
		/// </summary>
		public int HeartbeatInterval {

			get => SocketClient.HeartbeatInterval;

			set => SocketClient.HeartbeatInterval = value;

		}
		/// <summary>
		/// 是否使用异步处理所有消息
		/// </summary>
		public bool UseAsyncReceive {

			get {
				return SocketClient.UseAsyncOnReceived;
			}

			set {
				SocketClient.UseAsyncOnReceived = value;
			}

		}

		readonly SocketClient SocketClient;
		readonly WaitSystem WaitSystem = new WaitSystem ();
		readonly Encoding Encoding = Encoding.UTF8;

		/// <summary>
		/// 默认构造函数
		/// </summary>
		public ChatRobot () {
			SocketClient = new SocketClient () {
				OnReceived = SocketClient_OnReceived,
				OnSend = bytes => OnSend?.Invoke (Encoding.GetString (bytes)),
				OnDisconnected = () => OnDisconnected?.Invoke ()
			};
		}

		/// <summary>
		/// 连接机器人框架RPC插件（使用ChatRobot.OnDisconnected事件获知断开连接）
		/// </summary>
		/// <param name="ip"></param>
		/// <param name="port"></param>
		/// <param name="account"></param>
		/// <param name="password"></param>
		public void Connect (string ip, int port, string account, string password) {
			SocketClient.Connect (ip, port);
			if (!WaitSystemGet<bool> (new JObject () {
				{ "Type", "Login" },
				{ "Account", account },
				{ "Password", password }
			})) {
				throw new AuthenticationException ("账号或密码错误");
			}
		}

#if DEBUG
		public void Test (long robot) {
			ClientBeginSend (new JObject () {
				{ "Type", "Test" },
				{ "Robot",robot  }
			});
		}
#endif

		/// <summary>
		/// Tea加密
		/// </summary>
		/// <param name="content">需加密的内容</param>
		/// <param name="key"></param>
		/// <returns></returns>
		public string TEAEncryption (string content, string key) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (TEAEncryption) },
				{ "Content", content },
				{ "Key", key }
			});
		}

		/// <summary>
		/// Tea解密
		/// </summary>
		/// <param name="content">需解密的内容</param>
		/// <param name="key"></param>
		/// <returns></returns>
		public string TEADecryption (string content, string key) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (TEADecryption) },
				{ "Content", content },
				{ "Key", key }
			});
		}

		/// <summary>
		/// 查询我的群礼物 QQMini Pro才可用 返回礼物数量
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <returns>礼物列表</returns>
		public ChatRobotGiftInformation[] QueryGroupGiftInformations (long robot) {
			return WaitSystemGet<ChatRobotGiftInformation[]> (new JObject () {
				{ "Type", nameof (QueryGroupGiftInformations) },
				{ "Robot", robot }
			});
		}

		/// <summary>
		/// 撤回群消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">需撤回消息群号</param>
		/// <param name="messageNumber">需撤回消息序号</param>
		/// <param name="messageID">需撤回消息ID</param>
		/// <returns></returns>
		public string RevokeGroupMessage (long robot, long group, long messageNumber, long messageID) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (RevokeGroupMessage) },
				{ "Robot", robot },
				{ "Group", group },
				{ "MessageNumber", messageNumber },
				{ "MessageID", messageID }
			});
		}

		/// <summary>
		/// 抽取群礼物
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">目标群号</param>
		/// <returns></returns>
		public long DrawGroupGift (long robot, long group) {
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
		/// <param name="qq">请求添加好友人QQ</param>
		/// <param name="treatmentMethod">处理方式</param>
		/// <param name="information">拒绝添加好友 附加信息</param>
		public void HandleFriendAddRequest (long robot, long qq, ChatRobotRequestType treatmentMethod, string information) {
			SocketClientBeginSend (new JObject () {
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
		/// <param name="qq">申请入群 被邀请人 （当请求类型为某人被邀请时这里为邀请人QQ）</param>
		/// <param name="group">收到请求群号</param>
		/// <param name="sign">需要处理请求的标记</param>
		/// <param name="treatmentMethod">处理方式</param>
		/// <param name="information">拒绝入群 附加信息</param>
		public void HandleGroupAddRequest (long robot, ChatRobotGroupAddRequestType requestType, long qq, long group, long sign,
			ChatRobotRequestType treatmentMethod, string information
		) {
			SocketClientBeginSend (new JObject () {
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

		/// <summary>
		/// 创建一个讨论组 成功返回讨论组ID 失败返回原因
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">被邀请对象QQ</param>
		/// <returns></returns>
		public string CreateDiscuss (long robot, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (CreateDiscuss) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 登录指定QQ，应确保QQ号码在列表中已存在
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		public void LoginRobot (long robot) {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (LoginRobot) },
				{ "Robot", robot }
			});
		}

		/// <summary>
		/// 调用一次点一下，成功返回空，失败返回理由如：每天最多给他点十个赞哦，调用此Api时应注意频率，每人每日可被赞10次，每日每Q最多可赞50人
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">填写被赞人QQ</param>
		/// <returns></returns>
		public string Like (long robot, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (Like) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 发布群公告，调用此API应保证响应QQ为管理员 成功返回空,失败返回错误信息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">欲发布公告的群号</param>
		/// <param name="title">公告标题</param>
		/// <param name="content">公告内容</param>
		/// <returns></returns>
		public string PublishGroupAnnouncement (long robot, long group, string title, string content) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (PublishGroupAnnouncement) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Title", title },
				{ "Content", content }
			});
		}

		/// <summary>
		/// QQ群作业发布
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">需要发布的群号</param>
		/// <param name="name">作业名</param>
		/// <param name="title">作业标题</param>
		/// <param name="content">作业内容</param>
		/// <returns></returns>
		public string PublishGroupJob (long robot, long group, string name, string title, string content) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (PublishGroupJob) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Name", name },
				{ "Title", title },
				{ "Content", content }
			});
		}

		/// <summary>
		/// 发送好友JSON消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">好友的QQ号</param>
		/// <param name="message">信息内容</param>
		public void SendFriendJsonMessage (long robot, long qq, string message) {
			SendFriendMessage ("SendFriendJsonMessage", robot, qq, message);
		}

		/// <summary>
		/// 发送好友XML消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">好友的QQ号</param>
		/// <param name="message">信息内容</param>
		public void SendFriendXmlMessage (long robot, long qq, string message) {
			SendFriendMessage ("SendFriendXmlMessage", robot, qq, message);
		}

		/// <summary>
		/// 向好友发起窗口抖动，调用此Api腾讯会限制频率
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">接收抖动消息的QQ</param>
		/// <returns></returns>
		public bool SendFriendWindowJitter (long robot, long qq) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SendFriendWindowJitter) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 发送好友普通文本消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">好友的QQ号</param>
		/// <param name="message">信息内容</param>
		public void SendFriendMessage (long robot, long qq, string message) {
			SendFriendMessage ("SendFriendMessage", robot, qq, message);
		}

		/// <summary>
		/// 发送好友验证回复JSON消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">对方的QQ号</param>
		/// <param name="message">信息内容</param>
		public void SendFriendVerificationReplyJsonMessage (long robot, long qq, string message) {
			SendFriendMessage ("SendFriendVerificationReplyJsonMessage", robot, qq, message);
		}

		/// <summary>
		/// 发送好友验证回复XML消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">对方的QQ号</param>
		/// <param name="message">信息内容</param>
		public void SendFriendVerificationReplyXmlMessage (long robot, long qq, string message) {
			SendFriendMessage ("SendFriendVerificationReplyXmlMessage", robot, qq, message);
		}

		/// <summary>
		/// 发送好友验证回复普通文本消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">对方的QQ号</param>
		/// <param name="message">信息内容</param>
		public void SendFriendVerificationReplyMessage (long robot, long qq, string message) {
			SendFriendMessage ("SendFriendVerificationReplyMessage", robot, qq, message);
		}

		/// <summary>
		/// 好友语音上传并发送 （成功返回真 失败返回假） QQMini Pro才可用
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">要发送的QQ号</param>
		/// <param name="data">语音字节集数据（AMR Silk编码）</param>
		/// <returns></returns>
		public bool SendFriendVoice (long robot, long qq, byte[] data) {
			throw new NotImplementedException ();
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SendFriendVoice) },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "Data", data }
			});
		}

		/// <summary>
		/// 发送群JSON消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">要发送消息的群号</param>
		/// <param name="message">信息内容</param>
		/// <param name="isAnonymous">是否匿名（仅Pro有效）</param>
		public void SendGroupJsonMessage (long robot, long group, string message, bool isAnonymous = false) {
			SendGroupMessage ("SendGroupJsonMessage", robot, group, message, isAnonymous);
		}

		/// <summary>
		/// 发送群XML消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">要发送消息的群号</param>
		/// <param name="message">信息内容</param>
		/// 	/// <param name="isAnonymous">是否匿名（仅Pro有效）</param>
		public void SendGroupXmlMessage (long robot, long group, string message, bool isAnonymous = false) {
			SendGroupMessage ("SendGroupXmlMessage", robot, group, message, isAnonymous);
		}

		/// <summary>
		/// 送群礼物 成功返回真 失败返回假
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">需送礼物群号</param>
		/// <param name="qq">赠予礼物对象</param>
		/// <param name="gift">礼物id</param>
		/// <returns></returns>
		public bool SendGroupGift (long robot, long group, long qq, long gift) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SendGroupGift) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq },
				{ "Gift", gift }
			});
		}

		/// <summary>
		/// 发送群临时JSON消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">对方所在群号</param>
		/// <param name="qq">对方的QQ号</param>
		/// <param name="message">信息内容</param>
		public void SendGroupTempJsonMessage (long robot, long group, long qq, string message) {
			SendGroupTempMessage ("SendGroupTempJsonMessage", robot, group, qq, message);
		}

		/// <summary>
		/// 发送群临时XML消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">对方所在群号</param>
		/// <param name="qq">对方的QQ号</param>
		/// <param name="message">信息内容</param>
		public void SendGroupTempXmlMessage (long robot, long group, long qq, string message) {
			SendGroupTempMessage ("SendGroupTempXmlMessage", robot, group, qq, message);
		}

		/// <summary>
		/// 发送群临时普通文本消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">对方所在群号</param>
		/// <param name="qq">对方的QQ号</param>
		/// <param name="message">信息内容</param>
		public void SendGroupTempMessage (long robot, long group, long qq, string message) {
			SendGroupTempMessage ("SendGroupTempMessage", robot, group, qq, message);
		}

		/// <summary>
		/// QQ群签到（成功返回真 失败返回假
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">QQ群号</param>
		/// <param name="place">签到地名（Pro可自定义）</param>
		/// <param name="content">想发表的内容</param>
		/// <returns></returns>
		public bool SendGroupSignIn (long robot, long group, string place, string content) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SendGroupSignIn) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Place", place },
				{ "Content", content }
			});
		}

		/// <summary>
		/// 发送群普通文本消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">要发送消息的群号</param>
		/// <param name="message">信息内容</param>
		/// 	/// <param name="isAnonymous">是否匿名（仅Pro有效）</param>
		public void SendGroupMessage (long robot, long group, string message, bool isAnonymous = false) {
			SendGroupMessage ("SendGroupMessage", robot, group, message, isAnonymous);
		}

		/// <summary>
		/// 向服务器发送原始封包（成功返回服务器返回的包 失败返回空）
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="data">封包内容</param>
		/// <returns></returns>
		public string SendData (long robot, string data) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (SendData) },
				{ "Robot", robot },
				{ "Data", data }
			});
		}

		/// <summary>
		/// 发送讨论组临时JSON消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">对方所在讨论组号</param>
		/// <param name="qq">对方的QQ号</param>
		/// <param name="message">信息内容</param>
		public void SendDiscussTempJsonMessage (long robot, long group, long qq, string message) {
			SendGroupTempMessage ("SendDiscussTempJsonMessage", robot, group, qq, message);
		}

		/// <summary>
		/// 发送讨论组JSON消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">讨论组号</param>
		/// <param name="message">信息内容</param>
		public void SendDiscussJsonMessage (long robot, long group, string message) {
			SendGroupMessage ("SendDiscussJsonMessage", robot, group, message, false);
		}

		/// <summary>
		/// 发送讨论组临时XML消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">对方所在讨论组号</param>
		/// <param name="qq">对方的QQ号</param>
		/// <param name="message">信息内容</param>
		public void SendDiscussTempXmlMessage (long robot, long group, long qq, string message) {
			SendGroupTempMessage ("SendDiscussTempXmlMessage", robot, group, qq, message);
		}

		/// <summary>
		/// 发送讨论组XML消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">讨论组号</param>
		/// <param name="message">信息内容</param>
		public void SendDiscussXmlMessage (long robot, long group, string message) {
			SendGroupMessage ("SendDiscussXmlMessage", robot, group, message, false);
		}

		/// <summary>
		/// 发送讨论组临时普通文本消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">对方所在讨论组号</param>
		/// <param name="qq">对方的QQ号</param>
		/// <param name="message">信息内容</param>
		public void SendDiscussTempMessage (long robot, long group, long qq, string message) {
			SendGroupTempMessage ("SendDiscussTempMessage", robot, group, qq, message);
		}

		/// <summary>
		/// 发送讨论组普通文本消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">讨论组号</param>
		/// <param name="message">信息内容</param>
		public void SendDiscussMessage (long robot, long group, string message) {
			SendGroupMessage ("SendDiscussMessage", robot, group, message, false);
		}

		/// <summary>
		/// 发送网页临时JSON消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">对方的QQ号</param>
		/// <param name="message">信息内容</param>
		public void SendWebpageTempJsonMessage (long robot, long qq, string message) {
			SendFriendMessage ("SendWebpageTempJsonMessage", robot, qq, message);
		}

		/// <summary>
		/// 发送网页临时XML消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">对方的QQ号</param>
		/// <param name="message">信息内容</param>
		public void SendWebpageTempXmlMessage (long robot, long qq, string message) {
			SendFriendMessage ("SendWebpageTempXmlMessage", robot, qq, message);
		}

		/// <summary>
		/// 发送网页临时普通文本消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">对方的QQ号</param>
		/// <param name="message">信息内容</param>
		public void SendWebpageTempMessage (long robot, long qq, string message) {
			SendFriendMessage ("SendWebpageTempMessage", robot, qq, message);
		}

		/// <summary>
		/// 把好友图片的GUID转换成群聊可用的GUID
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="picture">例：[pic={30055346-3524074609-441EE15D1D802AA41D0396A7C303CD93}.jpg]</param>
		/// <returns></returns>
		public string FriendPictureToGroupPicture (long robot, string picture) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (FriendPictureToGroupPicture) },
				{ "Robot", robot },
				{ "Picture", picture }
			});
		}

		/// <summary>
		/// 通过连接加入讨论组
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="url">讨论组链接</param>
		/// <returns></returns>
		public string JoinDiscussByUrl (long robot, string url) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (JoinDiscussByUrl) },
				{ "Robot", robot },
				{ "Url", url }
			});
		}

		/// <summary>
		/// 请求禁用插件自身
		/// </summary>
		public void DisablePlugin () {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (DisablePlugin) }
			});
		}

		/// <summary>
		/// 取得机器人网页操作用参数Bkn或G_tk Pro可用
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <returns></returns>
		public string GetBkn (long robot) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetBkn) },
				{ "Robot", robot }
			});
		}

		/// <summary>
		/// 取得机器人网页操作用的Clientkey Pro可用
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <returns></returns>
		public string GetClientKey (long robot) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetClientKey) },
				{ "Robot", robot }
			});
		}

		/// <summary>
		/// 取得机器人网页操作用的Cookies Pro可用
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <returns></returns>
		public string GetCookies (long robot) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetCookies) },
				{ "Robot", robot }
			});
		}

		/// <summary>
		/// 取得机器人网页操作用的P_skey Pro可用
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="domainName">t.qq.com；qzone.qq.com；qun.qq.com；ke.qq.com</param>
		/// <returns></returns>
		public string GetPSKey (long robot, string domainName) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetPSKey) },
				{ "Robot", robot },
				{ "DomainName", domainName }
			});
		}

		/// <summary>
		/// 获取会话SessionKey密钥 Pro可用
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <returns></returns>
		public string GetSessionKey (long robot) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetSessionKey) },
				{ "Robot", robot }
			});
		}

		/// <summary>
		/// 取得机器人网页操作用参数长Bkn或长G_tk Pro可用
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <returns></returns>
		public string GetLongBkn (long robot) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetLongBkn) },
				{ "Robot", robot }
			});
		}

		/// <summary>
		/// 取得机器人网页操作用的长Clientkey Pro可用
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <returns></returns>
		public string GetLongClientKey (long robot) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetLongClientKey) },
				{ "Robot", robot }
			});
		}

		/// <summary>
		/// 取得机器人网页操作用参数长Ldw Pro可用
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <returns></returns>
		public string GetLongLdw (long robot) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetLongLdw) },
				{ "Robot", robot }
			});
		}

		/// <summary>
		/// 查询对象或自身群聊等级
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">查询群号</param>
		/// <param name="qq">需查询对象或机器人QQ</param>
		/// <returns></returns>
		public string GetMemberGroupChatLevel (long robot, long group, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetMemberGroupChatLevel) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 取当前框架内部时间戳
		/// </summary>
		/// <returns></returns>
		public long GetCurrentTimeStamp () {
			return WaitSystemGet<long> (new JObject () {
				{ "Type", nameof (GetCurrentTimeStamp) }
			});
		}

		/// <summary>
		/// 获取等级 活跃天数 升级剩余活跃天数	
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="levelInformation">等级信息</param>
		/// <returns></returns>
		public bool TryGetLevel (long robot, out ChatRobotLevelInformation levelInformation) {
			return WaitSystemGet<bool, ChatRobotLevelInformation> (new JObject () {
				{ "Type", nameof (TryGetLevel) },
				{ "Robot", robot }
			}, out levelInformation);
		}

		/// <summary>
		/// 查询对象或自身QQ达人天数（返回实际天数 失败返回-1）
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">需查询对象或机器人QQ</param>
		/// <returns></returns>
		public long GetFriendQQMasterDays (long robot, long qq) {
			return WaitSystemGet<long> (new JObject () {
				{ "Type", nameof (GetFriendQQMasterDays) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 取Q龄 成功返回Q龄 失败返回-1
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">好友QQ号</param>
		/// <returns></returns>
		public long GetFriendQAge (long robot, long qq) {
			return WaitSystemGet<long> (new JObject () {
				{ "Type", nameof (GetFriendQAge) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 取好友备注姓名（成功返回备注，失败或无备注返回空）
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">需获取对象好友QQ</param>
		/// <returns></returns>
		public string GetFriendNotes (long robot, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetFriendNotes) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 返回好友等级
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">好友QQ号</param>
		/// <returns></returns>
		public long GetFriendLevel (long robot, long qq) {
			return WaitSystemGet<long> (new JObject () {
				{ "Type", nameof (GetFriendLevel) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 取个人说明
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">好友QQ号</param>
		/// <returns></returns>
		public string GetFriendPersonalDescription (long robot, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetFriendPersonalDescription) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 取个人签名
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">好友QQ号</param>
		/// <returns></returns>
		public string GetFriendPersonalSignature (long robot, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetFriendPersonalSignature) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 取好友信息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <returns></returns>
		public ChatRobotFriendListInformation[] GetFriendListInformations (long robot) {
			return WaitSystemGet<ChatRobotFriendListInformation[]> (new JObject () {
				{ "Type", nameof (GetFriendListInformations) },
				{ "Robot", robot }
			});
		}

		/// <summary>
		/// 取 成功返回年龄 失败返回-1
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">好友QQ号</param>
		/// <returns></returns>
		public long GetFriendAge (long robot, long qq) {
			return WaitSystemGet<long> (new JObject () {
				{ "Type", nameof (GetFriendAge) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 查询对象是否在线
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">需获取对象QQ</param>
		/// <returns></returns>
		public bool IsFriendOnline (long robot, long qq) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (IsFriendOnline) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 获取好友资料 此方式为http，调用时应自行注意控制频率
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">好友QQ号</param>
		/// <param name="friendInformation">好友信息</param>
		/// <returns></returns>
		public bool TryGetFriendInformation (long robot, long qq, out ChatRobotFriendInformation friendInformation) {
			return WaitSystemGet<bool, ChatRobotFriendInformation> (new JObject () {
				{ "Type", nameof (TryGetFriendInformation) },
				{ "Robot", robot },
				{ "QQ", qq }
			}, out friendInformation);
		}

		/// <summary>
		/// 取对象性别 1男 2女 未知或失败返回-1
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">好友QQ号</param>
		/// <returns></returns>
		public long GetFriendGender (long robot, long qq) {
			return WaitSystemGet<long> (new JObject () {
				{ "Type", nameof (GetFriendGender) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 取邮箱，获取对象QQ资料内邮箱栏为邮箱时返回
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">好友QQ号</param>
		/// <returns></returns>
		public string GetFriendEmail (long robot, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetFriendEmail) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 查询对象在线状态 返回 #状态_ 常量 离线或隐身都返回#状态_隐身
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">需获取对象QQ</param>
		/// <returns></returns>
		public string GetFriendOnlineState (long robot, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetFriendOnlineState) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 取好友账号
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <returns></returns>
		public long[] GetFriends (long robot) {
			return WaitSystemGet<long[]> (new JObject () {
				{ "Type", nameof (GetFriends) },
				{ "Robot", robot }
			});
		}

		/// <summary>
		/// 获取机器人状态信息，成功返回真，失败返回假
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="robotStateInformation">返回状态信息</param>
		/// <returns></returns>
		public bool TryGetRobotStateInformation (long robot, out ChatRobotStateInformation robotStateInformation) {
			return WaitSystemGet<bool, ChatRobotStateInformation> (new JObject () {
				{ "Type", nameof (TryGetRobotStateInformation) },
				{ "Robot", robot }
			}, out robotStateInformation);
		}

		/// <summary>
		/// 取框架版本号
		/// </summary>
		/// <returns></returns>
		public string GetFrameVersionNumber () {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetFrameVersionNumber) }
			});
		}

		/// <summary>
		/// 取框架版本名，返回QQMini Air或QQMini Pro
		/// </summary>
		/// <returns></returns>
		public string GetFrameVersionName () {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetFrameVersionName) }
			});
		}

		/// <summary>
		/// 取框架离线QQ账号
		/// </summary>
		/// <returns></returns>
		public long[] GetOfflineRobots () {
			return WaitSystemGet<long[]> (new JObject () {
				{ "Type", nameof (GetOfflineRobots) }
			});
		}

		/// <summary>
		/// 取框架所有QQ账号 包括未登录以及登录失败的QQ
		/// </summary>
		/// <returns></returns>
		public long[] GetRobots () {
			return WaitSystemGet<long[]> (new JObject () {
				{ "Type", nameof (GetRobots) }
			});
		}

		/// <summary>
		/// 取框架日志
		/// </summary>
		/// <returns></returns>
		public string GetFrameLog () {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetFrameLog) }
			});
		}

		/// <summary>
		/// 取框架在线可用的QQ账号
		/// </summary>
		/// <returns></returns>
		public long[] GetOnlineRobots () {
			return WaitSystemGet<long[]> (new JObject () {
				{ "Type", nameof (GetOnlineRobots) }
			});
		}

		/// <summary>
		/// 取对象好友添加验证方式 返回常量 #验证_
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">需获取对象QQ</param>
		/// <returns></returns>
		public ChatRobotFriendAddMethod GetTargetFriendAddMethod (long robot, long qq) {
			return WaitSystemGet<ChatRobotFriendAddMethod> (new JObject () {
				{ "Type", nameof (GetTargetFriendAddMethod) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 群号转群ID
		/// </summary>
		/// <param name="group"></param>
		/// <returns></returns>
		public long GetGroupID (long group) {
			return WaitSystemGet<long> (new JObject () {
				{ "Type", nameof (GetGroupID) },
				{ "Group", group }
			});
		}

		/// <summary>
		/// 取群成员列表信息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">指定群号</param>
		/// <param name="groupMemberInformations">群成员信息</param>
		/// <returns></returns>
		public ChatRobotGroupMemberListInformation GetGroupMemberListInformation (long robot, long group,
			out ChatRobotGroupMemberInformation[] groupMemberInformations
		) {
			return WaitSystemGet<ChatRobotGroupMemberListInformation, ChatRobotGroupMemberInformation[]> (new JObject () {
				{ "Type", nameof (GetGroupMemberListInformation) },
				{ "Robot", robot },
				{ "Group", group }
			}, out groupMemberInformations);
		}

		/// <summary>
		/// 取对象群名片
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">QQ群号</param>
		/// <param name="qq">欲取得群名片的群成员QQ号</param>
		/// <returns></returns>
		public string GetGroupMemberBusinessCard (long robot, long group, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetGroupMemberBusinessCard) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 根据群号+QQ判断指定群员是否被禁言 获取失败的情况下亦会返回假
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">要查询的群号</param>
		/// <param name="qq">要查询的QQ号</param>
		/// <returns></returns>
		public bool IsGroupMemberBanSpeak (long robot, long group, long qq) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (IsGroupMemberBanSpeak) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 取群成员账号
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">指定群号</param>
		/// <returns></returns>
		public long[] GetGroupMembers (long robot, long group) {
			return WaitSystemGet<long[]> (new JObject () {
				{ "Type", nameof (GetGroupMembers) },
				{ "Robot", robot },
				{ "Group", group }
			});
		}

		/// <summary>
		/// 取群公告
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group"> 欲取得公告的群号 </param>
		/// <returns></returns>
		public string GetGroupAnnouncement (long robot, long group) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetGroupAnnouncement) },
				{ "Robot", robot },
				{ "Group", group }
			});
		}

		/// <summary>
		/// 取群管理员QQ（包含群主）
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">指定群号</param>
		/// <returns></returns>
		public long[] GetGroupAdministrators (long robot, long group) {
			return WaitSystemGet<long[]> (new JObject () {
				{ "Type", nameof (GetGroupAdministrators) },
				{ "Robot", robot },
				{ "Group", group }
			});
		}

		/// <summary>
		/// 群ID转群号
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public long GetGroupQQ (long id) {
			return WaitSystemGet<long> (new JObject () {
				{ "Type", nameof (GetGroupQQ) },
				{ "GroupID", id }
			});
		}

		/// <summary>
		/// 取群号
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <returns></returns>
		public long[] GetGroups (long robot) {
			return WaitSystemGet<long[]> (new JObject () {
				{ "Type", nameof (GetGroups) },
				{ "Robot", robot }
			});
		}

		/// <summary>
		/// 取群信息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <returns></returns>
		public ChatRobotGroupInformation[] GetGroupInformations (long robot) {
			return WaitSystemGet<ChatRobotGroupInformation[]> (new JObject () {
				{ "Type", nameof (GetGroupInformations) },
				{ "Robot", robot }
			});
		}

		/// <summary>
		/// 取群名
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">QQ群号</param>
		/// <returns></returns>
		public string GetGroupName (long robot, long group) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetGroupName) },
				{ "Robot", robot },
				{ "Group", group }
			});
		}

		/// <summary>
		/// 查询对象群当前人数和上限人数
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">需查询的群号</param>
		/// <param name="maxMemberNumber">群人数上限</param>
		/// <returns></returns>
		public int GetGroupMemberNumber (long robot, long group, out int maxMemberNumber) {
			return WaitSystemGet<int, int> (new JObject () {
				{ "Type", nameof (GetGroupMemberNumber) },
				{ "Robot", robot },
				{ "Group", group }
			}, out maxMemberNumber);
		}

		/// <summary>
		/// 取群是否支持匿名
		/// </summary>
		/// <returns></returns>
		public bool IsGroupAnonymousEnabled (long robot, long group) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (IsGroupAnonymousEnabled) },
				{ "Robot", robot },
				{ "Group", group }
			});
		}

		/// <summary>
		/// 取讨论组成员QQ
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="discuss">讨论组号</param>
		/// <returns></returns>
		public long[] GetDiscussMembers (long robot, long discuss) {
			return WaitSystemGet<long[]> (new JObject () {
				{ "Type", nameof (GetDiscussMembers) },
				{ "Robot", robot },
				{ "Discuss", discuss }
			});
		}

		/// <summary>
		/// 取讨论组号
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <returns></returns>
		public long[] GetDiscusss (long robot) {
			return WaitSystemGet<long[]> (new JObject () {
				{ "Type", nameof (GetDiscusss) },
				{ "Robot", robot }
			});
		}

		/// <summary>
		/// 通过讨论组号获取加群连接
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="discuss">需执行的讨论组ID</param>
		/// <returns></returns>
		public string GetDiscussURL (long robot, long discuss) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetDiscussURL) },
				{ "Robot", robot },
				{ "Discuss", discuss }
			});
		}

		/// <summary>
		/// 取讨论组名称
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="discuss">需执行的讨论组ID</param>
		/// <returns></returns>
		public string GetDiscussName (long robot, long discuss) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetDiscussName) },
				{ "Robot", robot },
				{ "Discuss", discuss }
			});
		}

		/// <summary>
		/// 根据图片码取得图片下载连接
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">指定的群号或讨论组号,临时会话和好友不填</param>
		/// <param name="code">例如[pic={xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx}.jpg]</param>
		/// <returns></returns>
		public string GetImageURL (long robot, long group, string code) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetImageURL) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Code", code }
			});
		}

		/// <summary>
		/// 取语音下载地址
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="code">例如[Voi={xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxx}.amr]</param>
		/// <returns></returns>
		public string GetVoiceURL (long robot, string code) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetVoiceURL) },
				{ "Robot", robot },
				{ "Code", code }
			});
		}

		/// <summary>
		/// 取用户名
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">欲取得的QQ的号码</param>
		/// <returns></returns>
		public string GetName (long robot, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetName) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 把群图片的GUID转换成好友可用的GUID
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="picture">例：[pic={441EE15D-1D80-2AA4-1D03-96A7C303CD93}.jpg]</param>
		/// <returns></returns>
		public string GroupPictureToFriendPicture (long robot, string picture) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GroupPictureToFriendPicture) },
				{ "Robot", robot },
				{ "Picture", picture }
			});
		}

		/// <summary>
		/// 删除好友 成功返回真 失败返回假
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">好友QQ号</param>
		/// <returns></returns>
		public bool RemoveFriend (long robot, long qq) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (RemoveFriend) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 把好友删除为单项，或从对方列表删除自己 Pro可用
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">欲操作的目标</param>
		/// <param name="operatorType">1为在对方的列表删除我(双向) 2为在我的列表删除对方(单项) 默认为2</param>
		/// <returns></returns>
		public bool RemoveFriendByOneWay (long robot, long qq, long operatorType) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (RemoveFriendByOneWay) },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "OperatorType", operatorType }
			});
		}

		/// <summary>
		/// 删除框架帐号列表内指定QQ，不可在执行登录过程中删除QQ否则有几率引起错误 QQMini Pro才可用
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		public void RemoveRobot (long robot) {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (RemoveRobot) },
				{ "Robot", robot }
			});
		}

		/// <summary>
		/// 上传群或讨论组图片（通过读入字节集方式），可使用网页链接或本地读入，成功返回该图片GUID，失败返回空
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">要上传的群号或讨论组号</param>
		/// <param name="base64">图片base64文本</param>
		/// <returns></returns>
		public string UploadGroupChatImage (long robot, long group, string base64) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (UploadGroupChatImage) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Data", base64 }
			});
		}
		/// <summary>
		/// 上传群或讨论组图片（通过读入字节集方式），可使用网页链接或本地读入，成功返回该图片GUID，失败返回空
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">要上传的群号或讨论组号</param>
		/// <param name="data">图片字节集数据</param>
		/// <returns></returns>
		public string UploadGroupChatImage (long robot, long group, byte[] data) {
			return UploadGroupChatImage (robot, group, Convert.ToBase64String (data));
		}

		/// <summary>
		/// 成功返回真 失败返回假 Pro可用
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">要发送的群号</param>
		/// <param name="filePath">文件路径</param>
		/// <returns></returns>
		public bool UploadGroupFile (long robot, long group, string filePath) {
			throw new NotImplementedException ();
		}

		/// <summary>
		/// 上传QQ语音，成功返回语音GUID 失败返回空
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">要上传的群号</param>
		/// <param name="base64">语音字节集数据（AMR Silk编码）</param>
		/// <returns></returns>
		public string UploadGroupChatVoice (long robot, long group, string base64) {
			throw new NotImplementedException ();
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (UploadGroupChatVoice) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Data", base64 }
			});
		}
		/// <summary>
		/// 上传QQ语音，成功返回语音GUID 失败返回空
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">要上传的群号</param>
		/// <param name="data">语音字节集数据（AMR Silk编码）</param>
		/// <returns></returns>
		public string UploadGroupChatVoice (long robot, long group, byte[] data) {
			throw new NotImplementedException ();
			return UploadGroupChatVoice (robot, group, Convert.ToBase64String (data));
		}

		/// <summary>
		/// 上传好友或临时会话图片（通过读入字节集方式），可使用网页链接或本地读入，成功返回该图片GUID，失败返回空
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">要上传的QQ号</param>
		/// <param name="base64">图片base64文本</param>
		/// <returns></returns>
		public string UploadPrivateChatImage (long robot, long qq, string base64) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (UploadPrivateChatImage) },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "Data",base64 }
			});
		}
		/// <summary>
		/// 上传好友或临时会话图片（通过读入字节集方式），可使用网页链接或本地读入，成功返回该图片GUID，失败返回空
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">要上传的QQ号</param>
		/// <param name="data">图片字节集数据</param>
		/// <returns></returns>
		public string UploadPrivateChatImage (long robot, long qq, byte[] data) {
			return UploadPrivateChatImage (robot, qq, Convert.ToBase64String (data));
		}

		/// <summary>
		/// 主动加好友 成功返回真 失败返回假 当对象设置需要正确回答问题或不允许任何人添加时无条件失败 QQMini Pro才可用
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">要添加的好友QQ号</param>
		/// <param name="message">加好友提交的理由</param>
		/// <returns></returns>
		public string RequestAddFriend (long robot, long qq, string message) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (RequestAddFriend) },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "Message", message }
			});
		}

		/// <summary>
		/// 申请加群.为了避免广告、群发行为。出现验证码时将会直接失败不处理 QQMini Pro才可用
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">欲申请加入的群号</param>
		/// <param name="message">附加理由，可留空（需回答正确问题时，请填写问题答案）</param>
		/// <returns></returns>
		public string RequestAddGroup (long robot, long group, string message) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (RequestAddGroup) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Message", message }
			});
		}

		/// <summary>
		/// 获取机器人QQ是否被屏蔽消息发送状态（真屏蔽 假未屏蔽）
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <returns></returns>
		public bool IsMaskSendMessage (long robot) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (IsMaskSendMessage) },
				{ "Robot", robot }
			});
		}

		/// <summary>
		/// 取得插件自身启用状态，启用真 禁用假
		/// </summary>
		/// <returns></returns>
		public bool IsEnable () {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (IsEnable) }
			});
		}

		/// <summary>
		/// 是否QQ好友（双向） 好友返回真 非好友或获取失败返回假
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">需获取对象QQ</param>
		/// <returns></returns>
		public bool IsFriend (long robot, long qq) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (IsFriend) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 查询群是否支持群私聊功能 Pro可用
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">要查询的群号</param>
		/// <returns></returns>
		public bool IsAllowGroupTempMessage (long robot, long group) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (IsAllowGroupTempMessage) },
				{ "Robot", robot },
				{ "Group", group }
			});
		}

		/// <summary>
		/// 查询对方是否允许网页咨询发起的临时会话消息（非讨论组和群临时）
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">需获取对象QQ</param>
		/// <returns></returns>
		public bool IsAllowWebpageTempMessage (long robot, long qq) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (IsAllowWebpageTempMessage) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 向框架帐号列表添加一个Q.当该Q已存在时则覆盖密码 QQMini Pro才可用
		/// </summary>
		/// <param name="robot">机器人QQ </param>
		/// <param name="password">机器人密码 </param>
		/// <param name="autoLogin">运行框架时是否自动登录该Q.若添加后需要登录该Q则需要通过API登录账号操作</param>
		/// <returns></returns>
		public bool AddRobot (long robot, string password, bool autoLogin) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (AddRobot) },
				{ "Robot", robot },
				{ "Password", password },
				{ "AutoLogin", autoLogin }
			});
		}

		/// <summary>
		/// 退群
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">欲退出的群号</param>
		public void RemoveGroup (long robot, long group) {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (RemoveGroup) },
				{ "Robot", robot },
				{ "Group", group }
			});
		}

		/// <summary>
		/// 退出讨论组
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="discuss">需退出的讨论组ID</param>
		public void RemoveDiscuss (long robot, long discuss) {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (RemoveDiscuss) },
				{ "Robot", robot },
				{ "Discuss", discuss }
			});
		}

		/// <summary>
		/// 令指定QQ下线，应确保QQ号码已在列表中且在线
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		public void LogoutRobot (long robot) {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (LogoutRobot) },
				{ "Robot", robot }
			});
		}

		/// <summary>
		/// 邀请对象加入讨论组 成功返回空 失败返回理由
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="discuss">需执行的讨论组ID</param>
		/// <param name="qq">被邀请对象QQ</param>
		/// <returns></returns>
		public bool InviteFriendJoinDiscuss (long robot, long discuss, long qq) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (InviteFriendJoinDiscuss) },
				{ "Robot", robot },
				{ "Discuss", discuss },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 管理员邀请对象入群，频率过快会失败
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">被邀请加入的群号</param>
		/// <param name="qq">被邀请人QQ号码</param>
		public void InviteFriendJoinGroupByAdministrator (long robot, long group, long qq) {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (InviteFriendJoinGroupByAdministrator) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 非管理员邀请对象入群，频率过快会失败
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">被邀请加入的群号</param>
		/// <param name="qq">被邀请人QQ号码</param>
		public void InviteFriendJoinGroupNonAdministrator (long robot, long group, long qq) {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (InviteFriendJoinGroupNonAdministrator) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 上传封面（通过读入字节集方式）成功真 失败假
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="base64">图片base64文本</param>
		/// <returns></returns>
		public bool SetCover (long robot, string base64) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetCover) },
				{ "Robot", robot },
				{ "Data", base64 }
			});
		}
		/// <summary>
		/// 上传封面（通过读入字节集方式）成功真 失败假
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="data">图片数据</param>
		/// <returns></returns>
		public bool SetCover (long robot, byte[] data) {
			return SetCover (robot, Convert.ToBase64String (data));
		}

		/// <summary>
		/// 设置个人签名
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="personalSignature">签名</param>
		public void SetPersonalSignature (long robot, string personalSignature) {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (SetPersonalSignature) },
				{ "Robot", robot },
				{ "PersonalSignature", personalSignature }
			});
		}

		/// <summary>
		/// 修改好友备注姓名
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">需获取对象好友QQ</param>
		/// <param name="notes">需要修改的备注姓名</param>
		public void SetFriendNotes (long robot, long qq, string notes) {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (SetFriendNotes) },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "Notes", notes }
			});
		}

		/// <summary>
		/// 将好友拉入黑名单或解除
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">要拉黑的好友QQ号</param>
		/// <param name="enable">真拉黑,假取消拉黑</param>
		public void SetFriendBlacklist (long robot, long qq, bool enable) {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (SetFriendBlacklist) },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "Enable", enable }
			});
		}

		/// <summary>
		/// 设置本机器人好友验证方式，可重复调用 Pro可用
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="VerificationMethod">验证方式</param>
		/// <param name="question">需要回答的问题,不需要可空</param>
		/// <param name="answer">设置的问题答案,不需要可空</param>
		/// <returns></returns>
		public bool SetFriendAuthenticationMethod (long robot, ChatRobotFriendAddMethod VerificationMethod, string question, string answer) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetFriendAuthenticationMethod) },
				{ "Robot", robot },
				{ "VerificationMethod", (int)VerificationMethod },
				{ "Question", question },
				{ "Answer", answer }
			});
		}

		/// <summary>
		/// 设置机器人性别
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="gender">1为男 2为女</param>
		public void SetRobotGender (long robot, long gender) {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (SetRobotGender) },
				{ "Robot", robot },
				{ "Gender", gender }
			});
		}

		/// <summary>
		/// 设置机器人在线状态+附加信息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="state">在线状态</param>
		/// <param name="information">最大255字节</param>
		public void SetRobotState (long robot, long state, string information) {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (SetRobotState) },
				{ "Robot", robot },
				{ "State", state },
				{ "Information", information }
			});
		}

		/// <summary>
		/// 设置机器人昵称
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="name">需要设置的昵称</param>
		public void SetRobotName (long robot, string name) {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (SetRobotName) },
				{ "Robot", robot },
				{ "Name", name },
			});
		}

		/// <summary>
		/// 禁言/解禁匿名成员
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">禁言对象所在群.</param>
		/// <param name="anonymousInformation">收到匿名消息时返回的Flag 例：[AnonyMsg,Name=小遮拦,Fkey=AB4A9698AA5C3D17A173D0F7C89B8675758534099F1477206EDF559D3E3A1DD964EC71B34F9B6B77]</param>
		/// <param name="duration">单位:秒 最大为1个月. 为零解除对象禁言</param>
		/// <returns></returns>
		public bool SetAnonymousMemberBanSpeak (long robot, long group, string anonymousInformation, long duration) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetAnonymousMemberBanSpeak) },
				{ "Robot", robot },
				{ "Group", group },
				{ "AnonymousInformation", anonymousInformation },
				{ "Duration", duration }
			});
		}

		/// <summary>
		/// 禁言群
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">要禁言的群号</param>
		/// <param name="enable">为真开启禁言. 为假解除</param>
		/// <returns></returns>
		public bool SetGroupBanSpeak (long robot, long group, bool enable) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetGroupBanSpeak) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Enable", enable }
			});
		}

		/// <summary>
		/// 禁言群成员
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">要禁言的群号</param>
		/// <param name="qq">要禁言的QQ号</param>
		/// <param name="seconds">单位:秒 最大为1个月. 为零解除</param>
		/// <returns></returns>
		public bool SetGroupMemberBanSpeak (long robot, long group, long qq, long seconds) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetGroupMemberBanSpeak) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq },
				{ "Seconds", seconds }
			});
		}

		/// <summary>
		/// 修改对象群名片 成功返回真 失败返回假
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">对象所处群号</param>
		/// <param name="qq">被修改名片人QQ</param>
		/// <param name="businessCard">需要修改的名片</param>
		/// <returns></returns>
		public bool SetGroupMemberBusinessCard (long robot, long group, long qq, string businessCard) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetGroupMemberBusinessCard) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq },
				{ "BusinessCard", businessCard }
			});
		}

		/// <summary>
		/// 将对象移除群
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">被执行群号</param>
		/// <param name="qq">被执行对象</param>
		/// <param name="noLongerAccept">真为不再接收，假为接收，默认为假</param>
		public void KickGroupMember (long robot, long group, long qq, bool noLongerAccept) {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (KickGroupMember) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq },
				{ "NoLongerAccept", noLongerAccept }
			});
		}

		/// <summary>
		/// 设置或取消群管理员 成功返回真 失败返回假
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">指定群号</param>
		/// <param name="qq">群员QQ号</param>
		/// <param name="enable">真 为设置管理 假为取消管理</param>
		/// <returns></returns>
		public bool SetGroupAdministrator (long robot, long group, long qq, bool enable) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetGroupAdministrator) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq },
				{ "Enable", enable }
			});
		}

		/// <summary>
		/// 开关群匿名消息发送功能 成功返回真 失败返回假
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">需开关群匿名功能群号</param>
		/// <param name="enable">真开 假关</param>
		/// <returns></returns>
		public bool SetGroupAnonymousEnable (long robot, long group, bool enable) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetGroupAnonymousEnable) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Enable", enable }
			});
		}

		/// <summary>
		/// 屏蔽或接收某群消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">指定群号</param>
		/// <param name="enable">真 为屏蔽接收 假为接收并提醒</param>
		public void SetMaskGroupMessage (long robot, long group, bool enable) {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (SetMaskGroupMessage) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Enable", enable }
			});
		}

		/// <summary>
		/// 在框架记录页输出一行信息
		/// </summary>
		/// <param name="content">输出的内容</param>
		public void Log (string content) {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (Log) },
				{ "Content", content }
			});
		}

		/// <summary>
		/// 置正在输入状态（发送消息后会打断状态）
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="qq">置正在输入状态接收对象QQ号</param>
		public void SetInputting (long robot, long qq) {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (SetInputting) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 将对象移除讨论组
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="discuss">需执行的讨论组ID</param>
		/// <param name="qq">被执行对象</param>
		public void KickDiscussMember (long robot, long discuss, long qq) {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (KickDiscussMember) },
				{ "Robot", robot },
				{ "Discuss", discuss },
				{ "QQ", qq }
			});
		}

		/// <summary>
		/// 修改讨论组名称
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="discuss">需执行的讨论组ID</param>
		/// <param name="name">需修改的名称</param>
		public void SetDiscussName (long robot, long discuss, string name) {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (SetDiscussName) },
				{ "Robot", robot },
				{ "Discuss", discuss },
				{ "Name", name }
			});
		}

		/// <summary>
		/// 上传头像（通过读入字节集方式）成功真 失败假
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="base64">图片base64文本</param>
		public void SetAvatar (long robot, string base64) {
			SocketClientBeginSend (new JObject () {
				{ "Type", nameof (SetAvatar) },
				{ "Robot", robot },
				{ "Data", base64 }
			});
		}
		/// <summary>
		/// 上传头像（通过读入字节集方式）成功真 失败假
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="data">图片数据</param>
		public void SetAvatar (long robot, byte[] data) {
			SetAvatar (robot, Convert.ToBase64String (data));
		}

		/// <summary>
		/// 断开与机器人框架RPC插件的连接
		/// </summary>
		public void Disconnect () {
			SocketClient.Disconnect ();
			WaitSystem.Dispose ();
		}

		void SocketClient_OnReceived (byte[] data) {
			string text = Encoding.UTF8.GetString (data);
			try {
				OnReceived?.Invoke (text);
				JObject jObject = JObject.Parse (text);
				string type = jObject.Value<string> ("Type");
				switch (type) {
					default:
						throw new NotImplementedException ($"未知的消息类型：{type}");
					case "Protocol": {
						string targetProtocolVersion = jObject.Value<string> ("Version");
						if (targetProtocolVersion != ProtocolVersision) {
							throw new Exception ($"SDK协议版本：{ProtocolVersision} 与机器人框架插件的协议版本：{targetProtocolVersion} 不符");
						}
						break;
					}
					case "Return":
						WaitSystem.Set (jObject.Value<long> ("ID"), jObject.Value<string> ("Result"));
						break;
					case "Message":
						OnReceivedMessage?.Invoke (new ChatRobotMessage (
							this,
							EnumParse<ChatRobotMessageType> (jObject.Value<string> ("SubType")),
							jObject.Value<long> ("Robot"),
							jObject.Value<long> ("Group"),
							jObject.Value<long> ("QQ"),
							jObject.Value<string> ("Message"),
							jObject.Value<long> ("MessageNumber"),
							jObject.Value<long> ("MessageID")
						));
						break;
					case "GroupAddRequest":
						OnReceivedGroupAddRequest?.Invoke (
							EnumParse<ChatRobotGroupAddRequestType> (jObject.Value<string> ("SubType")),
							jObject.Value<long> ("Robot"),
							jObject.Value<long> ("Group"),
							jObject.Value<long> ("QQ"),
							jObject.Value<long> ("InviterQQ"),
							jObject.Value<long> ("Sign"),
							jObject.Value<string> ("Message")
						);
						break;
					case "FriendAddResponse":
						OnReceivedFriendAddResponse?.Invoke (
							jObject.Value<bool> ("Agree"),
							jObject.Value<long> ("Robot"),
							jObject.Value<long> ("QQ"),
							jObject.Value<string> ("Message")
						);
						break;
					case "FriendAddRequest":
						OnReceivedFriendAddRequest?.Invoke (
							jObject.Value<long> ("Robot"),
							jObject.Value<long> ("QQ"),
							jObject.Value<string> ("Message")
						);
						break;
					case "GroupMessageRevoke":
						OnGroupMessageRevoked?.Invoke (
							jObject.Value<long> ("Robot"),
							jObject.Value<long> ("Group"),
							jObject.Value<long> ("QQ"),
							jObject.Value<long> ("MessageNumber"),
							jObject.Value<long> ("MessageID")
						);
						break;
					case "GroupAnonymousSwitch":
						OnGroupAnonymousSwitched?.Invoke (
							jObject.Value<bool> ("Enable"),
							jObject.Value<long> ("Robot"),
							jObject.Value<long> ("Group"),
							jObject.Value<long> ("QQ")
						);
						break;
					case "GroupNameChange":
						OnGroupNameChanged?.Invoke (
							jObject.Value<long> ("Robot"),
							jObject.Value<long> ("Group"),
							jObject.Value<string> ("Name")
						);
						break;
					case "GroupBanSpeak":
						OnGroupBannedSpeak?.Invoke (
							jObject.Value<bool> ("Enable"),
							jObject.Value<long> ("Robot"),
							jObject.Value<long> ("Group"),
							jObject.Value<long> ("QQ")
						);
						break;
					case "GroupAdminChange":
						OnGroupAdministratorChanged?.Invoke (
							jObject.Value<bool> ("Enable"),
							jObject.Value<long> ("Robot"),
							jObject.Value<long> ("Group"),
							jObject.Value<long> ("QQ")
						);
						break;
					case "GroupMemberBusinessCardChange":
						OnGroupMemberBusinessCardChanged?.Invoke (
							jObject.Value<long> ("Robot"),
							jObject.Value<long> ("Group"),
							jObject.Value<long> ("QQ"),
							jObject.Value<string> ("BusinessCard")
						);
						break;
					case "GroupMemberLeave":
						OnGroupMemberLeft?.Invoke (
							jObject.Value<bool> ("Kick"),
							jObject.Value<long> ("Robot"),
							jObject.Value<long> ("Group"),
							jObject.Value<long> ("QQ"),
							jObject.Value<long> ("OperatorQQ")
						);
						break;
					case "GroupMemberBanSpeak":
						OnGroupMemberBannedSpeak?.Invoke (
							jObject.Value<bool> ("Enable"),
							jObject.Value<long> ("Robot"),
							jObject.Value<long> ("Group"),
							jObject.Value<long> ("QQ"),
							jObject.Value<long> ("OperatorQQ"),
							jObject.Value<int> ("Seconds")
						);
						break;
					case "GroupMemberJoin":
						OnGroupMemberJoined?.Invoke (
							EnumParse<ChatRobotGroupMemberJoinType> (jObject.Value<string> ("SubType")),
							jObject.Value<long> ("Robot"),
							jObject.Value<long> ("Group"),
							jObject.Value<long> ("QQ"),
							jObject.Value<long> ("OperatorQQ")
						);
						break;
					case "GroupDisband":
						OnGroupDisbanded?.Invoke (
							jObject.Value<long> ("Robot"),
							jObject.Value<long> ("Group"),
							jObject.Value<long> ("QQ")
						);
						break;
					case "FriendStateChange":
						OnFriendStateChanged?.Invoke (
							jObject.Value<long> ("Robot"),
							jObject.Value<long> ("QQ"),
							jObject.Value<string> ("State")
						);
						break;
					case "WasRemoveByFriend":
						OnWasRemovedByFriend?.Invoke (
							jObject.Value<long> ("Robot"),
							jObject.Value<long> ("QQ")
						);
						break;
				}
			} catch (Exception exception) {
				Console.WriteLine ($"{DateTime.Now} 处理消息：{text} 时出现异常：{exception}");
			}
		}

		void SendGroupMessage (string type, long robot, long group, string message, bool isAnonymous) {
			SocketClientBeginSend (new JObject () {
				{ "Type", type },
				{ "Robot", robot },
				{ "Group", group },
				{ "Message", message },
				{ "IsAnonymous", isAnonymous }
			});
		}

		void SendFriendMessage (string type, long robot, long qq, string message) {
			SocketClientBeginSend (new JObject () {
				{ "Type", type },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "Message", message }
			});
		}

		void SendGroupTempMessage (string type, long robot, long group, long qq, string message) {
			SocketClientBeginSend (new JObject () {
				{ "Type", type },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq },
				{ "Message", message }
			});
		}

		T EnumParse<T> (string value) {
			return (T)Enum.Parse (typeof (T), value);
		}

		long WaitSystemSend (JObject jObject) {
			long id = WaitSystem.GetID ();
			jObject["ID"] = id;
			SocketClientBeginSend (jObject);
			return id;
		}

		T WaitSystemConvert<T> (string result) {
			if (typeof (T).IsEnum) {
				if (int.TryParse (result, out int enumIndex)) {
					return (T)Enum.ToObject (typeof (T), enumIndex);
				}
				return (T)Enum.Parse (typeof (T), result);
			}
			if (typeof (T) == typeof (bool)) {
				if (result == "真") {
					result = "true";
				} else if (result == "假") {
					result = "false";
				}
				return (T)Convert.ChangeType (result, typeof (T));
			}
			if (typeof (T).IsValueType || typeof (T) == typeof (string)) {
				return (T)Convert.ChangeType (result, typeof (T));
			}
			return JsonConvert.DeserializeObject<T> (result);
		}

		T WaitSystemGet<T> (JObject jObject) {
			return WaitSystemConvert<T> (WaitSystem.Get (WaitSystemSend (jObject)));
		}
		Result WaitSystemGet<Result, T> (JObject jObject, out T arg) {
			JArray result = JArray.Parse (WaitSystem.Get (WaitSystemSend (jObject)));
			arg = WaitSystemConvert<T> (result[1].Value<string> ());
			return WaitSystemConvert<Result> (result[0].Value<string> ());
		}
		Result WaitSystemGet<Result, T1, T2> (JObject jObject, out T1 arg1, out T2 arg2) {
			JArray result = JArray.Parse (WaitSystem.Get (WaitSystemSend (jObject)));
			arg1 = WaitSystemConvert<T1> (result[1].Value<string> ());
			arg2 = WaitSystemConvert<T2> (result[2].Value<string> ());
			return WaitSystemConvert<Result> (result[0].Value<string> ());
		}

		void SocketClientBeginSend (JObject jObject) {
			SocketClient.SendAsync (Encoding.GetBytes (jObject.ToString (Formatting.None)));
		}

	}

}