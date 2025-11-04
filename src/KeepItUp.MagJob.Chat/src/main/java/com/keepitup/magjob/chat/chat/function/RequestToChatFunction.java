package com.keepitup.magjob.chat.chat.function;

import com.keepitup.magjob.chat.chat.dto.PostChatRequest;
import com.keepitup.magjob.chat.chat.entity.Chat;
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
