package org.eruru.chatrobotrpc.enums;

/**
 * 请求类型
 */
public enum ChatRobotRequestType {

	/**
	 * 忽略
	 */
	ignore (30),
	/**
	 * 通过
	 */
	agree (10),
	/**
	 * 拒绝
	 */
	refuse (20);

	private final int value;

	ChatRobotRequestType (int value) {
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
	public static ChatRobotRequestType get (int value) {
		for (ChatRobotRequestType item : ChatRobotRequestType.values ()) {
			if (item.value == value) {
				return item;
			}
		}
		return null;
	}

}
