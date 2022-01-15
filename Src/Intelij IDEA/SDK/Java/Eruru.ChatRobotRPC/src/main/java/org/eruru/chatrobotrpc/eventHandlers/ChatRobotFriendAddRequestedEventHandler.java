package org.eruru.chatrobotrpc.eventhandlers;

/**
 * 好友添加请求
 */
public interface ChatRobotFriendAddRequestedEventHandler {

	/**
	 * @param robot   收到此事件机器人的QQ
	 * @param qq      请求添加响应QQ好友的QQ号
	 * @param message 请求添加好友的附加消息
	 */
	void invoke (long robot, long qq, String message);

}
