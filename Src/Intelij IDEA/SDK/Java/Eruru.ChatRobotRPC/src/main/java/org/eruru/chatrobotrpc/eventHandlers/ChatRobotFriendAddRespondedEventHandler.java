package org.eruru.chatrobotrpc.eventhandlers;

/**
 * 好友添加响应
 */
public interface ChatRobotFriendAddRespondedEventHandler {

	/**
	 * @param agree   是否同意
	 * @param robot   收到此事件机器人的QQ
	 * @param qq      同意或拒绝添加响应QQ的QQ号
	 * @param message 拒绝好友时的附加消息/目前暂不支持
	 */
	void invoke (boolean agree, long robot, long qq, String message);

}
