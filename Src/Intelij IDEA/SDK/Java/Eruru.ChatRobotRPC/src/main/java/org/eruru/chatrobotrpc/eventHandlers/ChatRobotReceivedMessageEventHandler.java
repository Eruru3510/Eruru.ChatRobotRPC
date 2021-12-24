package org.eruru.chatrobotrpc.eventHandlers;

import org.eruru.chatrobotrpc.ChatRobotMessage;

public interface ChatRobotReceivedMessageEventHandler {

	void invoke (ChatRobotMessage message);

}