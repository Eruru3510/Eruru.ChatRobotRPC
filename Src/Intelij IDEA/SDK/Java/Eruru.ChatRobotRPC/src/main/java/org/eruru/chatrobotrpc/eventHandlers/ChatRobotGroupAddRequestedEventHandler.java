package org.eruru.chatrobotrpc.eventhandlers;

import org.eruru.chatrobotrpc.enums.ChatRobotGroupAddRequestType;

public interface ChatRobotGroupAddRequestedEventHandler {

	void invoke (ChatRobotGroupAddRequestType type, long robot, long group, long qq, long inviterQQ, long sign, String message);

}