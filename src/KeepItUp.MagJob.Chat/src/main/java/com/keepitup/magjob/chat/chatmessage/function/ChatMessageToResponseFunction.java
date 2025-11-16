package com.keepitup.magjob.chat.chatmessage.function;

import com.keepitup.magjob.chat.chatmessage.dto.GetChatMessageResponse;
import com.keepitup.magjob.chat.chatmessage.entity.ChatMessage;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class ChatMessageToResponseFunction implements Function<ChatMessage, GetChatMessageResponse> {
    @Override
    public GetChatMessageResponse apply(ChatMessage chatMessage) {
        return GetChatMessageResponse.builder()
                .id(chatMessage.getId())
                .content(chatMessage.getContent())
                .firstAndLastName(chatMessage.getFirstAndLastName())
                .dateOfCreation(chatMessage.getDateOfCreation())
                .chatMember(chatMessage.getChatMember() != null ?
                        GetChatMessageResponse.ChatMember.builder()
                                .id(chatMessage.getChatMember().getId())
                                .nickname(chatMessage.getChatMember().getNickname())
                                .memberId(chatMessage.getChatMember().getMemberId())
                                .build() : null)
                .chat(GetChatMessageResponse.Chat.builder()
                        .id(chatMessage.getChat().getId())
                        .title(chatMessage.getChat().getTitle())
                        .organizationId(chatMessage.getChat().getOrganizationId())
                        .build())
                .build();
    }
}
