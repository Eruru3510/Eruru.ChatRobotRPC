package org.eruru.chatrobotrpc.eventhandlers;

/**
 * 好友状态改变
 */
public interface ChatRobotFriendStateChangedEventHandler {

	/**
	 * @param robot 收到此事件机器人的QQ
	 * @param qq    改变状态的人的QQ号
	 * @param state 在线状态
	 */
	void invoke (long robot, long qq, String state);

}
