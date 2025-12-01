package com.keepitup.magjob.chat.chatmessage.controller.api;

import com.keepitup.magjob.chat.chatmessage.dto.GetChatMessageResponse;
import com.keepitup.magjob.chat.chatmessage.dto.GetChatMessagesResponse;
import com.keepitup.magjob.chat.chatmessage.dto.PostChatMessageRequest;
import com.keepitup.magjob.chat.chatmessage.dto.TypingEventRequest;
import com.keepitup.magjob.chat.chatmessage.entity.ChatMessage;
import com.keepitup.magjob.chat.configuration.PageConfig;
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
    GetChatMessageResponse sendMessage(
            @Parameter(name = "chatId", description = "Chat id value", required = true)
            @DestinationVariable("chatId") UUID chatId,
            @Parameter(name = "PostChatMessageRequest", description = "PostChatMessageRequest DTO", 
                      schema = @Schema(implementation = PostChatMessageRequest.class), required = true)
            PostChatMessageRequest postChatMessageRequest
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
