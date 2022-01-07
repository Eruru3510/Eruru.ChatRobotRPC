package org.eruru.chatrobotrpc.eventhandlers;

public interface ChatRobotGroupBannedSpeakEventHandler {

	void invoke (boolean enable, long robot, long group, long qq);

}
