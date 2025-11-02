package com.keepitup.calendar.api.Calendar.API.configuration;

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

}
