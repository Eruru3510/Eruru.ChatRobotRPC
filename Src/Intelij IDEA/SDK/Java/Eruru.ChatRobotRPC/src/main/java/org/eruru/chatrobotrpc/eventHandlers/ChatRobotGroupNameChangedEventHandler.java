package org.eruru.chatrobotrpc.eventhandlers;

/**
 * 群名改变
 */
public interface ChatRobotGroupNameChangedEventHandler {

	/**
	 * @param robot 收到此事件机器人的QQ
	 * @param group 改名的群号
	 * @param name  改完的群名
	 */
	void invoke (long robot, long group, String name);

}
