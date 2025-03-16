package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.function;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chat.entity.Chat;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.dto.PostChatMemberRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmember.entity.ChatMember;
import org.springframework.stereotype.Component;

import java.util.function.Function;

@Component
public class RequestToChatMemberFunction implements Function<PostChatMemberRequest, ChatMember> {
    @Override
    public ChatMember apply(PostChatMemberRequest postChatMemberRequest) {
        return ChatMember.builder()
                .nickname(postChatMemberRequest.getNickname())
                .memberId(postChatMemberRequest.getMemberId())
                .chat(Chat.builder()
                        .id(postChatMemberRequest.getChatId())
                        .build())
                .build();
    }
}
