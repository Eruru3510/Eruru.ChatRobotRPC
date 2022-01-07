using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Eruru.ChatRobotRPC {

	/// <summary>
	/// 无参委托
	/// </summary>
	public delegate void ChatRobotAction ();
	/// <summary>
	/// 群添加请求
	/// </summary>
	/// <param name="type">请求类型</param>
	/// <param name="robot">收到此事件机器人的QQ</param>
	/// <param name="group">申请要加入的群号</param>
	/// <param name="qq">申请或被邀请加入群组者的QQ号.</param>
	/// <param name="inviterQQ">邀请者的QQ号。</param>
	/// <param name="sign">唯一表示本次请求,用于处理请求</param>
	/// <param name="message">附加理由</param>
	public delegate void ChatRobotGroupAddRequestedEventHandler (ChatRobotGroupAddRequestType type, long robot, long group, long qq, long inviterQQ,
		long sign, string message
	);
	/// <summary>
	/// 好友添加响应
	/// </summary>
	/// <param name="agree">是否同意</param>
	/// <param name="robot">收到此事件机器人的QQ</param>
	/// <param name="qq">同意或拒绝添加响应QQ的QQ号</param>
	/// <param name="message">拒绝好友时的附加消息/目前暂不支持</param>
	public delegate void ChatRobotFriendAddRespondedEventHandler (bool agree, long robot, long qq, string message);
	/// <summary>
	/// 好友添加请求
	/// </summary>
	/// <param name="robot">收到此事件机器人的QQ</param>
	/// <param name="qq">请求添加响应QQ好友的QQ号</param>
	/// <param name="message">请求添加好友的附加消息</param>
	public delegate void ChatRobotFriendAddRequestedEventHandler (long robot, long qq, string message);
	/// <summary>
	/// 群消息撤回
	/// </summary>
	/// <param name="robot">收到此事件机器人的QQ</param>
	/// <param name="group">撤回消息的来源群号</param>
	/// <param name="qq">撤回消息的QQ号</param>
	/// <param name="messageNumber">被撤回消息的序号</param>
	/// <param name="messageID">被撤回消息的标识</param>
	public delegate void ChatRobotGroupMessageRevokedEventHandler (long robot, long group, long qq, long messageNumber, long messageID);
	/// <summary>
	/// 群匿名开关
	/// </summary>
	/// <param name="enable">群匿名是否开启</param>
	/// <param name="robot">收到此事件机器人的QQ</param>
	/// <param name="group">发生事件的群号</param>
	/// <param name="qq">更改群匿名状态的管理员QQ号</param>
	public delegate void ChatRobotGroupAnonymousSwitchedEventHandler (bool enable, long robot, long group, long qq);
	/// <summary>
	/// 群禁言
	/// </summary>
	/// <param name="enable">全群禁言是否开启</param>
	/// <param name="robot">收到此事件机器人的QQ</param>
	/// <param name="group">开启或关闭群体禁言的群号</param>
	/// <param name="qq">开启或关闭群体禁言的QQ号</param>
	public delegate void ChatRobotGroupBannedSpeakEventHandler (bool enable, long robot, long group, long qq);
	/// <summary>
	/// 群名改变
	/// </summary>
	/// <param name="robot">收到此事件机器人的QQ</param>
	/// <param name="group">改名的群号</param>
	/// <param name="name">改完的群名</param>
	public delegate void ChatRobotGroupNameChangedEventHandler (long robot, long group, string name);
	/// <summary>
	/// 群管理员改变
	/// </summary>
	/// <param name="enable">是否成为管理</param>
	/// <param name="robot">收到此事件机器人的QQ</param>
	/// <param name="group">管理员改变的群号</param>
	/// <param name="qq">成为或被取消管理员的QQ号</param>
	public delegate void ChatRobotGroupAdministratorChangedEventHandler (bool enable, long robot, long group, long qq);
	/// <summary>
	/// 群成员名片改变
	/// </summary>
	/// <param name="robot">收到此事件机器人的QQ</param>
	/// <param name="group">群成员名片改的群号</param>
	/// <param name="qq">改变名片的人的QQ号</param>
	/// <param name="businessCard">修改完的群名片</param>
	public delegate void ChatRobotGroupMemberBusinessCardChangedEventHandler (long robot, long group, long qq, string businessCard);
	/// <summary>
	/// 群成员离开
	/// </summary>
	/// <param name="kick">是否是被踢</param>
	/// <param name="robot">收到此事件机器人的QQ</param>
	/// <param name="group">离开的群号</param>
	/// <param name="qq">退出或被移出群组的QQ号</param>
	/// <param name="operatorQQ">移除QQ的操作者QQ（如果是自己退群，则为-1）</param>
	public delegate void ChatRobotGroupMemberLeftEventHandler (bool kick, long robot, long group, long qq, long operatorQQ);
	/// <summary>
	/// 群成员禁言
	/// </summary>
	/// <param name="enable">是否被禁言</param>
	/// <param name="robot">收到此事件机器人的QQ</param>
	/// <param name="group">发生事件的群号</param>
	/// <param name="qq">被禁止发言的QQ号</param>
	/// <param name="operatorQQ">设置禁言的QQ号 (管理员或群主)</param>
	/// <param name="seconds">禁言时长, 单位: 秒, 范围: 1秒-30天</param>
	public delegate void ChatRobotGroupMemberBannedSpeakEventHandler (bool enable, long robot, long group, long qq, long operatorQQ, int seconds);
	/// <summary>
	/// 群成员加入
	/// </summary>
	/// <param name="type">加入类型</param>
	/// <param name="robot">收到此事件机器人的QQ</param>
	/// <param name="group">加入的群号</param>
	/// <param name="qq">被批准或邀请加入群组的QQ号</param>
	/// <param name="operatorQQ">邀请或批准请求的管理QQ</param>
	public delegate void ChatRobotGroupMemberJoinedEventHandler (ChatRobotGroupMemberJoinType type, long robot, long group, long qq, long operatorQQ);
	/// <summary>
	/// 群被解散
	/// </summary>
	/// <param name="robot">收到此事件机器人的QQ</param>
	/// <param name="group">被解散的群号</param>
	/// <param name="qq">解散群组的群主QQ</param>
	public delegate void ChatRobotGroupDisbandedEventHandler (long robot, long group, long qq);
	/// <summary>
	/// 好友状态改变
	/// </summary>
	/// <param name="robot">收到此事件机器人的QQ</param>
	/// <param name="qq">改变状态的人的QQ号</param>
	/// <param name="state">在线状态</param>
	public delegate void ChatRobotFriendStateChangedEventHandler (long robot, long qq, string state);
	/// <summary>
	/// 被好友删除
	/// </summary>
	/// <param name="robot">收到此事件机器人的QQ</param>
	/// <param name="qq">将响应QQ从好友列表中删除的QQ号</param>
	public delegate void ChatRobotWasRemovedByFriendEventHandler (long robot, long qq);
	/// <summary>
	/// 其他事件
	/// </summary>
	/// <param name="robot">收到此事件机器人的QQ</param>
	/// <param name="eventType">事件类型</param>
	/// <param name="subType">事件子类型</param>
	/// <param name="source">事件来源</param>
	/// <param name="active">主动对象</param>
	/// <param name="passive">被动对象</param>
	/// <param name="message">消息内容</param>
	/// <param name="messageNumber">消息序号</param>
	/// <param name="messageID">消息ID</param>
	public delegate void ChatRobotOtherEventEventHandler (long robot, ChatRobotEventType eventType, int subType, long source, long active, long passive, string message,
		long messageNumber, long messageID
	);

	/// <summary>
	/// 聊天机器人API
	/// </summary>
	public class ChatRobotAPI {

		/// <summary>
		/// 是否为语音消息
		/// </summary>
		/// <param name="message">消息文本</param>
		/// <param name="guid">提取出来的语音GUID</param>
		/// <param name="identifyResult">提取出来的语音识别结果</param>
		/// <returns></returns>
		public static bool IsVoiceMessage (string message, out string guid, out string identifyResult) {
			guid = null;
			identifyResult = null;
			Match match = Regex.Match (message, @"(\[Voi=\{.+?\}.+?\])(\[识别结果:(.+?)\])?");
			if (match.Success) {
				guid = match.Groups[1].Value;
				identifyResult = match.Groups.Count > 1 ? match.Groups[3].Value : null;
				return true;
			}
			return false;
		}

		/// <summary>
		/// 消息中是否包含图片
		/// </summary>
		/// <param name="message">消息文本</param>
		/// <param name="guids">提取出来的图片GUID</param>
		/// <returns></returns>
		public static bool ContainsPictureInMessage (string message, out List<string> guids) {
			guids = null;
			MatchCollection matchCollection = Regex.Matches (message, @"\[pic=\{.+?\}.+?\]");
			if (matchCollection.Count == 0) {
				return false;
			}
			guids = new List<string> ();
			foreach (Match match in matchCollection) {
				guids.Add (match.Value);
			}
			return true;
		}

		/// <summary>
		/// 是否为闪照消息
		/// </summary>
		/// <param name="message">消息文本</param>
		/// <returns></returns>
		public static bool IsFlashPictureMessage (string message) {
			return Regex.IsMatch (message, @"\[FlashPic=\{.+?\}.+?\]");
		}

		/// <summary>
		/// 将闪照转为图片
		/// </summary>
		/// <param name="flashPictureGUID"></param>
		/// <returns></returns>
		public static string FlashPictureToPicture (string flashPictureGUID) {
			return flashPictureGUID.Replace ("FlashPic", "pic");
		}

	}

}