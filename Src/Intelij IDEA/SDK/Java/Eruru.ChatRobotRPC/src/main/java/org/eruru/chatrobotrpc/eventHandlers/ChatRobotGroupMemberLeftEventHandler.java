package org.eruru.chatrobotrpc.eventhandlers;

/**
 * 群成员离开
 */
public interface ChatRobotGroupMemberLeftEventHandler {

	/**
	 * @param kick       是否是被踢
	 * @param robot      收到此事件机器人的QQ
	 * @param group      离开的群号
	 * @param qq         退出或被移出群组的QQ号
	 * @param operatorQQ 移除QQ的操作者QQ（如果是自己退群，则为-1）
	 */
	void invoke (boolean kick, long robot, long group, long qq, long operatorQQ);

}
