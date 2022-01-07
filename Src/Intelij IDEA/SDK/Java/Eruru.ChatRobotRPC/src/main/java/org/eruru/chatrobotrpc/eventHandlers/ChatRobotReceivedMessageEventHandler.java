package org.eruru.chatrobotrpc.eventhandlers;

import org.eruru.chatrobotrpc.ChatRobotMessage;

public interface ChatRobotReceivedMessageEventHandler {

	void invoke (ChatRobotMessage message);

}