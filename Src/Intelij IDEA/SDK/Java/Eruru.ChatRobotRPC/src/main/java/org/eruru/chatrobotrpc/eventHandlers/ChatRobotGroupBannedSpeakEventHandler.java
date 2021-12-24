package org.eruru.chatrobotrpc.eventHandlers;

public interface ChatRobotGroupBannedSpeakEventHandler {

	void invoke (boolean enable, long robot, long group, long qq);

}
