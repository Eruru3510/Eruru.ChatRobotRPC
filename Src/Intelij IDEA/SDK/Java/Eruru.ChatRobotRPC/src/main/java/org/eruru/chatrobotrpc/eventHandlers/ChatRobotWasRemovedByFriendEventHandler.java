package org.eruru.chatrobotrpc.eventhandlers;

/**
 * 被好友删除
 */
public interface ChatRobotWasRemovedByFriendEventHandler {

	/**
	 * @param robot 收到此事件机器人的QQ
	 * @param qq    将响应QQ从好友列表中删除的QQ号
	 */
	void invoke (long robot, long qq);

}
