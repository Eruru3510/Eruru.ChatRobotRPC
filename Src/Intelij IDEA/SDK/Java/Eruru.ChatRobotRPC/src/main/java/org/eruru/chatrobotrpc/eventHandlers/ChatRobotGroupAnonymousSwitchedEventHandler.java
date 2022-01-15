package org.eruru.chatrobotrpc.eventhandlers;

/**
 * 群匿名开关
 */
public interface ChatRobotGroupAnonymousSwitchedEventHandler {

	/**
	 * @param enable 群匿名是否开启
	 * @param robot  收到此事件机器人的QQ
	 * @param group  发生事件的群号
	 * @param qq     更改群匿名状态的管理员QQ号
	 */
	void invoke (boolean enable, long robot, long group, long qq);

}
