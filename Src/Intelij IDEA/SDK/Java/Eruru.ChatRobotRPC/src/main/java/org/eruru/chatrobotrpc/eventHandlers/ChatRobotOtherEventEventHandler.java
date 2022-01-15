package org.eruru.chatrobotrpc.eventhandlers;

/**
 * 其他事件
 */
public interface ChatRobotOtherEventEventHandler {

	/**
	 * @param robot         收到此事件机器人的QQ
	 * @param eventType     事件类型
	 * @param subType       事件子类型
	 * @param source        事件来源
	 * @param active        主动对象
	 * @param passive       被动对象
	 * @param message       消息内容
	 * @param messageNumber 消息序号
	 * @param messageID     消息ID
	 */
	void invoke (long robot, int eventType, int subType, long source, long active, long passive, String message, long messageNumber, long messageID);

}