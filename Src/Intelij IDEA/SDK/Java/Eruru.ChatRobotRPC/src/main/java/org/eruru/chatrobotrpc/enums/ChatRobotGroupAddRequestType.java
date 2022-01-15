package org.eruru.chatrobotrpc.enums;

/**
 * 群添加请求类型
 */
public enum ChatRobotGroupAddRequestType {

	/**
	 * 有人申请加群
	 */
	request (1),
	/**
	 * 某人邀请我加群
	 */
	inviteMe (2),
	/**
	 * 群员邀请某人加群
	 */
	memberInvite (3);

	private final int value;

	ChatRobotGroupAddRequestType (int value) {
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
	public static ChatRobotGroupAddRequestType get (int value) {
		for (ChatRobotGroupAddRequestType item : ChatRobotGroupAddRequestType.values ()) {
			if (item.value == value) {
				return item;
			}
		}
		return null;
	}

}
