package org.eruru.chatrobotrpc.enums;

/**
 * 好友添加方式
 */
public enum ChatRobotFriendAddMethod {

	/**
	 * 允许任何人
	 */
	allowAny (0),
	/**
	 * 需要验证
	 */
	needValidation (1),
	/**
	 * 需要正确答案
	 */
	needRightAnswer (3),
	/**
	 * 需要回答问题
	 */
	needAnswerQuestion (4),
	/**
	 * 已经是好友
	 */
	alreadyFriend (99);

	private final int value;

	ChatRobotFriendAddMethod (int value) {
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
	public static ChatRobotFriendAddMethod get (int value) {
		for (ChatRobotFriendAddMethod item : ChatRobotFriendAddMethod.values ()) {
			if (item.value == value) {
				return item;
			}
		}
		return null;
	}

}
