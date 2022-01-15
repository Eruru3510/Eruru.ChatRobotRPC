package org.eruru.chatrobotrpc.enums;

/**
 * 事件类型
 */
public enum ChatRobotEventType {

	/**
	 * 收到自身消息
	 */
	receivedOwnMessage (2099),
	/**
	 * 群公告改变
	 */
	groupAnnouncementChanged (2013),
	/**
	 * 好友签名改变
	 */
	friendSignatureChanged (1004),
	/**
	 * 说说被评论
	 */
	talkWasCommented (1005),
	/**
	 * 好友正在输入
	 */
	friendIsTyping (1006),
	/**
	 * 好友今天首次发起会话
	 */
	friendFirstChatToday (1007),
	/**
	 * 被好友抖动
	 */
	wasJitterByFriend (1008),
	/**
	 * 收到财付通转账
	 */
	receivedTenPayTransfer (80001),
	/**
	 * 添加了新账号
	 */
	addedNewAccount (11000),
	/**
	 * QQ登录完成
	 */
	qqloggedIn (11001),
	/**
	 * QQ被手动离线
	 */
	qqWasOfflineByManual (11002),
	/**
	 * QQ被强制离线
	 */
	qqWasOfflineByForce (11003),
	/**
	 * QQ长时间无响应或掉线
	 */
	qqNoResponseForLongTimeOrOffline (11004);

	private final int value;

	ChatRobotEventType (int value) {
		this.value = value;
	}

	/**
	 * 获取枚举对应的整形
	 */
	public int getValue () {
		return value;
	}

	/**
	 * 通过整形获取对应的枚举
	 */
	public static ChatRobotEventType get (int value) {
		for (ChatRobotEventType item : ChatRobotEventType.values ()) {
			if (item.value == value) {
				return item;
			}
		}
		return null;
	}

}