package org.eruru.chatrobotrpc.eventhandlers;

import org.eruru.chatrobotrpc.enums.ChatRobotGroupAddRequestType;

/**
 * 群添加请求
 */
public interface ChatRobotGroupAddRequestedEventHandler {

	/**
	 * @param type      请求类型
	 * @param robot     收到此事件机器人的QQ
	 * @param group     申请要加入的群号
	 * @param qq        申请或被邀请加入群组者的QQ号
	 * @param inviterQQ 邀请者的QQ号
	 * @param sign      唯一表示本次请求,用于处理请求
	 * @param message   附加理由
	 */
	void invoke (ChatRobotGroupAddRequestType type, long robot, long group, long qq, long inviterQQ, long sign, String message);

}