package org.eruru.chatrobotrpc.enums;

public enum ChatRobotEventType {

	receivedOwnMessage (2099),
	/// <summary>
	/// 群公告改变
	/// </summary>
	groupAnnouncementChanged (2013),
	/// <summary>
	/// 好友签名改变
	/// </summary>
	friendSignatureChanged (1004),
	/// <summary>
	/// 说说被评论
	/// </summary>
	talkWasCommented (1005),
	/// <summary>
	/// 好友正在输入
	/// </summary>
	friendIsTyping (1006),
	/// <summary>
	/// 好友今天首次发起会话
	/// </summary>
	friendFirstChatToday (1007),
	/// <summary>
	/// 被好友抖动
	/// </summary>
	wasJitterByFriend (1008),
	/// <summary>
	/// 收到财付通转账
	/// </summary>
	receivedTenPayTransfer (80001),
	/// <summary>
	/// 添加了新账号
	/// </summary>
	addedNewAccount (11000),
	/// <summary>
	/// QQ登录完成
	/// </summary>
	qqloggedIn (11001),
	/// <summary>
	/// QQ被手动离线
	/// </summary>
	qqWasOfflineByManual (11002),
	/// <summary>
	/// QQ被强制离线
	/// </summary>
	qqWasOfflineByForce (11003),
	/// <summary>
	/// QQ长时间无响应或掉线
	/// </summary>
	qqNoResponseForLongTimeOrOffline (11004);

	private final int value;

	ChatRobotEventType (int value) {
		this.value = value;
	}

	public int getValue () {
		return value;
	}

	public static ChatRobotEventType get (int value) {
		for (ChatRobotEventType item : ChatRobotEventType.values ()) {
			if (item.value == value) {
				return item;
			}
		}
		return null;
	}

}