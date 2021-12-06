using Eruru.ChatRobotAPI;
using Eruru.Html;
using Eruru.Http;
using Eruru.TextCommand;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace ConsoleApp1 {

	class Program {

		enum Permissionlevel {

			Friend,
			Group

		}

		static void Main (string[] args) {
			Console.Title = string.Empty;
			AppDomain.CurrentDomain.UnhandledException += (sender, e) => {
				Console.WriteLine (e);
			};
			long master = long.Parse (File.ReadAllText ("D:/Master.txt"));
			long[] whiteList = Array.ConvertAll (File.ReadAllLines ("D:/White List.txt"), value => long.Parse (value));
			TextCommandSystem<ChatRobotMessage> textCommandSystem = new TextCommandSystem<ChatRobotMessage> ();
			textCommandSystem.Register<Program> ();
			textCommandSystem.MatchParameterType = false;
			List<ChatRobotMessage> messages = new List<ChatRobotMessage> ();
			ChatRobotAPI.OnReceived = text => Console.WriteLine ($"收到消息：{text}");
			ChatRobotAPI.OnSent = text => Console.WriteLine ($"发送消息：{text}");
			ChatRobotAPI.OnFriendStateChanged = (robot, qq, state) => {
				ChatRobotAPI.SendFriendMessage (robot, master, $"{FormatQQName (robot, qq)}改变了在线状态：{state}");
			};
			ChatRobotAPI.OnGroupAdministratorChanged = (enable, robot, group, qq) => {
				if (!whiteList.Contains (group)) {
					return;
				}
				ChatRobotAPI.SendGroupMessage (robot, group, $"{ChatRobotCode.At (qq)}{(enable ? "成为" : "失去")}管理");
			};
			ChatRobotAPI.OnGroupAnonymousSwitched = (enable, robot, group, qq) => {
				if (!whiteList.Contains (group)) {
					return;
				}
				ChatRobotAPI.SendGroupMessage (robot, group, $"{ChatRobotCode.At (qq)}{(enable ? "开启了" : "关闭了")}群匿名功能");
			};
			ChatRobotAPI.OnGroupBannedSpeak = (enable, robot, group, qq) => {
				ChatRobotAPI.SendFriendMessage (robot, master, $"群{FormatGroupName (robot, group)}管理{FormatQQName (robot, qq)}{(enable ? "开启了" : "关闭了")}全体禁言");
			};
			ChatRobotAPI.OnGroupDisbanded = (robot, group, qq) => {
				ChatRobotAPI.SendFriendMessage (robot, master, $"群{FormatGroupName (robot, group)}被{FormatQQName (robot, qq)}解散");
			};
			ChatRobotAPI.OnGroupMemberBannedSpeak = (enable, robot, group, qq, operatorQQ, seconds) => {
				if (!whiteList.Contains (group)) {
					return;
				}
				ChatRobotAPI.SendGroupMessage (robot, group, $"{ChatRobotCode.At (qq)}被{ChatRobotCode.At (operatorQQ)}禁言{seconds}秒");
			};
			ChatRobotAPI.OnGroupMemberBusinessCardChanged = (robot, group, qq, businessCard) => {
				if (!whiteList.Contains (group)) {
					return;
				}
				ChatRobotAPI.SendGroupMessage (robot, group, $"{ChatRobotCode.At (qq)}修改了名片");
			};
			ChatRobotAPI.OnGroupMemberJoined = (type, robot, group, qq, operatorQQ) => {
				switch (type) {
					case ChatRobotGroupMemberJoinType.Approve:
						if (!whiteList.Contains (group)) {
							return;
						}
						ChatRobotAPI.SendGroupMessage (robot, group, $"{ChatRobotCode.At (operatorQQ)}同意{ChatRobotCode.At (qq)}加入此群");
						break;
					case ChatRobotGroupMemberJoinType.IJoin:
						ChatRobotAPI.SendFriendMessage (robot, master, $"机器人加入了群{FormatGroupName (robot, group)}");
						break;
					case ChatRobotGroupMemberJoinType.Invite:
						if (!whiteList.Contains (group)) {
							return;
						}
						ChatRobotAPI.SendGroupMessage (robot, group, $"{ChatRobotCode.At (operatorQQ)}邀请{ChatRobotCode.At (qq)}加入此群");
						break;
					default:
						throw new NotImplementedException (type.ToString ());
				}
			};
			ChatRobotAPI.OnGroupMemberLeaved = (kick, robot, group, qq, operatorQQ) => {
				if (qq == robot) {
					ChatRobotAPI.SendFriendMessage (robot, master, $"机器人{(kick ? $"被{operatorQQ}踢出" : "离开")}群{FormatGroupName (robot, group)}");
					return;
				}
				if (!whiteList.Contains (group)) {
					return;
				}
				ChatRobotAPI.SendGroupMessage (robot, group, $"{FormatQQName (robot, qq)}{(kick ? $"被{ChatRobotCode.At (operatorQQ)}踢出" : "离开")}群");
			};
			ChatRobotAPI.OnGroupMessageRevoked = (robot, group, qq, messageNumber, messageID) => {
				if (!whiteList.Contains (group)) {
					return;
				}
				DateTime expiry = DateTime.Now.AddMinutes (-3);
				for (int i = 0; i < messages.Count; i++) {
					if (messages[i].Number == messageNumber && messages[i].ID == messageID) {
						ChatRobotAPI.SendGroupMessage (robot, group, $"{ChatRobotCode.At (qq)}撤回了{ChatRobotCode.At (messages[i].QQ)}于{messages[i].DateTime}的消息：{messages[i].Text}");
						messages.RemoveAt (i);
						break;
					}
					if (messages[i].DateTime <= expiry) {
						messages.RemoveAt (i--);
					}
				}
			};
			ChatRobotAPI.OnGroupNameChanged = (robot, group, qq, name) => {
				if (!whiteList.Contains (group)) {
					return;
				}
				ChatRobotAPI.SendGroupMessage (robot, group, $"群名变为{name}");
			};
			ChatRobotAPI.OnReceivedFriendAddRequest = (robot, qq, message) => {
				ChatRobotAPI.SendFriendMessage (robot, master, $"收到{FormatQQName (robot, qq)}的加好友请求：{message}");
			};
			ChatRobotAPI.OnReceivedFriendAddResponse = (agree, robot, qq, message) => {
				ChatRobotAPI.SendFriendMessage (robot, master, $"{FormatQQName (robot, qq)}{(agree ? "同意" : "拒绝")}加好友，消息：{message}");
			};
			ChatRobotAPI.OnReceivedGroupAddRequest = (type, robot, group, qq, inviterQQ, sign, message) => {
				switch (type) {
					case ChatRobotGroupAddRequestType.InviteMe:
						ChatRobotAPI.SendFriendMessage (robot, master, $"{inviterQQ}邀请加入群{FormatGroupName (robot, group)}，消息：{message}，Sign：{sign}");
						break;
					case ChatRobotGroupAddRequestType.MemberInvite:
						if (!whiteList.Contains (group)) {
							return;
						}
						ChatRobotAPI.SendGroupMessage (robot, group, $"{ChatRobotCode.At (inviterQQ)}邀请{ChatRobotCode.At (qq)}加入此群，消息：{message}，Sign：{sign}");
						break;
					case ChatRobotGroupAddRequestType.Request:
						if (!whiteList.Contains (group)) {
							return;
						}
						ChatRobotAPI.SendGroupMessage (robot, group, $"{FormatQQName (robot, qq)}请求加入此群，消息：{message}，Sign：{sign}");
						break;
					default:
						throw new NotImplementedException (type.ToString ());
				}
			};
			ChatRobotAPI.OnReceivedMessage = message => {
				switch (message.Type) {
					case ChatRobotMessageType.Group:
					case ChatRobotMessageType.Discuss:
						if (!whiteList.Contains (message.Group)) {
							return;
						}
						break;
					case ChatRobotMessageType.Friend:
					case ChatRobotMessageType.GroupTemp:
					case ChatRobotMessageType.DiscussTemp:
						if (message.QQ != master) {
							return;
						}
						break;
				}
				messages.Add (message);
				try {
					textCommandSystem.Execute (message.Text, message, (int)Permissionlevel.Group);
				} catch (Exception exception) {
					message.Reply (exception.Message);
				}
				if (message.Text.StartsWith ("复读")) {
					message.Reply (message.Text.Substring ("复读".Length));
					return;
				}
				if (message.Text.StartsWith ("[FlashPic={")) {
					message.Reply (message.Text.Replace ("FlashPic", "pic"));
					return;
				}
				if (message.Type == ChatRobotMessageType.Friend || message.Text.Contains ($"[@{message.Robot}]")) {
					HttpRequestInformation httpRequestInformation = new HttpRequestInformation () {
						Url = "http://api.qingyunke.com/api.php",
						QueryStringParameters = {
							{ "key", "free" },
							{ "appid", 0 },
							{ "msg", message.Text.Replace ($"[@{message.Robot}]", "") }
						}
					};
					string response = JObject.Parse (new Http ().Request (httpRequestInformation)).Value<string> ("content").Replace ("{br}", "\n");
					message.Reply ($"{ChatRobotCode.At (message.QQ)} {response}");
					return;
				}
				switch (message.Text) {
					case "一言":
						message.Reply (new Http ().Request ("http://api.sdtro.cn/API/yiy/yiy.php"));
						break;
					case "来首歌":
						message.ReplyJson (new Http ().Request ("http://api.qfyu.top/API/wysj.php").Substring ("json:".Length));
						break;
				}
			};
			ChatRobotAPI.OnWasRemoveByFriend = (robot, qq) => {
				ChatRobotAPI.SendFriendMessage (robot, master, $"被{FormatQQName (robot, qq)}删除好友");
			};
			ChatRobotAPI.OnDisconnected = () => {
				Console.WriteLine ("连接已断开");
				Thread.Sleep (1000);
				ChatRobotAPI.Connect ("localhost", 19730, "root", "root");
			};
			Console.WriteLine ("连接服务器中");
			ChatRobotAPI.Connect ("localhost", 19730, "root", "root");
			Console.WriteLine ("连接成功");
			Console.ReadLine ();
			ChatRobotAPI.Disconnect ();
		}

		[TextCommand ("今日效率")]
		static void TodayCombat (ChatRobotMessage message, params string[] names) {
			Combat (message, string.Join (" ", names), DateTime.Now.Month, DateTime.Now.Day);
		}

		[TextCommand ("昨日效率")]
		static void YesterdayCombat (ChatRobotMessage message, params string[] names) {
			DateTime dateTime = DateTime.Now.AddDays (-1);
			Combat (message, string.Join (" ", names), dateTime.Month, dateTime.Day);
		}

		[TextCommand ("前日效率")]
		static void DayBeforeYesterdayCombat (ChatRobotMessage message, params string[] names) {
			DateTime dateTime = DateTime.Now.AddDays (-2);
			Combat (message, string.Join (" ", names), dateTime.Month, dateTime.Day);
		}

		[TextCommand ("效率")]
		static void Combat (ChatRobotMessage message, int month, int day, params string[] names) {
			Combat (message, string.Join (" ", names), month, day);
		}

		[TextCommand ("效率")]
		static void Combat (ChatRobotMessage message, int count, params string[] names) {
			if (count > 100) {
				message.Reply ("结果数限制为100");
				count = 100;
			}
			int recordCount = 0;
			Combat (message, string.Join (" ", names), $"前{count}个", null, item => {
				if (ModeFilter (item)) {
					if (recordCount++ < count) {
						return 1;
					}
					return -1;
				}
				return 0;
			});
		}

		static void Combat (ChatRobotMessage message, string name, string label, Func<int, bool> pageFilter, Func<BattleRecord, int> filter) {
			message.Reply ($"开始查询{name}{label}的效率，请稍后");
			if (!TryGetBattleRecords (name, pageFilter, filter, out List<BattleRecord> battleRecords, out string error) ||
				!TrySummaryBattleRecords (battleRecords, out int battleCount, out int victoryCount, out int evenCount, out int failCount, out float averageCombat, out error)) {
				message.Reply ($"查询{name}{label}的效率失败，{error}");
				return;
			}
			message.Reply ($"{name}{label}的战斗次数为{battleCount} 胜率{(float)victoryCount / battleCount:P2}\n胜利{victoryCount} 平局{evenCount} 失败{failCount}\n平均效率为{averageCombat:F2}");
		}
		static void Combat (ChatRobotMessage message, string name, int month, int day) {
			Combat (message, name, $"{month}月{day}日", null, item => {
				if (ModeFilter (item)) {
					return DayFilter (item, month, day);
				}
				return 0;
			});
		}

		[TextCommand ("TEA加密")]
		static void TEAEncrypted (ChatRobotMessage message, string content, string key) {
			message.Reply (ChatRobotAPI.TEAEncryption (content, key));
		}

		[TextCommand ("TEA解密")]
		static void TEADecrypt (ChatRobotMessage message, string content, string key) {
			message.Reply (ChatRobotAPI.TEADecryption (content, key));
		}

		[TextCommand ("查询群礼物")]
		static void QueryGroupGift (ChatRobotMessage message) {
			message.Reply (JsonConvert.SerializeObject (ChatRobotAPI.QueryGroupGiftInformations (message.Robot), Formatting.Indented));
		}

		[TextCommand ("撤回群消息")]
		static void RevokeGroupMessage (ChatRobotMessage message) {
			message.Reply (ChatRobotAPI.RevokeGroupMessage (message.Robot, message.Group, message.Number, message.ID));
		}

		[TextCommand ("抽取群礼物")]
		static void DrawGroupGift (ChatRobotMessage message) {
			message.Reply (ChatRobotAPI.DrawGroupGift (message.Robot, message.Group));
		}

		[TextCommand ("处理好友添加请求")]
		static void HandleFriendAddRequest (ChatRobotMessage message, long qq, long treatmentMethod, string information) {
			ChatRobotAPI.HandleFriendAddRequest (message.Robot, qq, treatmentMethod, information);
		}

		[TextCommand ("处理群添加请求")]
		static void HandleGroupAddRequest (ChatRobotMessage message, long requestType, long qq, long group, long tag, long treatmentMethod, string information) {
			ChatRobotAPI.HandleGroupAddRequest (message.Robot, requestType, qq, group, tag, treatmentMethod, information);
		}

		[TextCommand ("创建讨论组")]
		static void CreateDiscuss (ChatRobotMessage message) {
			message.Reply (ChatRobotAPI.CreateDiscuss (message.Robot, message.QQ));
		}

		[TextCommand ("登录账号")]
		static void LoginAccount (ChatRobotMessage message, long robot) {
			ChatRobotAPI.LoginRobot (robot);
		}

		[TextCommand ("点赞")]
		static void Like (ChatRobotMessage message, long qq) {
			message.Reply (ChatRobotAPI.Like (message.Robot, qq));
		}

		[TextCommand ("发布群公告")]
		static void PublishGroupAnnouncement (ChatRobotMessage message, string title, string content) {
			message.Reply (ChatRobotAPI.PublishGroupAnnouncement (message.Robot, message.Group, title, content));
		}

		[TextCommand ("发布群作业")]
		static void PublishGroupJob (ChatRobotMessage message, string name, string title, string content) {
			message.Reply (ChatRobotAPI.PublishGroupJob (message.Robot, message.Group, name, title, content));
		}

		[TextCommand ("发送好友Json消息")]
		static void SendFriendJsonMessage (ChatRobotMessage message, long qq, string content) {
			ChatRobotAPI.SendFriendJsonMessage (message.Robot, qq, content);
		}

		[TextCommand ("发送好友Xml消息")]
		static void SendFriendXmlMessage (ChatRobotMessage message, long qq, string content) {
			ChatRobotAPI.SendFriendXmlMessage (message.Robot, qq, content);
		}

		[TextCommand ("发送好友窗口抖动")]
		static void SendFriendWindowJitter (ChatRobotMessage message, long qq) {
			message.Reply (ChatRobotAPI.SendFriendWindowJitter (message.Robot, qq));
		}

		[TextCommand ("发送好友消息")]
		static void SendFriendMessage (ChatRobotMessage message, long qq, string content) {
			ChatRobotAPI.SendFriendMessage (message.Robot, qq, content);
		}

		[TextCommand ("发送好友验证回复Json消息")]
		static void SendFriendVerificationReplyJsonMessage (ChatRobotMessage message, long qq, string content) {
			ChatRobotAPI.SendFriendVerificationReplyJsonMessage (message.Robot, qq, content);
		}

		[TextCommand ("发送好友验证回复Xml消息")]
		static void SendFriendVerificationReplyXmlMessage (ChatRobotMessage message, long qq, string content) {
			ChatRobotAPI.SendFriendVerificationReplyXmlMessage (message.Robot, qq, content);
		}

		[TextCommand ("发送好友验证回复消息")]
		static void SendFriendVerificationReplyMessage (ChatRobotMessage message, long qq, string content) {
			ChatRobotAPI.SendFriendVerificationReplyMessage (message.Robot, qq, content);
		}

		[TextCommand ("发送好友语音")]
		static void SendFriendVoice (ChatRobotMessage message, long qq, string data) {
			throw new NotImplementedException (nameof (SendFriendVoice));
		}

		[TextCommand ("发送群Json消息")]
		static void SendFriendMessage (ChatRobotMessage message, string content) {
			ChatRobotAPI.SendGroupJsonMessage (message.Robot, message.Group, content);
		}

		[TextCommand ("发送群Xml消息")]
		static void SendGroupXmlMessage (ChatRobotMessage message, string content) {
			ChatRobotAPI.SendGroupXmlMessage (message.Robot, message.Group, content);
		}

		[TextCommand ("发送群礼物")]
		static void SendGroupGift (ChatRobotMessage message, long qq, long gift) {
			message.Reply (ChatRobotAPI.SendGroupGift (message.Robot, message.Group, qq, gift));
		}

		[TextCommand ("发送群临时Json消息")]
		static void SendGroupTempJsonMessage (ChatRobotMessage message, long qq, string content) {
			ChatRobotAPI.SendGroupTempJsonMessage (message.Robot, message.Group, qq, content);
		}

		[TextCommand ("发送群临时Xml消息")]
		static void SendGroupTempXmlMessage (ChatRobotMessage message, long qq, string content) {
			ChatRobotAPI.SendGroupTempXmlMessage (message.Robot, message.Group, qq, content);
		}

		[TextCommand ("发送群临时消息")]
		static void SendGroupTempMessage (ChatRobotMessage message, long qq, string content) {
			ChatRobotAPI.SendGroupTempMessage (message.Robot, message.Group, qq, content);
		}

		[TextCommand ("发送群签到", PermissionLevel = (int)Permissionlevel.Group)]
		static void GroupSignIn (ChatRobotMessage message, string place, string content) {
			message.Reply (ChatRobotAPI.SendGroupSignIn (message.Robot, message.Group, place, content));
		}

		[TextCommand ("发送群消息")]
		static void SendGroupMessage (ChatRobotMessage message, string content) {
			ChatRobotAPI.SendGroupMessage (message.Robot, message.Group, content);
		}

		[TextCommand ("取成员群聊等级")]
		static void GetMemberGroupChatLevel (ChatRobotMessage message, long qq) {
			message.Reply (ChatRobotAPI.GetMemberGroupChatLevel (message.Robot, message.Group, qq));
		}

		[TextCommand ("取当前时间戳")]
		static void GetMemberGroupChatLevel (ChatRobotMessage message) {
			message.Reply (ChatRobotAPI.GetCurrentTimeStamp ());
		}

		[TextCommand ("尝试取等级")]
		static void TryGetLevel (ChatRobotMessage message) {
			if (ChatRobotAPI.TryGetLevel (message.Robot, out ChatRobotLevelInformation levelInformation)) {
				message.Reply (JsonConvert.SerializeObject (levelInformation));
				return;
			}
			message.Reply ("失败");
		}

		[TextCommand ("取好友QQ达人天数")]
		static void GetFriendQQMasterDays (ChatRobotMessage message, long qq) {
			message.Reply (ChatRobotAPI.GetFriendQQMasterDays (message.Robot, qq));
		}

		[TextCommand ("取好友Q龄")]
		static void GetFriendQAge (ChatRobotMessage message, long qq) {
			message.Reply (ChatRobotAPI.GetFriendQAge (message.Robot, qq));
		}

		[TextCommand ("取好友备注")]
		static void GetFriendNotes (ChatRobotMessage message, long qq) {
			message.Reply (ChatRobotAPI.GetFriendNotes (message.Robot, qq));
		}

		[TextCommand ("取好友等级")]
		static void GetFriendLevel (ChatRobotMessage message, long qq) {
			message.Reply (ChatRobotAPI.GetFriendLevel (message.Robot, qq));
		}

		[TextCommand ("取好友个人说明")]
		static void GetFriendPersonalDescription (ChatRobotMessage message, long qq) {
			message.Reply (ChatRobotAPI.GetFriendPersonalDescription (message.Robot, qq));
		}

		[TextCommand ("取好友个性签名")]
		static void GetFriendPersonalSignature (ChatRobotMessage message, long qq) {
			message.Reply (ChatRobotAPI.GetFriendPersonalSignature (message.Robot, qq));
		}

		[TextCommand ("取好友年龄")]
		static void GetFriendAge (ChatRobotMessage message, long qq) {
			message.Reply (ChatRobotAPI.GetFriendAge (message.Robot, qq));
		}

		[TextCommand ("取好友是否在线")]
		static void IsFriendOnline (ChatRobotMessage message, long qq) {
			message.Reply (ChatRobotAPI.IsFriendOnline (message.Robot, qq));
		}

		[TextCommand ("尝试取好友信息")]
		static void TryGetFriendInformation (ChatRobotMessage message, long qq) {
			if (ChatRobotAPI.TryGetFriendInformation (message.Robot, qq, out ChatRobotFriendInformation friendInformation)) {
				message.Reply (JsonConvert.SerializeObject (friendInformation));
				return;
			}
			message.Reply ("失败");
		}

		[TextCommand ("取好友性别")]
		static void GetFriendGender (ChatRobotMessage message, long qq) {
			message.Reply (ChatRobotAPI.GetFriendGender (message.Robot, qq));
		}

		[TextCommand ("取好友邮箱")]
		static void GetFriendEmail (ChatRobotMessage message, long qq) {
			message.Reply (ChatRobotAPI.GetFriendEmail (message.Robot, qq));
		}

		[TextCommand ("取好友在线状态")]
		static void GetFriendOnlineState (ChatRobotMessage message, long qq) {
			message.Reply (ChatRobotAPI.GetFriendOnlineState (message.Robot, qq));
		}

		[TextCommand ("取好友账号列表")]
		static void GetFriendQQs (ChatRobotMessage message) {
			message.Reply (JsonConvert.SerializeObject (ChatRobotAPI.GetFriends (message.Robot)));
		}

		[TextCommand ("尝试取机器人状态")]
		static void TryGetRobotState (ChatRobotMessage message) {
			if (ChatRobotAPI.TryGetRobotStateInformation (message.Robot, out ChatRobotStateInformation robotStateInformation)) {
				message.Reply (JsonConvert.SerializeObject (robotStateInformation));
				return;
			}
			message.Reply ("失败");
		}

		[TextCommand ("取群成员列表")]
		static void GetGroupMemberList (ChatRobotMessage message) {
			message.Reply (JsonConvert.SerializeObject (ChatRobotAPI.GetGroupMemberListInformation (message.Robot, message.Group, out ChatRobotGroupMemberInformation[] groupMemberInformations)));
			message.Reply (JsonConvert.SerializeObject (groupMemberInformations));
		}

		[TextCommand ("取群公告")]
		static void GetGroupAnnouncement (ChatRobotMessage message) {
			message.Reply (ChatRobotAPI.GetGroupAnnouncement (message.Robot, message.Group));
		}

		[TextCommand ("取昵称")]
		static void GetName (ChatRobotMessage message, long qq) {
			message.Reply (ChatRobotAPI.GetName (message.Robot, qq));
		}

		[TextCommand ("取群名")]
		static void GetGroupName (ChatRobotMessage message) {
			message.Reply (ChatRobotAPI.GetGroupName (message.Robot, message.Group));
		}

		[TextCommand ("上传群聊图片")]
		static void UploadGroupChatImage (ChatRobotMessage message, string base64) {
			message.Reply (ChatRobotAPI.UploadGroupChatImage (message.Robot, message.Group, base64));
		}

		static string FormatGroupName (long robot, long group) {
			return $"{ChatRobotAPI.GetGroupName (robot, group)}（{group}）";
		}

		static string FormatQQName (long robot, long qq) {
			return $"{ChatRobotAPI.GetName (robot, qq)}（{qq}）";
		}

		static bool TryGetBattleRecords (string name, int page, out List<BattleRecord> battleRecords, out string message) {
			battleRecords = null;
			message = null;
			try {
				string response = new Http ().Request (new HttpRequestInformation () {
					Url = "http://wotbox.ouj.com/wotbox/index.php",
					Type = HttpRequestType.Get,
					QueryStringParameters = {
					{ "r", "default/battleLog" },
					{ "pn", HttpApi.UrlEncode (name) },
					{ "p", page }
				}
				});
				if (response.Contains ("您的访问过于频繁，请稍后再试")) {
					message = response;
					return false;
				}
				if (response.Contains ("NOT FOUND USER")) {
					message = "没有找到您搜索的玩家！";
					return false;
				}
				HtmlDocument htmlDocument = HtmlDocument.Parse (response);
				battleRecords = new List<BattleRecord> ();
				foreach (HtmlElement htmlElement in htmlDocument.QuerySelectorAll (".battle-log .J_nav li")) {
					string arenaID = htmlElement.GetAttribute ("arena-id");
					string[] datas = htmlElement.GetElementByClassName ("game").InnerHtml.Split (' ');
					string[] dates = datas[1].Split ('-');
					int month = int.Parse (dates[0]);
					int day = int.Parse (dates[1]);
					BattleResult battleResult;
					string state = htmlElement.GetElementByClassName ("state").TextContent;
					switch (state) {
						case "胜利":
							battleResult = BattleResult.Victory;
							break;
						case "平局":
							battleResult = BattleResult.Even;
							break;
						case "失败":
							battleResult = BattleResult.Fail;
							break;
						default:
							throw new NotImplementedException (state);
					}
					battleRecords.Add (new BattleRecord () {
						Name = name,
						ArenaID = arenaID,
						Result = battleResult,
						Mode = datas[0],
						DateTime = new DateTime (DateTime.Now.Year, month, day)
					});
				}
				if (battleRecords.Count == 0) {
					message = "没有战斗数据";
					return false;
				}
				return true;
			} catch (Exception exception) {
				message = exception.ToString ();
				return false;
			}
		}
		static bool TryGetBattleRecords (string name, Func<int, bool> pageFilter, Func<BattleRecord, int> filter, out List<BattleRecord> battleRecords, out string message) {
			battleRecords = new List<BattleRecord> ();
			int page = 1;
			bool needBreak = false;
			message = null;
			BattleRecord lastFirstBattleRecord = null;
			while (true) {
				if (!pageFilter?.Invoke (page) ?? false) {
					break;
				}
				if (!TryGetBattleRecords (name, page, out List<BattleRecord> pageBattleRecords, out message)) {
					return false;
				}
				if (pageBattleRecords?.Count > 0) {
					if (lastFirstBattleRecord?.ArenaID == pageBattleRecords[0].ArenaID) {
						break;
					}
					lastFirstBattleRecord = pageBattleRecords[0];
				}
				foreach (BattleRecord battleRecord in pageBattleRecords) {
					int code = filter?.Invoke (battleRecord) ?? 1;
					switch (code) {
						case 1:
							battleRecords.Add (battleRecord);
							break;
						case 0:
							break;
						case -1:
							needBreak = true;
							break;
						default:
							throw new NotImplementedException (code.ToString ());
					}
					if (needBreak) {
						break;
					}
				}
				if (needBreak) {
					break;
				}
				page++;
			}
			return true;
		}

		static bool TrySummaryBattleRecords (List<BattleRecord> battleRecords, out int battleCount, out int victoryCount, out int evenCount, out int failCount, out float averageCombat, out string message) {
			battleCount = 0;
			victoryCount = 0;
			evenCount = 0;
			failCount = 0;
			averageCombat = 0;
			message = null;
			if (battleRecords.Count == 0) {
				message = "没有战斗数据";
				return false;
			}
			battleCount = battleRecords.Count;
			foreach (BattleRecord item in battleRecords) {
				item.PullCombat ();
				switch (item.Result) {
					case BattleResult.Victory:
						victoryCount++;
						break;
					case BattleResult.Even:
						evenCount++;
						break;
					case BattleResult.Fail:
						failCount++;
						break;
					default:
						throw new NotImplementedException (item.Result.ToString ());
				}
				averageCombat += item.Combat;
			}
			averageCombat /= battleRecords.Count;
			return true;
		}

		static int DayFilter (BattleRecord battleRecord, int month, int day) {
			if (battleRecord.DateTime.Month > month || battleRecord.DateTime.Day > day) {
				return 0;
			}
			if (battleRecord.DateTime.Month < month || battleRecord.DateTime.Day < day) {
				return -1;
			}
			return 1;
		}

		static bool ModeFilter (BattleRecord battleRecord) {
			return battleRecord.Mode == "标准赛";
		}

		class BattleRecord {

			public string Name { get; set; }
			public string ArenaID { get; set; }
			public BattleResult Result { get; set; }
			public string Mode { get; set; }
			public DateTime DateTime { get; set; }
			public float Combat { get; set; }

			public void PullCombat () {
				Eruru.Json.JsonObject jsonObject = Eruru.Json.JsonObject.Parse (new Http ().Request (new HttpRequestInformation () {
					Url = "http://wotapp.ouj.com",
					QueryStringParameters = {
							{ "r", "wotboxapi/battledetail" },
							{ "pn", Name },
							{ "arena_id", ArenaID }
						}
				}).Trim ('(', ')'));
				float combat = -1;
				foreach (Eruru.Json.JsonValue player in jsonObject.Select ("result.team_a")) {
					if (player["name"] == Name) {
						combat = player["combat"];
						break;
					}
				}
				Combat = combat;
			}

		}

		enum BattleResult {

			Victory,
			Fail,
			Even

		}

	}

}