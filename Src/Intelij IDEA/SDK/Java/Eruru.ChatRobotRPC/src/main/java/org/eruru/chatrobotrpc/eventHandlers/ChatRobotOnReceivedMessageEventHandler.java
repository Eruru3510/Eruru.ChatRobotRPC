package org.eruru.chatrobotrpc.eventHandlers;

import org.eruru.chatrobotrpc.ChatRobotMessage;

public interface ChatRobotOnReceivedMessageEventHandler {

	void invoke (ChatRobotMessage message);

}