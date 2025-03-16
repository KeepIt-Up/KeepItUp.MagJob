package com.keepitup.chat.notification.api.ChatAndNotification.API.chat.function;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.dto.GetChatsResponse;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.entity.Chat;
import org.springframework.data.domain.Page;
import org.springframework.stereotype.Component;

import java.util.function.BiFunction;

@Component
public class ChatsToResponseFunction implements BiFunction<Page<Chat>, Integer, GetChatsResponse> {
    @Override
    public GetChatsResponse apply(Page<Chat> chats, Integer count) {
        return GetChatsResponse.builder()
                .chats(chats.stream()
                        .map(chat -> GetChatsResponse.Chat.builder()
                                .id(chat.getId())
                                .title(chat.getTitle())
                                .organizationId(chat.getOrganizationId())
                                .build())
                        .toList())
                .count(count)
                .build();
    }
}
