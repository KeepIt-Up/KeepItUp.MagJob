package com.keepitup.chat.notification.api.ChatAndNotification.API.chat.function;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.dto.PatchChatRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.entity.Chat;
import org.springframework.stereotype.Component;

import java.util.function.BiFunction;

@Component
public class UpdateChatWithRequestFunction implements BiFunction<Chat, PatchChatRequest, Chat> {
    @Override
    public Chat apply(Chat chat, PatchChatRequest patchChatRequest) {
        return Chat.builder()
                .id(chat.getId())
                .title(patchChatRequest.getTitle())
                .organizationId(chat.getOrganizationId())
                .dateOfCreation(chat.getDateOfCreation())
                .build();
    }
}
