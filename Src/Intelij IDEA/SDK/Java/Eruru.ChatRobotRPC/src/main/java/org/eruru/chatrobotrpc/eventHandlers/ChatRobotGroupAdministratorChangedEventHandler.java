package org.eruru.chatrobotrpc.eventhandlers;

/**
 * 群管理员改变
 */
public interface ChatRobotGroupAdministratorChangedEventHandler {

	/**
	 * @param enable 是否成为管理
	 * @param robot  收到此事件机器人的QQ
	 * @param group  管理员改变的群号
	 * @param qq     成为或被取消管理员的QQ号
	 */
	void invoke (boolean enable, long robot, long group, long qq);

}
