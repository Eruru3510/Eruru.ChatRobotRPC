package org.eruru.chatrobotrpc.informations;

public class ChatRobotGroupMemberListInformation {

	private  int memberNumber;
	private  int maxMemberNumber;
	private  long master;
	private  long[] administrators;

	public int getMemberNumber () {
		return memberNumber;
	}

	public void setMemberNumber (int memberNumber) {
		this.memberNumber = memberNumber;
	}

	public int getMaxMemberNumber () {
		return maxMemberNumber;
	}

	public void setMaxMemberNumber (int maxMemberNumber) {
		this.maxMemberNumber = maxMemberNumber;
	}

	public long getMaster () {
		return master;
	}

	public void setMaster (long master) {
		this.master = master;
	}

	public long[] getAdministrators () {
		return administrators;
	}

	public void setAdministrators (long[] administrators) {
		this.administrators = administrators;
	}
}
