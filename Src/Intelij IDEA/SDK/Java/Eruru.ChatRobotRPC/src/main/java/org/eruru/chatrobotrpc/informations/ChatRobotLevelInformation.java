package org.eruru.chatrobotrpc.informations;

/**
 * 等级信息
 */
public class ChatRobotLevelInformation {

	private String vip;
	private int level;
	private int activeDays;
	private int daysRemainingForUpgrade;

	/**
	 * 会员
	 */
	public String getVip () {
		return vip;
	}

	/**
	 * 会员
	 */
	public void setVip (String vip) {
		this.vip = vip;
	}

	/**
	 * 等级
	 */
	public int getLevel () {
		return level;
	}

	/**
	 * 等级
	 */
	public void setLevel (int level) {
		this.level = level;
	}

	/**
	 * 活跃天数
	 */
	public int getActiveDays () {
		return activeDays;
	}

	/**
	 * 活跃天数
	 */
	public void setActiveDays (int activeDays) {
		this.activeDays = activeDays;
	}

	/**
	 * 升级剩余天数
	 */
	public int getDaysRemainingForUpgrade () {
		return daysRemainingForUpgrade;
	}

	/**
	 * 升级剩余天数
	 */
	public void setDaysRemainingForUpgrade (int daysRemainingForUpgrade) {
		this.daysRemainingForUpgrade = daysRemainingForUpgrade;
	}

}