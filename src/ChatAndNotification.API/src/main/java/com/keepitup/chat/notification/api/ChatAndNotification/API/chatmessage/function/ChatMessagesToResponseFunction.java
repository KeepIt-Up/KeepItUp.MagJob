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
                                .viewedBy(chatMessage.getViewedBy())
                                .firstAndLastName(chatMessage.getFirstAndLastName())
                                .chatMember(chatMessage.getChatMember() != null ?
                                        GetChatMessagesResponse.ChatMessage.ChatMember.builder()
                                                .id(chatMessage.getChatMember().getId())
                                                .nickname(chatMessage.getChatMember().getNickname())
                                                .memberId(chatMessage.getChatMember().getMemberId())
                                                .build() : null)
                                .chat(chatMessage.getChat() != null ?
                                        GetChatMessagesResponse.ChatMessage.Chat.builder()
                                                .id(chatMessage.getChat().getId())
                                                .title(chatMessage.getChat().getTitle())
                                                .organizationId(chatMessage.getChat().getOrganizationId())
                                                .build() : null)
                                .build())
                        .toList())
                .count(count)
                .build();
    }
}
