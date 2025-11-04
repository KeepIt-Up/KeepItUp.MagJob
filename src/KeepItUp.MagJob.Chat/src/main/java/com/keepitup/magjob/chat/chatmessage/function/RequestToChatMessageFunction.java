package com.keepitup.magjob.chat.chatmessage.function;

import com.keepitup.magjob.chat.chat.entity.Chat;
import com.keepitup.magjob.chat.chatmember.entity.ChatMember;
import com.keepitup.magjob.chat.chatmessage.dto.PostChatMessageRequest;
import com.keepitup.magjob.chat.chatmessage.entity.ChatMessage;
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
                .firstAndLastName(postChatMessageRequest.getFirstAndLastName())
                .chat(chat)
                .chatMember(chatMember)
                .build();
    }
}
