package com.keepitup.chat.notification.api.ChatAndNotification.API.notification.controller.api;

import com.keepitup.chat.notification.api.ChatAndNotification.API.configuration.PageConfig;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.dto.GetNotificationResponse;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.dto.GetNotificationsResponse;
import com.keepitup.chat.notification.api.ChatAndNotification.API.notification.dto.PostNotificationRequest;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

import java.math.BigInteger;
import java.util.UUID;

@Tag(name = "Notification Controller")
public interface NotificationController {
    PageConfig pageConfig = new PageConfig();

    @Operation(summary = "Get All Notifications")
    @GetMapping("api/notifications")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetNotificationsResponse getNotifications(
            @Parameter(
                    name = "page number",
                    description = "Page number to retrieve"
            )
            @RequestParam(defaultValue = "#{pageConfig.number}")
            int page,
            @Parameter(
                    name = "page size",
                    description = "Number of records per page"
            )
            @RequestParam(defaultValue = "#{pageConfig.size}")
            int size
    );

    @Operation(summary = "Get All Seen/Unseen Notifications")
    @GetMapping("api/notifications/seen/{seen}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetNotificationsResponse getSeenOrUnseenNotifications(
            @Parameter(
                    name = "page number",
                    description = "Page number to retrieve"
            )
            @RequestParam(defaultValue = "#{pageConfig.number}")
            int page,
            @Parameter(
                    name = "page size",
                    description = "Number of records per page"
            )
            @RequestParam(defaultValue = "#{pageConfig.size}")
            int size,
            @Parameter(
                    name = "seen",
                    description = "Notification seen value"
            )
            @PathVariable("seen")
            boolean seen
    );

    @Operation(summary = "Get All Not Sent Notifications")
    @GetMapping("api/notifications/sent/{sent}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetNotificationsResponse getSentOrNotSentNotifications(
            @Parameter(
                    name = "page number",
                    description = "Page number to retrieve"
            )
            @RequestParam(defaultValue = "#{pageConfig.number}")
            int page,
            @Parameter(
                    name = "page size",
                    description = "Number of records per page"
            )
            @RequestParam(defaultValue = "#{pageConfig.size}")
            int size,
            @Parameter(
                    name = "seen",
                    description = "Notification seen value"
            )
            @PathVariable("sent")
            boolean sent
    );

    @Operation(summary = "Get Notification of given id")
    @GetMapping("api/notifications/{id}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetNotificationResponse getNotification(
            @Parameter(
                    name = "id",
                    description = "Notification id value",
                    required = true
            )
            @PathVariable("id")
            UUID id
    );

    @Operation(summary = "Get Notifications By Organization")
    @GetMapping("api/organizations/{organizationId}/notifications")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetNotificationsResponse getNotificationsByOrganization(
            @Parameter(
                    name = "page number",
                    description = "Page number to retrieve"
            )
            @RequestParam(defaultValue = "#{pageConfig.number}")
            int page,
            @Parameter(
                    name = "page size",
                    description = "Number of records per page"
            )
            @RequestParam(defaultValue = "#{pageConfig.size}")
            int size,
            @Parameter(
                    name = "organizationId",
                    description = "Organization id value",
                    required = true
            )
            @PathVariable("organizationId")
            UUID organizationId
    );

    @Operation(summary = "Get Seen or Unseen Notifications By Organization")
    @GetMapping("api/organizations/{organizationId}/notifications/{seen}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetNotificationsResponse getSeenOrUnseenNotificationsByOrganization(
            @Parameter(
                    name = "page number",
                    description = "Page number to retrieve"
            )
            @RequestParam(defaultValue = "#{pageConfig.number}")
            int page,
            @Parameter(
                    name = "page size",
                    description = "Number of records per page"
            )
            @RequestParam(defaultValue = "#{pageConfig.size}")
            int size,
            @Parameter(
                    name = "organizationId",
                    description = "Organization id value",
                    required = true
            )
            @PathVariable("organizationId")
            UUID organizationId,
            @Parameter(
                    name = "seen",
                    description = "Notification seen value",
                    required = true
            )
            @PathVariable("seen")
            boolean seen
    );

