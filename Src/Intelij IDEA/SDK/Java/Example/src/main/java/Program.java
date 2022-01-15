import org.eruru.chatrobotrpc.ChatRobot;
import org.eruru.chatrobotrpc.enums.ChatRobotMessageType;

import java.io.IOException;
import java.util.Scanner;
import java.util.concurrent.TimeoutException;

public class Program {

	private static final ChatRobot chatRobot = new ChatRobot ();

	public static void main (String[] args) {
		chatRobot.setOnReceivedMessage (message -> {//当收到消息
			if (message.getType () == ChatRobotMessageType.friend) {//如果是好友消息
				message.reply ("%s发送了：%s", chatRobot.getName (message.getRobot (), message.getQQ ()), message);//复读
			}
		});
		chatRobot.setOnDisconnected (() -> {//当与机器人框架的Chat Robot RPC插件断开了连接
			System.out.println ("连接断开");
			connect ();//重连
		});
		connect ();//连接机器人框架的Chat Robot RPC插件
		new Scanner (System.in).nextLine ();//保持运行
	}

	static void connect () {
		try {
			System.out.println ("开始连接");
			chatRobot.connect ("localhost", 19730, "root", "root");
			System.out.println ("连接成功");
		} catch (IOException ioException) {
			ioException.printStackTrace ();
			connect ();
		} catch (TimeoutException timeoutException) {
			System.out.println ("连接成功，但是响应登录请求超时");
			timeoutException.printStackTrace ();
			connect ();
		} catch (Exception exception) {
			exception.printStackTrace ();
		}
	}

}