package org.eruru.chatrobotrpc.enums;

/**
 * 发送消息类型
 */
public enum ChatRobotSendMessageType {

	/**
	 * 文本
	 */
	text (0),
	json (1),
	xml (2);

	private final int value;

	ChatRobotSendMessageType (int value) {
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
	public static ChatRobotSendMessageType get (int value) {
		for (ChatRobotSendMessageType item : ChatRobotSendMessageType.values ()) {
			if (item.value == value) {
				return item;
			}
		}
		return null;
	}

}