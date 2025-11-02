package com.keepitup.magjob.chat.chat.function;

import com.keepitup.magjob.chat.chat.dto.GetChatResponse;
import com.keepitup.magjob.chat.chat.entity.Chat;
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
