package org.eruru.chatrobotrpc.enums;

/**
 * 在线状态
 */
public enum ChatRobotState {

	/**
	 * 在线
	 */
	online (1),
	/**
	 * Q我吧
	 */
	qMe (2),
	/**
	 * 离开
	 */
	leave (3),
	/**
	 * 忙碌
	 */
	busy (4),
	/**
	 * 勿扰
	 */
	doNotDisturb (5),
	/**
	 * 隐身
	 */
	invisible (6);

	private final int value;

	ChatRobotState (int value) {
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
	public static ChatRobotState get (int value) {
		for (ChatRobotState item : ChatRobotState.values ()) {
			if (item.value == value) {
				return item;
			}
		}
		return null;
	}

}