    @Operation(summary = "Get Notifications By Member")
    @GetMapping("api/members/{memberId}/notifications")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetNotificationsResponse getNotificationsByMember(
            @Parameter(
                    name = "page number",
                    description = "Page number to retrieve"
            )
            @RequestParam(defaultValue = "#{pageConfig.number}")
            int page,
            @Parameter(
                    name = "page size",
                    description = "Number of records per page"
            )
            @RequestParam(defaultValue = "#{pageConfig.size}")
            int size,
            @Parameter(
                    name = "memberId",
                    description = "Member id value",
                    required = true
            )
            @PathVariable("memberId")
            UUID memberId
    );

    @Operation(summary = "Get Seen or Unseen Notifications By Member")
    @GetMapping("api/members/{memberId}/notifications/{seen}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetNotificationsResponse getSeenOrUnseenNotificationsByMember(
            @Parameter(
                    name = "page number",
                    description = "Page number to retrieve"
            )
            @RequestParam(defaultValue = "#{pageConfig.number}")
            int page,
            @Parameter(
                    name = "page size",
                    description = "Number of records per page"
            )
            @RequestParam(defaultValue = "#{pageConfig.size}")
            int size,
            @Parameter(
                    name = "memberId",
                    description = "Member id value",
                    required = true
            )
            @PathVariable("memberId")
            UUID memberId,
            @Parameter(
                    name = "seen",
                    description = "Notification seen value",
                    required = true
            )
            @PathVariable("seen")
            boolean seen
    );

    @Operation(summary = "Get Notifications By User")
    @GetMapping("api/users/{userId}/notifications")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetNotificationsResponse getNotificationsByUser(
            @Parameter(
                    name = "page number",
                    description = "Page number to retrieve"
            )
            @RequestParam(defaultValue = "#{pageConfig.number}")
            int page,
            @Parameter(
                    name = "page size",
                    description = "Number of records per page"
            )
            @RequestParam(defaultValue = "#{pageConfig.size}")
            int size,
            @Parameter(
                    name = "userId",
                    description = "User id value",
                    required = true
            )
            @PathVariable("userId")
            UUID userId
    );

    @Operation(summary = "Get Seen or Unseen Notifications By User")
    @GetMapping("api/users/{userId}/notifications/{seen}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetNotificationsResponse getSeenOrUnseenNotificationsByUser(
            @Parameter(
                    name = "page number",
                    description = "Page number to retrieve"
            )
            @RequestParam(defaultValue = "#{pageConfig.number}")
            int page,
            @Parameter(
                    name = "page size",
                    description = "Number of records per page"
            )
            @RequestParam(defaultValue = "#{pageConfig.size}")
            int size,
            @Parameter(
                    name = "userId",
                    description = "User id value",
                    required = true
            )
            @PathVariable("userId")
            UUID userId,
            @Parameter(
                    name = "seen",
                    description = "Notification seen value",
                    required = true
            )
            @PathVariable("seen")
            boolean seen
    );


    @Operation(summary = "Create Notification")
    @PostMapping("api/notifications")
    @ResponseStatus(HttpStatus.CREATED)
    @ResponseBody
    GetNotificationResponse createNotification(
            @Parameter(
                    name = "PostNotificationRequest",
                    description = "PostNotificationRequest DTO",
                    schema = @Schema(implementation = PostNotificationRequest.class),
                    required = true
            )
            @RequestBody
            PostNotificationRequest postChatRequest
    );

    @Operation(summary = "Update Notification As Seen")
    @PatchMapping("api/notifications/seen/{id}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetNotificationResponse updateNotificationAsSeen(
            @Parameter(
                    name = "id",
                    description = "Notification id value",
                    required = true
            )
            @PathVariable("id")
            UUID id
    );

    @Operation(summary = "Update Notification As Sent")
    @PatchMapping("api/notifications/sent/{id}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetNotificationResponse updateNotificationAsSent(
            @Parameter(
                    name = "id",
                    description = "Notification id value",
                    required = true
            )
            @PathVariable("id")
            UUID id
    );

    @Operation(summary = "Delete Notification")
    @DeleteMapping("/api/notifications/{id}")
    @ResponseStatus(HttpStatus.NO_CONTENT)
    void deleteNotification(
            @Parameter(
                    name = "id",
                    description = "Notification id value",
                    required = true
            )
            @PathVariable("id")
            UUID id
    );
}
