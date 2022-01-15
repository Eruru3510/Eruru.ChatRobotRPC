package org.eruru.chatrobotrpc.eventhandlers;

/**
 * 群成员禁言
 */
public interface ChatRobotGroupMemberBannedSpeakEventHandler {

	/**
	 * @param enable     是否被禁言
	 * @param robot      收到此事件机器人的QQ
	 * @param group      发生事件的群号
	 * @param qq         被禁止发言的QQ号
	 * @param operatorQQ 设置禁言的QQ号 (管理员或群主)
	 * @param seconds    禁言时长, 单位: 秒, 范围: 1秒-30天
	 */
	void invoke (boolean enable, long robot, long group, long qq, long operatorQQ, int seconds);

}
