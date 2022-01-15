package org.eruru.chatrobotrpc.eventhandlers;

/**
 * 群成员名片改变
 */
public interface ChatRobotGroupMemberBusinessCardChangedEventHandler {

	/**
	 * @param robot        收到此事件机器人的QQ
	 * @param group        群成员名片改的群号
	 * @param qq           改变名片的人的QQ号
	 * @param businessCard 修改完的群名片
	 */
	void invoke (long robot, long group, long qq, String businessCard);

}
