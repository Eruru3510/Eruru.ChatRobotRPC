package org.eruru.chatrobotrpc.informations;

/**
 * 好友列表信息
 */
public class ChatRobotFriendListInformation {

	private long qq;
	private String notes;
	private String group;

	/**
	 * 好友QQ
	 */
	public long getQQ () {
		return qq;
	}

	/**
	 * 好友QQ
	 */
	public void setQQ (long qq) {
		this.qq = qq;
	}

	/**
	 * 备注
	 */
	public String getNotes () {
		return notes;
	}

	/**
	 * 备注
	 */
	public void setNotes (String notes) {
		this.notes = notes;
	}

	/**
	 * 所在分组
	 */
	public String getGroup () {
		return group;
	}

	/**
	 * 所在分组
	 */
	public void setGroup (String group) {
		this.group = group;
	}

}