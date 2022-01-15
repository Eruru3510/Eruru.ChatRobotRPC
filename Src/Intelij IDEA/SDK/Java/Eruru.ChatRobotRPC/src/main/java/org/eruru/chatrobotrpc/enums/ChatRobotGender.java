package org.eruru.chatrobotrpc.enums;

/**
 * 性别
 */
public enum ChatRobotGender {

	/**
	 * 男
	 */
	male (0),
	/**
	 * 女
	 */
	female (1),
	/**
	 * 隐藏
	 */
	hide (255);

	private final int value;

	ChatRobotGender (int value) {
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
	public static ChatRobotGender get (int value) {
		for (ChatRobotGender item : ChatRobotGender.values ()) {
			if (item.value == value) {
				return item;
			}
		}
		return null;
	}

}
