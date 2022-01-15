package org.eruru.chatrobotrpc.informations;

/**
 * 群成员列表信息
 */
public class ChatRobotGroupMemberListInformation {

	private int memberNumber;
	private int maxMemberNumber;
	private long master;
	private long[] administrators;

	/**
	 * 群人数
	 */
	public int getMemberNumber () {
		return memberNumber;
	}

	/**
	 * 群人数
	 */
	public void setMemberNumber (int memberNumber) {
		this.memberNumber = memberNumber;
	}

	/**
	 * 群人数上限
	 */
	public int getMaxMemberNumber () {
		return maxMemberNumber;
	}

	/**
	 * 群人数上限
	 */
	public void setMaxMemberNumber (int maxMemberNumber) {
		this.maxMemberNumber = maxMemberNumber;
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
	 * 群管理员
	 */
	public long[] getAdministrators () {
		return administrators;
	}

	/**
	 * 群管理员
	 */
	public void setAdministrators (long[] administrators) {
		this.administrators = administrators;
	}

}
