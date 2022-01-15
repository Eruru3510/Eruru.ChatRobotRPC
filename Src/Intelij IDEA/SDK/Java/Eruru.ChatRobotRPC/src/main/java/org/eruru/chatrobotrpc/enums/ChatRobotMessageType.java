package org.eruru.chatrobotrpc.enums;

/**
 * 消息类型
 */
public enum ChatRobotMessageType {

	/**
	 * 好友
	 */
	friend (1),
	/**
	 * 群临时
	 */
	groupTemp (2),
	/**
	 * 讨论组临时
	 */
	discussTemp (3),
	/**
	 * 网页临时
	 */
	webpageTemp (4),
	/**
	 * 好友验证回复
	 */
	friendVerificationReply (5),
	/**
	 * 群
	 */
	group (6),
	/**
	 * 讨论组
	 */
	discuss (7);

	private final int value;

	ChatRobotMessageType (int value) {
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
	public static ChatRobotMessageType get (int value) {
		for (ChatRobotMessageType item : ChatRobotMessageType.values ()) {
			if (item.value == value) {
				return item;
			}
		}
		return null;
	}

}