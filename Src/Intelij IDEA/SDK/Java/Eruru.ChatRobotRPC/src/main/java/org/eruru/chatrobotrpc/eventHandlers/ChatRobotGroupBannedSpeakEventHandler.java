package org.eruru.chatrobotrpc.eventhandlers;

/**
 * 群禁言
 */
public interface ChatRobotGroupBannedSpeakEventHandler {

	/**
	 * @param enable 全群禁言是否开启
	 * @param robot  收到此事件机器人的QQ
	 * @param group  开启或关闭群体禁言的群号
	 * @param qq     开启或关闭群体禁言的QQ号
	 */
	void invoke (boolean enable, long robot, long group, long qq);

}
