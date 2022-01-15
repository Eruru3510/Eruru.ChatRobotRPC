package org.eruru.chatrobotrpc.informations;

import org.eruru.chatrobotrpc.enums.ChatRobotGender;

/**
 * 好友信息
 */
public class ChatRobotFriendInformation {

	private long qq;
	private String name;
	private ChatRobotGender gender;
	private int age;
	private String country;
	private String province;
	private String city;
	private String avaterURL;

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
	 * 昵称
	 */
	public String getName () {
		return name;
	}

	/**
	 * 昵称
	 */
	public void setName (String name) {
		this.name = name;
	}

	/**
	 * 性别（255隐藏 0男 1女）
	 */
	public ChatRobotGender getGender () {
		return gender;
	}

	/**
	 * 性别（255隐藏 0男 1女）
	 */
	public void setGender (ChatRobotGender gender) {
		this.gender = gender;
	}

	/**
	 * 年龄
	 */
	public int getAge () {
		return age;
	}

	/**
	 * 年龄
	 */
	public void setAge (int age) {
		this.age = age;
	}

	/**
	 * 国家
	 */
	public String getCountry () {
		return country;
	}

	/**
	 * 国家
	 */
	public void setCountry (String country) {
		this.country = country;
	}

	/**
	 * 省份
	 */
	public String getProvince () {
		return province;
	}

	/**
	 * 省份
	 */
	public void setProvince (String province) {
		this.province = province;
	}

	/**
	 * 城市
	 */
	public String getCity () {
		return city;
	}

	/**
	 * 城市
	 */
	public void setCity (String city) {
		this.city = city;
	}

	/**
	 * 头像URL
	 */
	public String getAvaterURL () {
		return avaterURL;
	}

	/**
	 * 头像URL
	 */
	public void setAvaterURL (String avaterURL) {
		this.avaterURL = avaterURL;
	}

}