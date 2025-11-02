package com.keepitup.magjob.chat.chatmessage.controller.impl;

import com.keepitup.magjob.chat.chat.entity.Chat;
import com.keepitup.magjob.chat.chat.service.impl.ChatDefaultService;
import com.keepitup.magjob.chat.chatmember.entity.ChatMember;
import com.keepitup.magjob.chat.chatmember.service.impl.ChatMemberDefaultService;
import com.keepitup.magjob.chat.chatmessage.controller.api.ChatMessageController;
import com.keepitup.magjob.chat.chatmessage.dto.GetChatMessageResponse;
import com.keepitup.magjob.chat.chatmessage.dto.GetChatMessagesResponse;
import com.keepitup.magjob.chat.chatmessage.dto.PatchChatMessageRequest;
import com.keepitup.magjob.chat.chatmessage.dto.PatchChatMessageWebSocketRequest;
import com.keepitup.magjob.chat.chatmessage.dto.PostChatMessageRequest;
import com.keepitup.magjob.chat.chatmessage.dto.TypingEventRequest;
import com.keepitup.magjob.chat.chatmessage.entity.ChatMessage;
import com.keepitup.magjob.chat.chatmessage.function.ChatMessagesToResponseFunction;
import com.keepitup.magjob.chat.chatmessage.function.RequestToChatMessageFunction;
import com.keepitup.magjob.chat.chatmessage.function.UpdateChatMessageWithRequestFunction;
import com.keepitup.magjob.chat.chatmessage.service.impl.ChatMessageDefaultService;
import lombok.extern.java.Log;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.data.domain.Sort;
import org.springframework.http.HttpStatus;
import org.springframework.messaging.handler.annotation.DestinationVariable;
import org.springframework.messaging.handler.annotation.MessageMapping;
import org.springframework.messaging.handler.annotation.SendTo;
import org.springframework.stereotype.Controller;
import org.springframework.web.server.ResponseStatusException;

import java.util.UUID;

@Controller
@Log
public class ChatMessageDefaultController implements ChatMessageController {
    private final ChatMessageDefaultService chatMessageService;
    private final ChatDefaultService chatService;
    private final ChatMemberDefaultService chatMemberService;
    private final RequestToChatMessageFunction requestToChatMessageFunction;
    private final UpdateChatMessageWithRequestFunction updateChatMessageWithRequestFunction;
    private final ChatMessagesToResponseFunction chatMessagesToResponseFunction;

    @Autowired
    public ChatMessageDefaultController(
            ChatMessageDefaultService chatMessageService,
            ChatDefaultService chatService,
            ChatMemberDefaultService chatMemberService,
            RequestToChatMessageFunction requestToChatMessageFunction,
            UpdateChatMessageWithRequestFunction updateChatMessageWithRequestFunction,
            ChatMessagesToResponseFunction chatMessagesToResponseFunction
    ) {
       this.chatMessageService = chatMessageService;
       this.chatService = chatService;
       this.chatMemberService = chatMemberService;
       this.requestToChatMessageFunction = requestToChatMessageFunction;
       this.updateChatMessageWithRequestFunction = updateChatMessageWithRequestFunction;
       this.chatMessagesToResponseFunction = chatMessagesToResponseFunction;
    }

    @Override
    public GetChatMessagesResponse getChatMessagesByChat(int page, int size, UUID chatId) {
        PageRequest pageRequest = PageRequest.of(page, size, Sort.by(Sort.Direction.ASC, "dateOfCreation"));
        Chat chat = chatService.find(chatId)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

        Integer count = chatMessageService.findAllByChat(chat, Pageable.unpaged()).getNumberOfElements();

        return chatMessagesToResponseFunction.apply(chatMessageService.findAllByChat(chat, pageRequest), count);
    }

    @Override
    @MessageMapping("/chat/{chatId}/sendMessage")
    @SendTo("/topic/chat/{chatId}")
    public GetChatMessageResponse sendMessage(
            @DestinationVariable UUID chatId,
            PostChatMessageRequest postChatMessageRequest
    ) {
        log.info("Received WebSocket message for chat: " + chatId);
        log.info("Message content: " + postChatMessageRequest.getContent());
        log.info("ChatMember ID: " + postChatMessageRequest.getChatMember());
        
        Chat chat = chatService.find(postChatMessageRequest.getChat()).orElseThrow(
                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
        );

        ChatMember chatMember = chatMemberService.find(postChatMessageRequest.getChatMember()).orElseThrow(
                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
        );

        ChatMessage createdMessage = chatMessageService.create(requestToChatMessageFunction.apply(postChatMessageRequest));
        log.info("Created message with ID: " + createdMessage.getId());
        log.info("Broadcasting to topic: /topic/chat/" + chatId);
        
        GetChatMessageResponse response = GetChatMessageResponse.builder()
                .id(createdMessage.getId())
                .content(createdMessage.getContent())
                .attachment(createdMessage.getAttachment())
                .viewedBy(createdMessage.getViewedBy())
                .firstAndLastName(createdMessage.getFirstAndLastName())
                .dateOfCreation(createdMessage.getDateOfCreation())
                .chatMember(GetChatMessageResponse.ChatMember.builder()
                        .id(chatMember.getId())
                        .nickname(chatMember.getNickname())
                        .memberId(chatMember.getMemberId())
                        .build())
                .chat(GetChatMessageResponse.Chat.builder()
                        .id(chat.getId())
                        .title(chat.getTitle())
                        .organizationId(chat.getOrganizationId())
                        .build())
                .build();
        
        return response;
    }

    @Override
    public void markMessageAsViewed(
            UUID id,
            PatchChatMessageRequest patchChatMessageRequest
    ) {
        ChatMessage chatMessage = chatMessageService.find(id).orElseThrow(
                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
        );

        Chat chat = chatService.find(id).orElseThrow(
                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
        );

        chatMessageService.update(updateChatMessageWithRequestFunction.apply(chatMessage, patchChatMessageRequest));
    }

    @MessageMapping("/chat/{chatId}/messageViewed")
    @SendTo("/topic/chat/{chatId}/viewed")
    public void handleViewedMessage(
            @DestinationVariable UUID chatId,
            PatchChatMessageWebSocketRequest patchChatMessageWebSocketRequest
    ) {
        ChatMessage chatMessage = chatMessageService.find(patchChatMessageWebSocketRequest.getChatMessageId()).orElseThrow(
                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
        );

        Chat chat = chatService.find(chatId).orElseThrow(
                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
        );

        PatchChatMessageRequest patchChatMessageRequest = new PatchChatMessageRequest();
        patchChatMessageRequest.setViewedBy(patchChatMessageWebSocketRequest.getViewedBy());

        chatMessageService.update(updateChatMessageWithRequestFunction.apply(chatMessage, patchChatMessageRequest));
    }

    @Override
    @MessageMapping("/chat/{chatId}/typing")
    @SendTo("/topic/chat/{chatId}/typing")
    public TypingEventRequest handleTypingEvent(
            @DestinationVariable UUID chatId,
            TypingEventRequest typingEventRequest
    ) {
        log.info("Received typing event for chat: " + chatId);
        log.info("Typing event type: " + typingEventRequest.getType());
        log.info("Member: " + typingEventRequest.getMemberName() + " (ID: " + typingEventRequest.getMemberId() + ")");
        log.info("Timestamp: " + typingEventRequest.getTimestamp());
        
        chatService.find(chatId).orElseThrow(
                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
        );

        return typingEventRequest;
    }
}
