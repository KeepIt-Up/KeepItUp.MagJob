package com.keepitup.magjob.chat.chatmember.function;

import com.keepitup.magjob.chat.chat.entity.Chat;
import com.keepitup.magjob.chat.chatmember.dto.PostChatMemberRequest;
import com.keepitup.magjob.chat.chatmember.entity.ChatMember;
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
