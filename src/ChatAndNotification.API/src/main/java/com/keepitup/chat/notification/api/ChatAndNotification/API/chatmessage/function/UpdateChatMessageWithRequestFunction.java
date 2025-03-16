package com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.function;

import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.dto.PatchChatMessageRequest;
import com.keepitup.chat.notification.api.ChatAndNotification.API.chatmessage.entity.ChatMessage;
import org.springframework.stereotype.Component;

import java.util.ArrayList;
import java.util.List;
import java.util.function.BiFunction;

@Component
public class UpdateChatMessageWithRequestFunction implements BiFunction<ChatMessage, PatchChatMessageRequest, ChatMessage> {
    @Override
    public ChatMessage apply(ChatMessage chatMessage, PatchChatMessageRequest patchChatMessageRequest) {
        List<String> updatedViewedBy = new ArrayList<>(chatMessage.getViewedBy() != null ? chatMessage.getViewedBy() : new ArrayList<>());

        String newViewer = patchChatMessageRequest.getViewedBy();
        if (newViewer != null && !updatedViewedBy.contains(newViewer)) {
            updatedViewedBy.add(newViewer);
        }

        return ChatMessage.builder()
                .id(chatMessage.getId())
                .content(chatMessage.getContent())
                .chatMember(chatMessage.getChatMember())
                .chat(chatMessage.getChat())
                .dateOfCreation(chatMessage.getDateOfCreation())
                .attachment(chatMessage.getAttachment())
                .viewedBy(updatedViewedBy)
                .firstAndLastName(chatMessage.getFirstAndLastName())
                .build();
    }
}
