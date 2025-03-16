package com.keepitup.chat.notification.api.ChatAndNotification.API.configuration;

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

    // Notification templates
    public static final String NOTIFICATION_USER_UPDATE_TEMPLATE = "User profile has been updated";
    public static final String NOTIFICATION_INVITATION_CREATION_TEMPLATE = "You have been invited to organization %s";
    public static final String NOTIFICATION_INVITATION_ACCEPT_TEMPLATE = "You have joined organization %s";
    public static final String NOTIFICATION_ORGANIZATION_CREATION_TEMPLATE = "Organization %s has been created";
    public static final String NOTIFICATION_ORGANIZATION_DELETION_TEMPLATE = "Organization %s has been deleted";
    public static final String NOTIFICATION_ORGANIZATION_UPDATE_TEMPLATE = "Organization %s profile has been updated";
    public static final String NOTIFICATION_MEMBER_CREATION_TEMPLATE = "You have been added to organization %s";
    public static final String NOTIFICATION_MEMBER_DELETION_TEMPLATE = "You have been removed from organization %s";
    public static final String NOTIFICATION_MEMBER_UPDATE_TEMPLATE = "Member profile has been updated in organization %s";
    public static final String NOTIFICATION_ROLE_MEMBER_CREATION_TEMPLATE = "You have been assigned a new role (%s) in organization %s";
    public static final String NOTIFICATION_ROLE_MEMBER_DELETION_TEMPLATE = "Your have been removed from role %s";
    public static final String NOTIFICATION_MATERIAL_RECEIVER_CREATION_TEMPLATE = "New material has been assigned to you in organization %s";
    public static final String NOTIFICATION_MATERIAL_RECEIVER_DELETION_TEMPLATE = "Material %s has been removed from you in organization %s";
    public static final String NOTIFICATION_ASSIGNEE_CREATION_TEMPLATE = "You have been assigned a new task in organization %s";
    public static final String NOTIFICATION_ASSIGNEE_DELETION_TEMPLATE = "You have been removed from a task in organization %s";
    public static final String NOTIFICATION_ANNOUNCEMENT_RECEIVER_CREATION_TEMPLATE = "New announcement in organization %s";
}
