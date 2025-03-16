package com.keepitup.chat.notification.api.ChatAndNotification.API.chat.function;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.dto.PostChatRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.entity.Chat;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class RequestToChatFunction implements Function<PostChatRequest, Chat> {
    @Override
    public Chat apply(PostChatRequest postChatRequest) {
        return Chat.builder()
                .title(postChatRequest.getTitle())
                .organizationId(postChatRequest.getOrganizationId())
                .build();
    }
}
