package com.keepitup.chat.notification.api.ChatAndNotification.API.chat.controller.api;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.dto.GetChatResponse;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.dto.GetChatsResponse;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.dto.PatchChatRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.dto.PostChatRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.configuration.PageConfig;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.*;

import java.util.UUID;

@Tag(name = "Chat Controller")
public interface ChatController {
    PageConfig pageConfig = new PageConfig();

    @Operation(summary = "Get all chats")
    @GetMapping("api/chats")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetChatsResponse getChats(
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

    @Operation(summary = "Get Chat of given id")
    @GetMapping("api/chats/{id}")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetChatResponse getChat(
            @Parameter(
                    name = "id",
                    description = "Chat id value",
                    required = true
            )
            @PathVariable("id")
            UUID id
    );

    @Operation(summary = "Get Chats By Organization")
    @GetMapping("api/organizations/{organizationId}/chats")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetChatsResponse getChatsByOrganization(
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


    @Operation(summary = "Get Chats By Member")
    @GetMapping("api/members/{memberId}/chats")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetChatsResponse getChatsByMember(
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

    @Operation(summary = "Create Chat")
    @PostMapping("api/chats")
    @ResponseStatus(HttpStatus.CREATED)
    @ResponseBody
    GetChatResponse createChat(
            @Parameter(
                    name = "PostChatRequest",
                    description = "PostChatRequest DTO",
                    schema = @Schema(implementation = PostChatRequest.class),
                    required = true
            )
            @RequestBody
            PostChatRequest postChatRequest
    );

    @Operation(summary = "Update Chat")
    @PatchMapping("api/chats/{id}")
    @ResponseStatus(HttpStatus.CREATED)
    @ResponseBody
    GetChatResponse updateChat(
            @Parameter(
                    name = "id",
                    description = "Chat id value",
                    required = true
            )
            @PathVariable("id")
            UUID id,
            @Parameter(
                    name = "PatchChatRequest",
                    description = "PatchChatRequest DTO",
                    schema = @Schema(implementation = PatchChatRequest.class),
                    required = true
            )
            @RequestBody
            PatchChatRequest patchChatRequest
    );

    @Operation(summary = "Delete Chat")
    @DeleteMapping("/api/chats/{id}")
    @ResponseStatus(HttpStatus.NO_CONTENT)
    void deleteChat(
            @Parameter(
                    name = "id",
                    description = "Chat id value",
                    required = true
            )
            @PathVariable("id")
            UUID id
    );
}
