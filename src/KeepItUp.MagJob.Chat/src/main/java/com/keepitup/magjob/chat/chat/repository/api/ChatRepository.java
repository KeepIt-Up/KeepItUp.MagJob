package com.keepitup.magjob.chat.chat.repository.api;

import com.keepitup.magjob.chat.chat.entity.Chat;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.math.BigInteger;
import java.util.Optional;
import java.util.UUID;

@Repository
public interface ChatRepository extends JpaRepository<Chat, UUID> {
    Optional<Chat> findByTitle(String title);

    Page<Chat> findAllByOrganizationId(UUID organizationId, Pageable pageable);
}
