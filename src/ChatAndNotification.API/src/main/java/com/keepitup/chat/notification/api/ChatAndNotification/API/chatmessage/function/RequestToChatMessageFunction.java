package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.function;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.entity.Chat;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.entity.ChatMember;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.dto.PostChatMessageRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.entity.ChatMessage;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class RequestToChatMessageFunction implements Function<PostChatMessageRequest, ChatMessage> {
    @Override
    public ChatMessage apply(PostChatMessageRequest postChatMessageRequest) {
        Chat chat = Chat.builder()
                .id(postChatMessageRequest.getChat())
                .build();

        ChatMember chatMember = ChatMember.builder()
                .id(postChatMessageRequest.getChatMember())
                .chat(chat)
                .build();

        return ChatMessage.builder()
                .content(postChatMessageRequest.getContent())
                .attachment(postChatMessageRequest.getAttachment())
                .chat(chat)
                .chatMember(chatMember)
                .build();
    }
}
