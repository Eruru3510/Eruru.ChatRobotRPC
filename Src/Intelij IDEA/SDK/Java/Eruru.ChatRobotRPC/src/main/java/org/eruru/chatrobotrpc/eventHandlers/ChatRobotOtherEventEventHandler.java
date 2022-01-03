package org.eruru.chatrobotrpc.eventHandlers;

public interface ChatRobotOtherEventEventHandler {

	void invoke (long robot, int eventType, int subType, long source, long active, long passive, String message, long messageNumber, long messageID);

}