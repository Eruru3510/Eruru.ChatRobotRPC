package org.eruru.chatrobotrpc.eventhandlers;

/**
 * 群被解散
 */
public interface ChatRobotGroupDisbandedEventHandler {

	/**
	 * @param robot 收到此事件机器人的QQ
	 * @param group 被解散的群号
	 * @param qq    解散群组的群主QQ
	 */
	void invoke (long robot, long group, long qq);

}
