package org.eruru.chatrobotrpc.eventHandlers;

public interface ChatRobotGroupMessageRevokedEventHandler {

	void invoke (long robot, long group, long qq, long messageNumber, long messageID);

}