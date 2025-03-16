package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.controller.impl;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.entity.Chat;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.service.impl.ChatDefaultService;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.service.impl.ChatMemberDefaultService;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.controller.api.ChatMessageController;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.dto.GetChatMessagesResponse;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.dto.PatchChatMessageRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.dto.PatchChatMessageWebSocketRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.dto.PostChatMessageRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.entity.ChatMessage;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.function.ChatMessagesToResponseFunction;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.function.RequestToChatMessageFunction;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.function.UpdateChatMessageWithRequestFunction;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.service.impl.ChatMessageDefaultService;
//import com.keepitup.chat.notification.api.ChatAndNotification.API.configuration.SecurityService;
import lombok.extern.java.Log;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.data.domain.Sort;
import org.springframework.http.HttpStatus;
import org.springframework.messaging.handler.annotation.DestinationVariable;
import org.springframework.stereotype.Controller;
import org.springframework.web.server.ResponseStatusException;

import java.util.UUID;

@Controller
@Log
public class ChatMessageDefaultController implements ChatMessageController {
    private final ChatMessageDefaultService chatMessageService;
    private final ChatDefaultService chatService;
    private final ChatMemberDefaultService chatMemberService;
    //private final SecurityService securityService;
    private final RequestToChatMessageFunction requestToChatMessageFunction;
    private final UpdateChatMessageWithRequestFunction updateChatMessageWithRequestFunction;
    private final ChatMessagesToResponseFunction chatMessagesToResponseFunction;

    @Autowired
    public ChatMessageDefaultController(
            ChatMessageDefaultService chatMessageService,
            ChatDefaultService chatService,
            ChatMemberDefaultService chatMemberService,
            //SecurityService securityService,
            RequestToChatMessageFunction requestToChatMessageFunction,
            UpdateChatMessageWithRequestFunction updateChatMessageWithRequestFunction,
            ChatMessagesToResponseFunction chatMessagesToResponseFunction
    ) {
       this.chatMessageService = chatMessageService;
       this.chatService = chatService;
       this.chatMemberService = chatMemberService;
       //this.securityService = securityService;
       this.requestToChatMessageFunction = requestToChatMessageFunction;
       this.updateChatMessageWithRequestFunction = updateChatMessageWithRequestFunction;
       this.chatMessagesToResponseFunction = chatMessagesToResponseFunction;
    }

    @Override
    public GetChatMessagesResponse getChatMessagesByChat(int page, int size, UUID chatId) {
        PageRequest pageRequest = PageRequest.of(page, size, Sort.by(Sort.Direction.DESC, "dateOfCreation"));
        Chat chat = chatService.find(chatId)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND));

//        if (!securityService.isChatMember(chat)) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        Integer count = chatMessageService.findAllByChat(chat, Pageable.unpaged()).getNumberOfElements();

        return chatMessagesToResponseFunction.apply(chatMessageService.findAllByChat(chat, pageRequest), count);
    }

    @Override
    public ChatMessage sendMessage(
            @DestinationVariable UUID chatId,
            PostChatMessageRequest postChatMessageRequest
    ) {
        Chat chat = chatService.find(postChatMessageRequest.getChat()).orElseThrow(
                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
        );

//        if (!securityService.isChatMember(chat)) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        chatMemberService.find(postChatMessageRequest.getChatMember()).orElseThrow(
                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
        );

        return chatMessageService.create(requestToChatMessageFunction.apply(postChatMessageRequest));
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

//        if (!securityService.isChatMember(chat)) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        chatMessageService.update(updateChatMessageWithRequestFunction.apply(chatMessage, patchChatMessageRequest));
    }

    public void handleViewedMessage(
            UUID chatId,
            PatchChatMessageWebSocketRequest patchChatMessageWebSocketRequest
    ) {
        ChatMessage chatMessage = chatMessageService.find(patchChatMessageWebSocketRequest.getChatMessageId()).orElseThrow(
                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
        );

        Chat chat = chatService.find(chatId).orElseThrow(
                () -> new ResponseStatusException(HttpStatus.NOT_FOUND)
        );

//        if (!securityService.isChatMember(chat)) {
//            throw new ResponseStatusException(HttpStatus.FORBIDDEN);
//        }

        PatchChatMessageRequest patchChatMessageRequest = new PatchChatMessageRequest();
        patchChatMessageRequest.setViewedBy(patchChatMessageWebSocketRequest.getViewedBy());

        chatMessageService.update(updateChatMessageWithRequestFunction.apply(chatMessage, patchChatMessageRequest));
    }
}
