package org.eruru.chatrobotrpc;

public class ChatRobotVoiceMessageResult {

	private boolean success;
	private String guid;
	private String identifyResult;

	public ChatRobotVoiceMessageResult (boolean success, String guid, String identifyResult) {
		this.success = success;
		this.guid = guid;
		this.identifyResult = identifyResult;
	}

	public ChatRobotVoiceMessageResult (String guid, String identifyResult) {
		this (true, guid, identifyResult);
	}

	public ChatRobotVoiceMessageResult () {
		this (false, null, null);
	}

	public String getGuid () {
		return guid;
	}

	public void setGuid (String guid) {
		this.guid = guid;
	}

	public String getIdentifyResult () {
		return identifyResult;
	}

	public void setIdentifyResult (String identifyResult) {
		this.identifyResult = identifyResult;
	}

	public boolean isSuccess () {
		return success;
	}

	public void setSuccess (boolean success) {
		this.success = success;
	}

}