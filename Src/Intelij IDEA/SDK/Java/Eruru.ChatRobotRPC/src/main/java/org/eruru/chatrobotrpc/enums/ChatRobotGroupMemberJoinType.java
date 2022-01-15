package org.eruru.chatrobotrpc.enums;

/**
 * 群成员加入类型
 */
public enum ChatRobotGroupMemberJoinType {

	/**
	 * 某人被批准加入
	 */
	approve (1),
	/**
	 * 我加入某个群
	 */
	iJoin (2),
	/**
	 * 某人被邀请加入了群
	 */
	invite (3);

	private final int value;

	ChatRobotGroupMemberJoinType (int value) {
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
	public static ChatRobotGroupMemberJoinType get (int value) {
		for (ChatRobotGroupMemberJoinType item : ChatRobotGroupMemberJoinType.values ()) {
			if (item.value == value) {
				return item;
			}
		}
		return null;
	}

}