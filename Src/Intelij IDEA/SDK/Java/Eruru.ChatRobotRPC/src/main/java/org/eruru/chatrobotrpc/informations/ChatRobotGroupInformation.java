package org.eruru.chatrobotrpc.informations;

/**
 * 群信息
 */
public class ChatRobotGroupInformation {

	private long group;
	private String name;
	private long master;
	private boolean isAdministrator;

	/**
	 * 群号
	 */
	public long getGroup () {
		return group;
	}

	/**
	 * 群号
	 */
	public void setGroup (long group) {
		this.group = group;
	}

	/**
	 * 群名
	 */
	public String getName () {
		return name;
	}

	/**
	 * 群名
	 */
	public void setName (String name) {
		this.name = name;
	}

	/**
	 * 群主QQ
	 */
	public long getMaster () {
		return master;
	}

	/**
	 * 群主QQ
	 */
	public void setMaster (long master) {
		this.master = master;
	}

	/**
	 * 是否是管理员（群主时也为真,可通过群主QQ区分）
	 */
	public boolean isAdministrator () {
		return isAdministrator;
	}

	/**
	 * 是否是管理员（群主时也为真,可通过群主QQ区分）
	 */
	public void setAdministrator (boolean administrator) {
		isAdministrator = administrator;
	}
}
