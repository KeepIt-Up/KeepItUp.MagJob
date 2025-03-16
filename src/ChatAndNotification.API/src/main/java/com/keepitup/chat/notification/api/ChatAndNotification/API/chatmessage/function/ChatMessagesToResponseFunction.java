package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.function;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.dto.GetChatMessagesResponse;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.entity.ChatMessage;
import org.springframework.data.domain.Page;
import org.springframework.stereotype.Component;

import java.util.function.BiFunction;

@Component
public class ChatMessagesToResponseFunction implements BiFunction<Page<ChatMessage>, Integer, GetChatMessagesResponse> {
    @Override
    public GetChatMessagesResponse apply(Page<ChatMessage> chatMessages, Integer count) {
        return GetChatMessagesResponse.builder()
                .chatMessages(chatMessages.stream()
                        .map(chatMessage -> GetChatMessagesResponse.ChatMessage.builder()
                                .id(chatMessage.getId())
                                .content(chatMessage.getContent())
                                .attachment(chatMessage.getAttachment())
                                .dateOfCreation(chatMessage.getDateOfCreation())
                                .chatMemberId(chatMessage.getChatMember().getId())
                                .viewedBy(chatMessage.getViewedBy())
                                .firstAndLastName(chatMessage.getFirstAndLastName())
                                .build())
                        .toList())
                .count(count)
                .build();
    }
}
