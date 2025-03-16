//package com.keepitup.chat.notification.api.ChatAndNotification.API.configuration;
//
//import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.entity.Chat;
//import com.keepitup.chat.notification.api.ChatAndNotification.API.jwt.CustomJwt;
//import org.springframework.beans.factory.annotation.Autowired;
//import org.springframework.http.HttpStatus;
//import org.springframework.security.core.context.SecurityContextHolder;
//import org.springframework.stereotype.Service;
//import org.springframework.web.server.ResponseStatusException;
//
//import java.lang.reflect.Field;
//import java.util.*;
//
//@Service
//public class SecurityService {
//    private final OrganizationService organizationService;
//    private final RoleDefaultService roleService;
//
//
//    @Autowired
//    public SecurityService(
//            OrganizationService organizationService,
//            RoleDefaultService roleService
//    ) {
//        this.organizationService = organizationService;
//        this.roleService = roleService;
//    }
//
//    public Boolean hasPermission(Organization organization, String permissionName) {
//        var jwt = (CustomJwt) SecurityContextHolder.getContext().getAuthentication();
//
//        List<String> userRoles = jwt.getMembershipMap().get(organization.getName());
//
//        if (userRoles == null || userRoles.isEmpty()) {
//            return false;
//        }
//
//        for (String userRole : userRoles) {
//            Role role = roleService.findByNameAndOrganization(userRole, organization)
//                    .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
//            if (hasRolePermission(role, permissionName) || role.getIsAdmin()) {
//                return true;
//            }
//        }
//        return false;
//    }
//
//    private boolean hasRolePermission(Role role, String permissionName) {
//        try {
//            Field permissionField = Role.class.getDeclaredField(permissionName);
//            permissionField.setAccessible(true);
//            Boolean hasPermission = (Boolean) permissionField.get(role);
//            return hasPermission != null && hasPermission;
//        } catch (NoSuchFieldException | IllegalAccessException e) {
//            throw new IllegalArgumentException("Invalid permission name: " + permissionName, e);
//        }
//    }
//
//    public boolean hasAdminPermission() {
//        var jwt = (CustomJwt) SecurityContextHolder.getContext().getAuthentication();
//
//        Map<String, List<String>> userRoles = jwt.getMembershipMap();
//        if (userRoles == null || userRoles.isEmpty()) {
//            return false;
//        }
//
//        Set<String> organizationNames = userRoles.keySet();
//        List<Organization> organizations = new ArrayList<>();
//
//        for (String organizationName : organizationNames) {
//            organizations.add(organizationService.findByName(organizationName).get());
//        }
//
//        for (Organization organization : organizations) {
//            for (String roleName : userRoles.get(organization.getName())) {
//                Role role = roleService.findByNameAndOrganization(roleName, organization)
//                        .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));
//                if (role.getIsAdmin() != null && role.getIsAdmin()) {
//                    return true;
//                }
//            }
//        }
//        return false;
//    }
//
//    public boolean belongsToOrganization(Organization organization) {
//        var jwt = (CustomJwt) SecurityContextHolder.getContext().getAuthentication();
//
//        return jwt.getMembershipMap().containsKey(organization.getName());
//    }
//
//    public boolean belongsToChat(Chat chat, Organization organization) {
//        Member member = getCurrentMember(organization);
//        return chat.getChatMembers().stream()
//                .anyMatch(chatMember -> chatMember.getMember().equals(member));
//    }
//    public Member getCurrentMember(Organization organization) {
//        var jwt = (CustomJwt) SecurityContextHolder.getContext().getAuthentication();
//        UUID loggedInUserId = UUID.fromString(jwt.getExternalId());
//
//        return organization.getMembers().stream()
//                .filter(member -> member.getUser().getId().equals(loggedInUserId) && member.getIsStillMember())
//                .findFirst()
//                .orElseThrow(() -> new ResponseStatusException(HttpStatus.FORBIDDEN));
//    }
//
//    public boolean isCurrentMember(Member member) {
//        var jwt = (CustomJwt) SecurityContextHolder.getContext().getAuthentication();
//        UUID loggedInUserId = UUID.fromString(jwt.getExternalId());
//
//        return loggedInUserId.equals(member.getUser().getId());
//    }
//
//    public boolean isCurrentUser(User user) {
//        var jwt = (CustomJwt) SecurityContextHolder.getContext().getAuthentication();
//        UUID loogedInUserId = UUID.fromString(jwt.getExternalId());
//
//        return loogedInUserId.equals(user.getId());
//    }
//
//    public boolean isAnyMember() {
//        var jwt = (CustomJwt) SecurityContextHolder.getContext().getAuthentication();
//
//        return !jwt.getMembershipMap().isEmpty();
//    }
//
//    public boolean isOwner(Organization organization) {
//        var jwt = (CustomJwt) SecurityContextHolder.getContext().getAuthentication();
//
//        List<String> userRoles = jwt.getMembershipMap().get(organization.getName());
//
//        if (userRoles == null || userRoles.isEmpty()) {
//            return false;
//        }
//
//        return userRoles.contains(Constants.ROLE_NAME_OWNER);
//    }
//
//    public boolean isChatAdmin(Chat chat) {
//        Member member = getCurrentMember(chat.getOrganization());
//        return chat.getChatAdministrators().stream()
//                .anyMatch(chatMember -> chatMember.getMember().equals(member));
//    }
//
//    public boolean isChatMember(Chat chat) {
//        Member member = getCurrentMember(chat.getOrganization());
//        return chat.getChatMembers().stream()
//                .anyMatch(chatMember -> chatMember.getMember().equals(member));
//    }
//}
