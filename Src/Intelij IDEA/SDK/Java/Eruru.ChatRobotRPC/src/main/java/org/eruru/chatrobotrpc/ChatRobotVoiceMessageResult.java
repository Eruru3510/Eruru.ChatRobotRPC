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

	/**
	 * 提取出来的语音GUID
	 */
	public String getGuid () {
		return guid;
	}

	/**
	 * 提取出来的语音GUID
	 */
	public void setGuid (String guid) {
		this.guid = guid;
	}

	/**
	 * 提取出来的语音识别结果
	 */
	public String getIdentifyResult () {
		return identifyResult;
	}

	/**
	 * 提取出来的语音识别结果
	 */
	public void setIdentifyResult (String identifyResult) {
		this.identifyResult = identifyResult;
	}

	/**
	 * 是否为语音消息
	 */
	public boolean isSuccess () {
		return success;
	}

	/**
	 * 是否为语音消息
	 */
	public void setSuccess (boolean success) {
		this.success = success;
	}

}