package org.eruru.chatrobotrpc.eventhandlers;

import org.eruru.chatrobotrpc.enums.ChatRobotGroupMemberJoinType;

/**
 * 群成员加入
 */
public interface ChatRobotGroupMemberJoinedEventHandler {

	/**
	 * @param type       加入类型
	 * @param robot      收到此事件机器人的QQ
	 * @param group      加入的群号
	 * @param qq         被批准或邀请加入群组的QQ号
	 * @param operatorQQ 邀请或批准请求的管理QQ
	 */
	void invoke (ChatRobotGroupMemberJoinType type, long robot, long group, long qq, long operatorQQ);

}
