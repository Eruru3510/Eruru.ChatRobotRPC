using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace Eruru.ChatRobotRPC {

	/// <summary>
	/// 聊天机器人
	/// </summary>
	public class ChatRobot {

		/// <summary>
		/// 收到消息（底层协议消息）
		/// </summary>
		public Action<string> OnReceived { get; set; }
		/// <summary>
		/// 发送消息（底层协议消息）
		/// </summary>
		public Action<string> OnSent { get; set; }
		/// <summary>
		/// 收到消息（好友或群等，统一入口，可用ChatRobot.UseAsyncReceive决定是否异步处理消息）
		/// </summary>
		public Action<ChatRobotMessage> OnReceivedMessage { get; set; }
		/// <summary>
		/// 收到群添加请求（使用ChatRobot.HandleGroupAddRequest处理）
		/// </summary>
		public ChatRobotGroupAddRequestEventHandler OnReceivedGroupAddRequest { get; set; }
		/// <summary>
		/// 收到好友添加响应
		/// </summary>
		public ChatRobotFriendAddResponseEventHandler OnReceivedFriendAddResponse { get; set; }
		/// <summary>
		/// 收到好友添加请求（使用ChatRobot.HandleFriendAddRequest处理）
		/// </summary>
		public ChatRobotFriendAddRequestEventHandler OnReceivedFriendAddRequest { get; set; }
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
		public ChatRobotGroupMemberLeavedEventHandler OnGroupMemberLeaved { get; set; }
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
		public ChatRobotOnWasRemoveByFriendEventHandler OnWasRemoveByFriend { get; set; }
		/// <summary>
		/// 与机器人框架RPC插件断开了连接
		/// </summary>
		public ChatRobotAction OnDisconnected { get; set; }
		/// <summary>
		/// 心跳包发送间隔（秒）
		/// </summary>
		public long HeartbeatPacketSendIntervalBySeconds {

			get => Client.HeartbeatPacketSendIntervalBySeconds;

			set => Client.HeartbeatPacketSendIntervalBySeconds = value;

		}
		/// <summary>
		/// 协议版本
		/// </summary>
		public string ProtocolVersision { get; } = "1.0.0.0";
		/// <summary>
		/// 是否使用异步接收并处理消息
		/// </summary>
		public bool UseAsyncReceive { get; set; } = true;

		readonly WaitSystem WaitSystem = new WaitSystem ();
		readonly Client Client;
		readonly Action<byte[]> ReceivedAsync;


		/// <summary>
		/// 默认构造函数
		/// </summary>
		public ChatRobot () {
			ReceivedAsync = Received;
			Client = new Client () {
				OnReceived = bytes => {
					if (UseAsyncReceive) {
						ReceivedAsync.BeginInvoke (bytes, asyncResult => ReceivedAsync.EndInvoke (asyncResult), null);
						return;
					}
					Received (bytes);
				},
				OnSent = Sent,
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
			Client.Connect (ip, port);
			ClientBeginSend (new JObject () {
				{ "Type", "Login" },
				{ "Account", account },
				{ "Password", password }
			});
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
		/// <param name="qq">申请入群 被邀请人 （当请求类型为某人被邀请时这里为邀请人QQ）</param>
		/// <param name="group">收到请求群号</param>
		/// <param name="sign">需要处理请求的标记</param>
		/// <param name="treatmentMethod">处理方式</param>
		/// <param name="information">拒绝入群 附加信息</param>
		public void HandleGroupAddRequest (long robot, ChatRobotGroupAddRequestType requestType, long qq, long group, long sign,
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
			ClientBeginSend (new JObject () {
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
		public void SendGroupJsonMessage (long robot, long group, string message) {
			SendGroupMessage ("SendGroupJsonMessage", robot, group, message);
		}

		/// <summary>
		/// 发送群XML消息
		/// </summary>
		/// <param name="robot">机器人QQ</param>
		/// <param name="group">要发送消息的群号</param>
		/// <param name="message">信息内容</param>
		public void SendGroupXmlMessage (long robot, long group, string message) {
			SendGroupMessage ("SendGroupXmlMessage", robot, group, message);
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
		public void SendGroupMessage (long robot, long group, string message) {
			SendGroupMessage ("SendGroupMessage", robot, group, message);
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
			SendGroupMessage ("SendDiscussJsonMessage", robot, group, message);
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
			SendGroupMessage ("SendDiscussXmlMessage", robot, group, message);
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
			SendGroupMessage ("SendDiscussMessage", robot, group, message);
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
			ClientBeginSend (new JObject () {
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
		public long GetGroupMemberNumber (long robot, long group, out long maxMemberNumber) {
			return WaitSystemGet<long, long> (new JObject () {
				{ "Type", nameof (GetGroupMemberNumber) },
				{ "Robot", robot },
				{ "Group", group }
			}, out maxMemberNumber);
		}

		public long[] GetDiscussMembers (long robot, long discuss) {
			return WaitSystemGet<long[]> (new JObject () {
				{ "Type", nameof (GetDiscussMembers) },
				{ "Robot", robot },
				{ "Discuss", discuss }
			});
		}

		public long[] GetDiscusss (long robot) {
			return WaitSystemGet<long[]> (new JObject () {
				{ "Type", nameof (GetDiscusss) },
				{ "Robot", robot }
			});
		}

		public string GetDiscussURL (long robot, long discuss) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetDiscussURL) },
				{ "Robot", robot },
				{ "Discuss", discuss }
			});
		}

		public string GetDiscussName (long robot, long discuss) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetDiscussName) },
				{ "Robot", robot },
				{ "Discuss", discuss }
			});
		}

		public string GetImageURL (long robot, long group, string code) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetImageURL) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Code", code }
			});
		}

		public string GetVoiceURL (long robot, string code) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetVoiceURL) },
				{ "Robot", robot },
				{ "Code", code }
			});
		}

		public string GetName (long robot, long qq) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (GetName) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public bool RemoveFriend (long robot, long qq) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (RemoveFriend) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public bool RemoveFriendByOneWay (long robot, long qq, long operatorType) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (RemoveFriendByOneWay) },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "OperatorType", operatorType }
			});
		}

		public void RemoveRobot (long robot) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (RemoveRobot) },
				{ "Robot", robot }
			});
		}

		public string UploadGroupChatImage (long robot, long group, string base64) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (UploadGroupChatImage) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Data", base64 }
			});
		}
		public string UploadGroupChatImage (long robot, long group, byte[] data) {
			return UploadGroupChatImage (robot, group, Convert.ToBase64String (data));
		}

		public string UploadGroupChatVoice (long robot, long group, string base64) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (UploadGroupChatVoice) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Data", base64 }
			});
		}
		public string UploadGroupChatVoice (long robot, long group, byte[] data) {
			return UploadGroupChatVoice (robot, group, Convert.ToBase64String (data));
		}

		public string UploadPrivateChatImage (long robot, long qq, string base64) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (UploadPrivateChatImage) },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "Data",base64 }
			});
		}
		public string UploadPrivateChatImage (long robot, long qq, byte[] data) {
			return UploadPrivateChatImage (robot, qq, Convert.ToBase64String (data));
		}

		public string RequestAddFriend (long robot, long qq, string message) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (RequestAddFriend) },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "Message", message }
			});
		}

		public string RequestAddGroup (long robot, long group, string message) {
			return WaitSystemGet<string> (new JObject () {
				{ "Type", nameof (RequestAddGroup) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Message", message }
			});
		}

		public bool IsMaskSendMessage (long robot) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (IsMaskSendMessage) },
				{ "Robot", robot }
			});
		}

		public bool IsEnable () {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (IsEnable) }
			});
		}

		public bool IsFriend (long robot, long qq) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (IsFriend) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public bool IsAllowGroupTempMessage (long robot, long group) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (IsAllowGroupTempMessage) },
				{ "Robot", robot },
				{ "Group", group }
			});
		}

		public bool IsAllowWebpageTempMessage (long robot, long qq) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (IsAllowWebpageTempMessage) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public bool AddRobot (long robot, string password, bool autoLogin) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (AddRobot) },
				{ "Robot", robot },
				{ "Password", password },
				{ "AutoLogin", autoLogin }
			});
		}

		public void RemoveGroup (long robot, long group) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (RemoveGroup) },
				{ "Robot", robot },
				{ "Group", group }
			});
		}

		public void RemoveDiscuss (long robot, long discuss) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (RemoveDiscuss) },
				{ "Robot", robot },
				{ "Discuss", discuss }
			});
		}

		public void LogoutRobot (long robot) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (LogoutRobot) },
				{ "Robot", robot }
			});
		}

		public bool InviteFriendJoinDiscuss (long robot, long discuss, long qq) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (InviteFriendJoinDiscuss) },
				{ "Robot", robot },
				{ "Discuss", discuss },
				{ "QQ", qq }
			});
		}

		public void InviteFriendJoinGroupByAdministrator (long robot, long group, long qq) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (InviteFriendJoinGroupByAdministrator) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq }
			});
		}

		public void InviteFriendJoinGroupNonAdministrator (long robot, long group, long qq) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (InviteFriendJoinGroupNonAdministrator) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq }
			});
		}

		public bool SetCover (long robot, string base64) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetCover) },
				{ "Robot", robot },
				{ "Data", base64 }
			});
		}
		public bool SetCover (long robot, byte[] data) {
			return SetCover (robot, Convert.ToBase64String (data));
		}

		public void SetPersonalSignature (long robot, string personalSignature) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (SetPersonalSignature) },
				{ "Robot", robot },
				{ "PersonalSignature", personalSignature }
			});
		}

		public void SetFriendNotes (long robot, long qq, string notes) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (SetFriendNotes) },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "Notes", notes }
			});
		}

		public void SetFriendBlacklist (long robot, long qq, bool enable) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (SetFriendBlacklist) },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "Enable", enable }
			});
		}

		public bool SetFriendAuthenticationMethod (long robot, long VerificationMethod, string question, string answer) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetFriendAuthenticationMethod) },
				{ "Robot", robot },
				{ "VerificationMethod", VerificationMethod },
				{ "Question", question },
				{ "Answer", answer }
			});
		}

		public void SetRobotGender (long robot, long gender) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (SetRobotGender) },
				{ "Robot", robot },
				{ "Gender", gender }
			});
		}

		public void SetRobotState (long robot, long state, string information) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (SetRobotState) },
				{ "Robot", robot },
				{ "State", state },
				{ "Information", information }
			});
		}

		public void SetRobotName (long robot, string name) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (SetRobotName) },
				{ "Robot", robot },
				{ "Name", name },
			});
		}

		public bool SetGroupBanSpeak (long robot, long group, bool enable) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetGroupBanSpeak) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Enable", enable }
			});
		}

		public bool SetGroupMemberBanSpeak (long robot, long group, long qq, long seconds) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetGroupMemberBanSpeak) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq },
				{ "Seconds", seconds }
			});
		}

		public bool SetGroupMemberBusinessCard (long robot, long group, long qq, string businessCard) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetGroupMemberBusinessCard) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq },
				{ "BusinessCard", businessCard }
			});
		}

		public void KickGroupMember (long robot, long group, long qq, bool noLongerAccept) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (KickGroupMember) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq },
				{ "NoLongerAccept", noLongerAccept }
			});
		}

		public bool SetGroupAdministrator (long robot, long group, long qq, bool enable) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetGroupAdministrator) },
				{ "Robot", robot },
				{ "Group", group },
				{ "QQ", qq },
				{ "Enable", enable }
			});
		}

		public bool SetGroupAnonymousEnable (long robot, long group, bool enable) {
			return WaitSystemGet<bool> (new JObject () {
				{ "Type", nameof (SetGroupAnonymousEnable) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Enable", enable }
			});
		}

		public void SetMaskGroupMessage (long robot, long group, bool enable) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (SetMaskGroupMessage) },
				{ "Robot", robot },
				{ "Group", group },
				{ "Enable", enable }
			});
		}

		public void Log (string content) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (Log) },
				{ "Content", content }
			});
		}

		public void SetInputting (long robot, long qq) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (SetInputting) },
				{ "Robot", robot },
				{ "QQ", qq }
			});
		}

		public void KickDiscussMember (long robot, long discuss, long qq) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (KickDiscussMember) },
				{ "Robot", robot },
				{ "Discuss", discuss },
				{ "QQ", qq }
			});
		}

		public void SetDiscussName (long robot, long discuss, string name) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (SetDiscussName) },
				{ "Robot", robot },
				{ "Discuss", discuss },
				{ "Name", name }
			});
		}

		public void SetAvatar (long robot, string base64) {
			ClientBeginSend (new JObject () {
				{ "Type", nameof (SetAvatar) },
				{ "Robot", robot },
				{ "Data", base64 }
			});
		}
		public void SetAvatar (long robot, byte[] data) {
			SetAvatar (robot, Convert.ToBase64String (data));
		}

		/// <summary>
		/// 断开与机器人框架RPC插件的连接
		/// </summary>
		public void Disconnect () {
			Client.Disconnect ();
			WaitSystem.Dispose ();
		}

		void Received (byte[] data) {
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
					OnGroupMemberLeaved?.Invoke (
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
						jObject.Value<long> ("Seconds")
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
					OnWasRemoveByFriend?.Invoke (
						jObject.Value<long> ("Robot"),
						jObject.Value<long> ("QQ")
					);
					break;
				default:
					Console.WriteLine ($"未知的消息类型：{jObject.Value<string> ("Type")}");
					break;
			}
		}

		void Sent (string text) {
			OnSent?.Invoke (text);
		}

		void SendGroupMessage (string type, long robot, long group, string message) {
			ClientBeginSend (new JObject () {
				{ "Type", type },
				{ "Robot", robot },
				{ "Group", group },
				{ "Message", message }
			});
		}

		void SendFriendMessage (string type, long robot, long qq, string message) {
			ClientBeginSend (new JObject () {
				{ "Type", type },
				{ "Robot", robot },
				{ "QQ", qq },
				{ "Message", message }
			});
		}

		void SendGroupTempMessage (string type, long robot, long group, long qq, string message) {
			ClientBeginSend (new JObject () {
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
			Client.BeginSend (jObject.ToString (Formatting.None));
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

		void ClientBeginSend (JObject jObject) {
			Client.BeginSend (jObject.ToString (Formatting.None));
		}

	}

}