package com.keepitup.chat.notification.api.ChatAndNotification.API.chat.function;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.dto.GetChatResponse;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.entity.Chat;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class ChatToResponseFunction implements Function<Chat, GetChatResponse> {
    @Override
    public GetChatResponse apply(Chat chat) {
        return GetChatResponse.builder()
                .id(chat.getId())
                .title(chat.getTitle())
                .dateOfCreation(chat.getDateOfCreation())
                .organizationId(chat.getOrganizationId())
                .build();
    }
}
