package org.eruru.chatrobotrpc.informations;

public class ChatRobotGroupInformation {

	private long group ;
	private String name ;
	private long master ;
	private boolean isAdministrator;

	public long getGroup () {
		return group;
	}

	public void setGroup (long group) {
		this.group = group;
	}

	public String getName () {
		return name;
	}

	public void setName (String name) {
		this.name = name;
	}

	public long getMaster () {
		return master;
	}

	public void setMaster (long master) {
		this.master = master;
	}

	public boolean isAdministrator () {
		return isAdministrator;
	}

	public void setAdministrator (boolean administrator) {
		isAdministrator = administrator;
	}
}
