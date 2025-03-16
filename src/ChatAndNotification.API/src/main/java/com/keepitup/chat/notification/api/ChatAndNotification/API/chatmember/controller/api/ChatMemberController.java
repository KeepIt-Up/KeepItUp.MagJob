package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.controller.api;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.dto.*;
import com.keepitup.chat.notification.api.ChatAndNotification.API.configuration.PageConfig;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

import java.math.BigInteger;
import java.util.UUID;

@Tag(name = "Chat Member Controller")
public interface ChatMemberController {
    PageConfig pageConfig = new PageConfig();

    @Operation(summary = "Get Chat Members By Member")
    @GetMapping("api/members/{memberId}/chat-members")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetChatMembersResponse getChatMembersByMember(
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

    @Operation(summary = "Get Chat Members By Chat")
    @GetMapping("api/chats/{chatId}/chat-members")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetChatMembersResponse getChatMembersByChat(
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
                    name = "chatId",
                    description = "Member id value",
                    required = true
            )
            @PathVariable("chatId")
            UUID chatId
    );

    @Operation(summary = "Invite chat member to chat")
    @PostMapping("api/chat-members")
    @ResponseStatus(HttpStatus.CREATED)
    @ResponseBody
    GetChatMemberResponse createChatMember(
            @Parameter(
                    name = "PostChatMemberRequest",
                    description = "PostChatMemberRequest DTO",
                    schema = @Schema(implementation = PostChatMemberRequest.class),
                    required = true
            )
            @RequestBody
            PostChatMemberRequest postChatMemberRequest
    );

    @Operation(summary = "Update Chat Member nickname")
    @PatchMapping("api/chat-members/{id}")
    @ResponseStatus(HttpStatus.CREATED)
    @ResponseBody
    GetChatMemberResponse setNickname(
            @Parameter(
                    name = "id",
                    description = "Chat Member id value",
                    required = true
            )
            @PathVariable("id")
            UUID id,
            @Parameter(
                    name = "PatchChatMemberRequest",
                    description = "PatchChatMemberRequest DTO",
                    schema = @Schema(implementation = PatchChatMemberRequest.class),
                    required = true
            )
            @RequestBody
            PatchChatMemberRequest patchChatMemberRequest
    );

    @Operation(summary = "Accept Invitation to chat")
    @PostMapping("/api/chat-members/accept")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    void acceptInvitation(
            @Parameter(
                    name = "AcceptInvitationToChatRequest",
                    description = "AcceptInvitationToChatRequest DTO",
                    schema = @Schema(implementation = AcceptInvitationToChatRequest.class),
                    required = true
            )
            @RequestBody
            AcceptInvitationToChatRequest request
    );

    @Operation(summary = "Reject Invitation to chat")
    @PostMapping("/api/chat-members/reject")
    @ResponseStatus(HttpStatus.OK)
    void rejectInvitation(
            @Parameter(
                    name = "AcceptInvitationToChatRequest",
                    description = "AcceptInvitationToChatRequest DTO",
                    schema = @Schema(implementation = AcceptInvitationToChatRequest.class),
                    required = true
            )
            @RequestBody
            AcceptInvitationToChatRequest request
    );

    @Operation(summary = "Remove admin access from chat member")
    @PostMapping("api/chat-members/{id}/admin/remove")
    @ResponseStatus(HttpStatus.OK)
    void removeAdminAccess(
            @Parameter(
                    name = "id",
                    description = "Chat Member id value",
                    required = true
            )
            @PathVariable("id")
            UUID id
    );

    @Operation(summary = "Give admin access to chat member")
    @PostMapping("api/chat-members/{id}/admin/add")
    @ResponseStatus(HttpStatus.OK)
    void giveAdminAccess(
            @Parameter(
                    name = "id",
                    description = "Chat Member id value",
                    required = true
            )
            @PathVariable("id")
            UUID id
    );

}
