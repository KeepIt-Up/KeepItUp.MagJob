package com.keepitup.magjob.chat.configuration;

public class Constants {
    // Role Names
    public static final String ROLE_NAME_OWNER = "Owner";
    public static final String ROLE_NAME_MODERATOR = "Moderator";
    public static final String ROLE_NAME_MEMBER = "Member";
    public static final String[] DEFAULT_ROLE_NAMES = {ROLE_NAME_OWNER, ROLE_NAME_MODERATOR, ROLE_NAME_MEMBER};

    // Permission names
    public static final String PERMISSION_NAME_CAN_MANAGE_TASKS = "canManageTasks";
    public static final String PERMISSION_NAME_CAN_MANAGE_ANNOUNCEMENTS = "canManageAnnouncements";
    public static final String PERMISSION_NAME_CAN_MANAGE_INVITATIONS = "canManageInvitations";
    public static final String PERMISSION_NAME_CAN_MANAGE_ROLES = "canManageRoles";

    // Chat
    public static final String CHAT_DEFAULT_WEBSOCKET_ENDPOINT = "/topic/chat";
    public static final String CHAT_JOIN_MESSAGE = "Chat member %s has joined chat";
    public static final String CHAT_LEAVE_MESSAGE = "Chat member %s has left chat";
    public static final String CHAT_ADD_ADMIN_MESSAGE = "Chat member %s has been granted administrator privileges";
    public static final String CHAT_DELETE_ADMIN_MESSAGE = "Chat member %s is no longer administrator in this chat";

    // Notification websocket
    public static final String NOTIFICATION_USER_DEFAULT_WEBSOCKET_ENDPOINT = "/topic/user";
    public static final String NOTIFICATION_MEMBER_DEFAULT_WEBSOCKET_ENDPOINT = "/topic/member";
    public static final String NOTIFICATION_ORGANIZATION_DEFAULT_WEBSOCKET_ENDPOINT = "/topic/organization";
    public static final String NOTIFICATION_ENDPOINT = "/notifications";

    // Notification
    public static final String NOTIFICATION_CHAT_MESSAGE_TEMPLATE = "Nowa wiadomość na czacie \"%s\"";
}
