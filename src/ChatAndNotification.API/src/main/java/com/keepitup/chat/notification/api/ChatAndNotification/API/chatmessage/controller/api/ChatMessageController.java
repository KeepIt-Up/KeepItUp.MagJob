package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.controller.api;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.dto.GetChatMessagesResponse;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.dto.PatchChatMessageRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.dto.PatchChatMessageWebSocketRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.dto.PostChatMessageRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.dto.TypingEventRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.entity.ChatMessage;
import com.keepitup.chat.notification.api.ChatAndNotification.API.configuration.PageConfig;
import io.swagger.v3.oas.annotations.Operation;
import io.swagger.v3.oas.annotations.Parameter;
import io.swagger.v3.oas.annotations.media.Schema;
import io.swagger.v3.oas.annotations.tags.Tag;
import org.springframework.http.HttpStatus;
import org.springframework.messaging.handler.annotation.DestinationVariable;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.SendTo;
import org.springframework.web.bind.annotation.*;

import java.util.UUID;

@Tag(name = "ChatMessageController")
public interface ChatMessageController {
    PageConfig pageConfig = new PageConfig();

    // HTTP endpoint - pobieranie wiadomości
    @Operation(summary = "Get Chat Messages By Chat")
    @GetMapping("api/chats/{id}/chat-messages")
    @ResponseStatus(HttpStatus.OK)
    @ResponseBody
    GetChatMessagesResponse getChatMessagesByChat(
            @Parameter(name = "page number", description = "Page number to retrieve")
            @RequestParam(defaultValue = "#{pageConfig.number}") int page,
            @Parameter(name = "page size", description = "Number of records per page")
            @RequestParam(defaultValue = "#{pageConfig.size}") int size,
            @Parameter(name = "chatId", description = "Chat id value", required = true)
            @PathVariable("id") UUID chatId
    );

    @MessageMapping("/chat/{chatId}/sendMessage")
    @SendTo("/topic/chat/{chatId}")
    ChatMessage sendMessage(
            @Parameter(name = "chatId", description = "Chat id value", required = true)
            @DestinationVariable("chatId") UUID chatId,
            @Parameter(name = "PostChatMessageRequest", description = "PostChatMessageRequest DTO", 
                      schema = @Schema(implementation = PostChatMessageRequest.class), required = true)
            PostChatMessageRequest postChatMessageRequest
    );

    @MessageMapping("/chat/{chatId}/messageViewed")
    @SendTo("/topic/chat/{chatId}/viewed")
    void handleViewedMessage(
            @Parameter(name = "chatId", description = "Chat id value", required = true)
            @DestinationVariable("chatId") UUID chatId,
            @Parameter(name = "PatchChatMessageWebSocketRequest", description = "PatchChatMessageWebSocketRequest DTO", 
                      schema = @Schema(implementation = PatchChatMessageWebSocketRequest.class), required = true)
            PatchChatMessageWebSocketRequest patchChatMessageWebSocketRequest
    );

    @PatchMapping("/api/messages/{id}")
    @ResponseStatus(HttpStatus.OK)
    void markMessageAsViewed(
            @Parameter(name = "id", description = "Message id value", required = true)
            @PathVariable("id") UUID id,
            @RequestBody PatchChatMessageRequest patchChatMessageRequest
    );

    @MessageMapping("/chat/{chatId}/typing")
    @SendTo("/topic/chat/{chatId}/typing")
    TypingEventRequest handleTypingEvent(
            @Parameter(name = "chatId", description = "Chat id value", required = true)
            @DestinationVariable("chatId") UUID chatId,
            @Parameter(name = "TypingEventRequest", description = "TypingEventRequest DTO", 
                      schema = @Schema(implementation = TypingEventRequest.class), required = true)
            TypingEventRequest typingEventRequest
    );
}