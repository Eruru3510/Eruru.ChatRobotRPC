package org.eruru.chatrobotrpc.eventhandlers;

/**
 * 群消息撤回
 */
public interface ChatRobotGroupMessageRevokedEventHandler {

	/**
	 * @param robot         收到此事件机器人的QQ
	 * @param group         撤回消息的来源群号
	 * @param qq            撤回消息的QQ号
	 * @param messageNumber 被撤回消息的序号
	 * @param messageID     被撤回消息的标识
	 */
	void invoke (long robot, long group, long qq, long messageNumber, long messageID);

}